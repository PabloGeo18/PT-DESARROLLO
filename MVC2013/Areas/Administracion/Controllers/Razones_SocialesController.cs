using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Src.Seguridad.To;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Areas.Administracion.Controllers
{
    public class Razones_SocialesController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Razones_Sociales
        public ActionResult Index()
        {
            var razones_Sociales = db.Razones_Sociales.Where(x => x.eliminado == false).OrderBy(y => y.id_cliente).Include(r => r.Clientes).Include(r => r.Municipios).Include(r => r.Municipios1).Include(r => r.Municipios2).Include(r => r.Usuarios).Include(r => r.Usuarios1).Include(r => r.Usuarios2);
            return View(razones_Sociales.ToList());
        }

        // GET: Administracion/Razones_Sociales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Razones_Sociales razones_Sociales = db.Razones_Sociales.Find(id);
            if (razones_Sociales == null)
            {
                return HttpNotFound();
            }
            return View(razones_Sociales);
        }

        // GET: Administracion/Razones_Sociales/Create
        public ActionResult Create()
        {
            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.eliminado == false), "id_cliente", "nombre");
            ViewBag.municipio_direccion_facturacion = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre");
            ViewBag.municipio_direccion_fiscal = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre");
            ViewBag.municipio_direccion_fisica = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre");
            return View();
        }

        // POST: Administracion/Razones_Sociales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_razon_social,id_cliente,razon_social,nombre_comercial,nit,direccion_fisica,municipio_direccion_fisica,direccion_fiscal,municipio_direccion_fiscal,direccion_facturacion,municipio_direccion_facturacion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Razones_Sociales razones_Sociales)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                razones_Sociales.activo = true;
                razones_Sociales.eliminado = false;
                razones_Sociales.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                razones_Sociales.fecha_creacion = DateTime.Now;
                db.Razones_Sociales.Add(razones_Sociales);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.eliminado == false), "id_cliente", "nombre", razones_Sociales.id_cliente);
            ViewBag.municipio_direccion_facturacion = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razones_Sociales.municipio_direccion_facturacion);
            ViewBag.municipio_direccion_fiscal = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razones_Sociales.municipio_direccion_fiscal);
            ViewBag.municipio_direccion_fisica = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razones_Sociales.municipio_direccion_fisica);
            return View(razones_Sociales);
        }

        // GET: Administracion/Razones_Sociales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Razones_Sociales razones_Sociales = db.Razones_Sociales.Find(id);
            if (razones_Sociales == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.eliminado == false), "id_cliente", "nombre", razones_Sociales.id_cliente);
            ViewBag.municipio_direccion_facturacion = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razones_Sociales.municipio_direccion_facturacion);
            ViewBag.municipio_direccion_fiscal = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razones_Sociales.municipio_direccion_fiscal);
            ViewBag.municipio_direccion_fisica = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razones_Sociales.municipio_direccion_fisica);
            var lista = db.Contactos.Where(c => !c.eliminado).Select(c => c.id_contacto).Except(db.Razon_Social_Contacto.Where(r => r.id_razon_social == id).Select(c => c.id_contacto));
            ViewBag.id_contacto = new SelectList(db.Contactos.Where(c => lista.Contains(c.id_contacto)), "id_contacto", "nombre");
            return View(razones_Sociales);
        }

        // POST: Administracion/Razones_Sociales/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_razon_social,id_cliente,razon_social,nombre_comercial,nit,direccion_fisica,municipio_direccion_fisica,direccion_fiscal,municipio_direccion_fiscal,direccion_facturacion,municipio_direccion_facturacion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Razones_Sociales razones_Sociales)
        {
            if (ModelState.IsValid)
            {
                Razones_Sociales razones_sociales_edit = db.Razones_Sociales.Find(razones_Sociales.id_razon_social);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                razones_sociales_edit.razon_social = razones_Sociales.razon_social;
                razones_sociales_edit.nombre_comercial = razones_Sociales.nombre_comercial;
                razones_sociales_edit.nit = razones_Sociales.nit;
                razones_sociales_edit.direccion_fisica = razones_Sociales.direccion_fisica;
                razones_sociales_edit.direccion_fiscal = razones_Sociales.direccion_fiscal;
                razones_sociales_edit.direccion_facturacion = razones_Sociales.direccion_facturacion;
                razones_sociales_edit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                razones_sociales_edit.fecha_modificacion = DateTime.Now;
                razones_sociales_edit.eliminado = false;
                razones_sociales_edit.activo = true;
                db.Entry(razones_sociales_edit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_cliente = new SelectList(db.Clientes.Where(x => x.eliminado == false), "id_cliente", "nombre", razones_Sociales.id_cliente);
            ViewBag.municipio_direccion_facturacion = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razones_Sociales.municipio_direccion_facturacion);
            ViewBag.municipio_direccion_fiscal = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razones_Sociales.municipio_direccion_fiscal);
            ViewBag.municipio_direccion_fisica = new SelectList(db.Municipios.Where(x => x.eliminado == false), "id_municipio", "nombre", razones_Sociales.municipio_direccion_fisica);
            return View(razones_Sociales);
        }

        // GET: Administracion/Razones_Sociales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Razones_Sociales razones_Sociales = db.Razones_Sociales.Find(id);
            if (razones_Sociales == null)
            {
                return HttpNotFound();
            }
            return View(razones_Sociales);
        }

        // POST: Administracion/Razones_Sociales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Razones_Sociales razones_Sociales = db.Razones_Sociales.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            razones_Sociales.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            razones_Sociales.fecha_eliminacion = DateTime.Now;
            razones_Sociales.eliminado = true;
            razones_Sociales.activo = true;
            db.Entry(razones_Sociales).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult crear_razon_social_contacto([Bind(Include = "id_razon_social,id_contacto")] Razon_Social_Contacto razon_social_contacto)
        {
            if (ModelState.IsValid)
            {
                razon_social_contacto.activo = true;
                razon_social_contacto.eliminado = false;
                razon_social_contacto.fecha_creacion = DateTime.Now;
                razon_social_contacto.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Razon_Social_Contacto.Add(razon_social_contacto);
                db.SaveChanges();
            }

            return RedirectToAction("Edit", new { id = razon_social_contacto.id_razon_social });
        }

        //[ValidateAntiForgeryToken]
        public ActionResult eliminar_razon_social_contacto(int id, int id_redirect)
        {
            if (ModelState.IsValid)
            {
                Razon_Social_Contacto razon_social_contacto = db.Razon_Social_Contacto.Find(id);
                razon_social_contacto.activo = false;
                razon_social_contacto.eliminado = true;
                razon_social_contacto.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                razon_social_contacto.fecha_eliminacion = DateTime.Now;
                db.Entry(razon_social_contacto).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Edit", new { id = id_redirect });
        }

        // GET: Administracion/Contactos/Create
        public ActionResult Create_contacto(int id)
        {
            Razones_Sociales razones_Sociales = db.Razones_Sociales.Find(id);
            ViewBag.cliente = db.Clientes.Where(y => y.id_cliente == razones_Sociales.id_cliente).Select(c => c.nombre).First();
            ViewBag.cliente = db.Clientes.Find(razones_Sociales.id_cliente).nombre;
            ViewBag.razon_social = razones_Sociales.razon_social;
            ViewBag.id_razon_social = new SelectList(db.Razones_Sociales.Where(x => x.id_razon_social == id), "id_razon_social", "razon_social");
            ViewBag.id_contacto_puesto = new SelectList(db.Contacto_Puesto.Where(x => x.eliminado == false), "id_contacto_puesto", "descripcion");
            return View();
        }

        // POST: Administracion/Contactos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_contacto(int id, [Bind(Include = "id_contacto,nombre,id_contacto_puesto,telefono,email,comentario,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Contactos contactos)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                contactos.eliminado = false;
                contactos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                contactos.fecha_creacion = DateTime.Now;
                db.Contactos.Add(contactos);
                db.SaveChanges();

                Razon_Social_Contacto rsc = new Razon_Social_Contacto();
                rsc.id_contacto = contactos.id_contacto;
                rsc.id_razon_social = id;
                rsc.activo = true;
                rsc.eliminado = false;
                rsc.fecha_creacion = DateTime.Now;
                rsc.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Razon_Social_Contacto.Add(rsc);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = id });
            }
            ViewBag.id_contacto_puesto = new SelectList(db.Contacto_Puesto.Where(x => x.eliminado == false), "id_contacto_puesto", "descripcion", contactos.id_contacto_puesto);
            return View(contactos);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
