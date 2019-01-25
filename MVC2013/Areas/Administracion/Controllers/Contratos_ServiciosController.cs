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
    public class Contratos_ServiciosController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Contratos_Servicios
        public ActionResult Index()
        {
            var contratos_Servicios = db.Contratos_Servicios.Include(c => c.Ubicaciones).Include(c => c.Usuarios).Include(c => c.Usuarios1).Include(c => c.Usuarios2).Include(c => c.Cat_Estados_Servicio_Contratado).Include(c => c.Cat_Tipos_Facturacion);
            return View(contratos_Servicios.ToList());
        }

        // GET: Administracion/Contratos_Servicios/Details/5
        public ActionResult Details(int? id)
        {
            Contratos_Servicios contratos_Servicios = db.Contratos_Servicios.Find(id);
            if (contratos_Servicios == null)
            {
                return HttpNotFound();
            }
            
            return View(contratos_Servicios);
        }

        // GET: Administracion/Contratos_Servicios/Create
        public ActionResult Create()
        {
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones, "id_ubicacion", "direccion");
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_cat_estado_servicio_contratado = new SelectList(db.Cat_Estados_Servicio_Contratado, "id_cat_estado_servicio_contratado", "nombre");
            ViewBag.id_cat_tipo_facturacion = new SelectList(db.Cat_Tipos_Facturacion, "id_cat_tipo_facturacion", "nombre");
            return View();
        }

        // POST: Administracion/Contratos_Servicios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_contrato_servicio,id_ubicacion,id_cat_tipo_facturacion,id_cat_estado_servicio_contratado,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Contratos_Servicios contratos_Servicios)
        {
            if (ModelState.IsValid)
            {
                db.Contratos_Servicios.Add(contratos_Servicios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones, "id_ubicacion", "direccion", contratos_Servicios.id_ubicacion);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_modificacion);
            ViewBag.id_cat_estado_servicio_contratado = new SelectList(db.Cat_Estados_Servicio_Contratado, "id_cat_estado_servicio_contratado", "nombre", contratos_Servicios.id_cat_estado_servicio_contratado);
            ViewBag.id_cat_tipo_facturacion = new SelectList(db.Cat_Tipos_Facturacion, "id_cat_tipo_facturacion", "nombre", contratos_Servicios.id_cat_tipo_facturacion);
            return View(contratos_Servicios);
        }

        // GET: Administracion/Contratos_Servicios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratos_Servicios contratos_Servicios = db.Contratos_Servicios.Find(id);
            if (contratos_Servicios == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones, "id_ubicacion", "direccion", contratos_Servicios.id_ubicacion);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_modificacion);
            ViewBag.id_cat_estado_servicio_contratado = new SelectList(db.Cat_Estados_Servicio_Contratado, "id_cat_estado_servicio_contratado", "nombre", contratos_Servicios.id_cat_estado_servicio_contratado);
            ViewBag.id_cat_tipo_facturacion = new SelectList(db.Cat_Tipos_Facturacion, "id_cat_tipo_facturacion", "nombre", contratos_Servicios.id_cat_tipo_facturacion);
            return View(contratos_Servicios);
        }

        // POST: Administracion/Contratos_Servicios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_contrato_servicio,id_ubicacion,id_cat_tipo_facturacion,id_cat_estado_servicio_contratado,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Contratos_Servicios contratos_Servicios)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contratos_Servicios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones, "id_ubicacion", "direccion", contratos_Servicios.id_ubicacion);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Servicios.id_usuario_modificacion);
            ViewBag.id_cat_estado_servicio_contratado = new SelectList(db.Cat_Estados_Servicio_Contratado, "id_cat_estado_servicio_contratado", "nombre", contratos_Servicios.id_cat_estado_servicio_contratado);
            ViewBag.id_cat_tipo_facturacion = new SelectList(db.Cat_Tipos_Facturacion, "id_cat_tipo_facturacion", "nombre", contratos_Servicios.id_cat_tipo_facturacion);
            return View(contratos_Servicios);
        }

        // GET: Administracion/Contratos_Servicios/Delete/5
        public ActionResult Delete(int id)
        {
            Contratos_Servicios contratos_Servicios = db.Contratos_Servicios.Find(id);
            if (contratos_Servicios == null)
            {
                return HttpNotFound();
            }
            return View(contratos_Servicios);
        }

        // POST: Administracion/Contratos_Servicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Contratos_Servicios contratos_Servicios = db.Contratos_Servicios.Find(id);
            contratos_Servicios.fecha_modificacion = DateTime.Now;
            contratos_Servicios.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
            contratos_Servicios.id_cat_estado_servicio_contratado = (int)Catalogos.Estado_Contrato_Servicio.Cancelado;
            db.Entry(contratos_Servicios).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details_U", "Clientes", new { id_ubicacion = contratos_Servicios.id_ubicacion});
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
