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

namespace MVC2013.Areas.Customers.Controllers
{
    public class Contratos_Otros_ServiciosController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Contratos_Otros_Servicios
        public ActionResult Index()
        {
            var contratos_Otros_Servicios = db.Contratos_Otros_Servicios.Include(c => c.Usuarios).Include(c => c.Usuarios1).Include(c => c.Usuarios2).Include(c => c.Cat_Otros_Servicios).Include(c => c.Contratos_Servicios);
            return View(contratos_Otros_Servicios.ToList());
        }

        // GET: Administracion/Contratos_Otros_Servicios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratos_Otros_Servicios contratos_Otros_Servicios = db.Contratos_Otros_Servicios.Find(id);
            if (contratos_Otros_Servicios == null)
            {
                return HttpNotFound();
            }
            return View(contratos_Otros_Servicios);
        }

        // GET: Administracion/Contratos_Otros_Servicios/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_cat_otro_servicio = new SelectList(db.Cat_Otros_Servicios, "id_cat_otro_servicio", "nombre");
            ViewBag.id_contrato_servicio = new SelectList(db.Contratos_Servicios, "id_contrato_servicio", "id_contrato_servicio");
            return View();
        }

        // POST: Administracion/Contratos_Otros_Servicios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_contrato_otro_servicio,id_contrato_servicio,id_cat_otro_servicio,cantidad,costo,precio_venta_unitario,fecha_inicio,fecha_fin,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Contratos_Otros_Servicios contratos_Otros_Servicios)
        {
            if (ModelState.IsValid)
            {
                db.Contratos_Otros_Servicios.Add(contratos_Otros_Servicios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Otros_Servicios.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Otros_Servicios.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", contratos_Otros_Servicios.id_usuario_modificacion);
            ViewBag.id_cat_otro_servicio = new SelectList(db.Cat_Otros_Servicios, "id_cat_otro_servicio", "nombre", contratos_Otros_Servicios.id_cat_otro_servicio);
            ViewBag.id_contrato_servicio = new SelectList(db.Contratos_Servicios, "id_contrato_servicio", "id_contrato_servicio", contratos_Otros_Servicios.id_contrato_servicio);
            return View(contratos_Otros_Servicios);
        }

        // GET: Administracion/Contratos_Otros_Servicios/Edit/5
        public ActionResult Edit(int id)
        {
            Contratos_Otros_Servicios contratos_Otros_Servicios = db.Contratos_Otros_Servicios.Where(x => x.activo && !x.eliminado && x.id_contrato_otro_servicio == id).Take(1).FirstOrDefault();
            if (contratos_Otros_Servicios == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cat_otro_servicio = new SelectList(db.Cat_Otros_Servicios.Where(x => x.activo && !x.eliminado), "id_cat_otro_servicio", "nombre", contratos_Otros_Servicios.id_cat_otro_servicio);
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(x => x.activo && !x.eliminado && x.id_razon_social == contratos_Otros_Servicios.id_razon_social).Select(s => new { id_ubicacion = s.id_ubicacion, nombre = s.id_ubicacion + ". " + s.nombre }), "id_ubicacion", "nombre", contratos_Otros_Servicios.id_ubicacion);
            ViewBag.id_razon_social_grupo_factura = new SelectList(db.Razon_Social_Grupos_Factura.Where(x => x.activo && !x.eliminado && x.id_razon_social == contratos_Otros_Servicios.id_razon_social).Select(s => new { id_razon_social_grupo_factura = s.id_razon_social_grupo_factura, nombre = s.correlativo + ". " + s.nombre }), "id_razon_social_grupo_factura", "nombre", contratos_Otros_Servicios.id_razon_social_grupo_factura);
            ViewBag.fecha_inicio = contratos_Otros_Servicios.fecha_inicio;
            ViewBag.fecha_fin = contratos_Otros_Servicios.fecha_fin;
            ViewBag.id_contrato_agente = id;
            return View(contratos_Otros_Servicios);
        }

        // POST: Administracion/Contratos_Otros_Servicios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Contratos_Otros_Servicios contratos_Otros_Servicios)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                Contratos_Otros_Servicios Temp_contrato_otro_servicio = db.Contratos_Otros_Servicios.Find(contratos_Otros_Servicios.id_contrato_otro_servicio);
                Temp_contrato_otro_servicio.cantidad = contratos_Otros_Servicios.cantidad;
                Temp_contrato_otro_servicio.costo = contratos_Otros_Servicios.costo;
                Temp_contrato_otro_servicio.precio_venta_unitario = contratos_Otros_Servicios.precio_venta_unitario;
                Temp_contrato_otro_servicio.id_cat_otro_servicio = contratos_Otros_Servicios.id_cat_otro_servicio;
                Temp_contrato_otro_servicio.fecha_inicio = contratos_Otros_Servicios.fecha_inicio;
                Temp_contrato_otro_servicio.fecha_fin = contratos_Otros_Servicios.fecha_fin;
                Temp_contrato_otro_servicio.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                Temp_contrato_otro_servicio.fecha_modificacion = DateTime.Now;
                Temp_contrato_otro_servicio.id_razon_social_grupo_factura = contratos_Otros_Servicios.id_razon_social_grupo_factura;
                Temp_contrato_otro_servicio.id_ubicacion = contratos_Otros_Servicios.id_ubicacion;
                db.Entry(Temp_contrato_otro_servicio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Contratos_Servicios", new { id = Temp_contrato_otro_servicio.id_contrato_servicio });
            }
            return View(contratos_Otros_Servicios);
        }

        // GET: Administracion/Contratos_Otros_Servicios/Edit/5
        public ActionResult Edit_Servicio(int id)
        {
            Contratos_Otros_Servicios contratos_Otros_Servicios = db.Contratos_Otros_Servicios.Where(x => x.activo && !x.eliminado && x.id_contrato_otro_servicio == id).Take(1).FirstOrDefault();
            if (contratos_Otros_Servicios == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cat_otro_servicio = new SelectList(db.Cat_Otros_Servicios.Where(x => x.activo && !x.eliminado), "id_cat_otro_servicio", "nombre", contratos_Otros_Servicios.id_cat_otro_servicio);
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(x => x.activo && !x.eliminado && x.id_razon_social == contratos_Otros_Servicios.id_razon_social).Select(s => new { id_ubicacion = s.id_ubicacion, nombre = s.id_ubicacion + ". " + s.nombre }), "id_ubicacion", "nombre", contratos_Otros_Servicios.id_ubicacion);
            ViewBag.id_razon_social_grupo_factura = new SelectList(db.Razon_Social_Grupos_Factura.Where(x => x.activo && !x.eliminado && x.id_razon_social == contratos_Otros_Servicios.id_razon_social).Select(s => new { id_razon_social_grupo_factura = s.id_razon_social_grupo_factura, nombre = s.correlativo + ". " + s.nombre }), "id_razon_social_grupo_factura", "nombre", contratos_Otros_Servicios.id_razon_social_grupo_factura);
            ViewBag.fecha_inicio = contratos_Otros_Servicios.fecha_inicio;
            ViewBag.fecha_fin = contratos_Otros_Servicios.fecha_fin;
            ViewBag.id_contrato_agente = id;
            return View(contratos_Otros_Servicios);
        }

        // POST: Administracion/Contratos_Otros_Servicios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Servicio(Contratos_Otros_Servicios contratos_Otros_Servicios)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                Contratos_Otros_Servicios Temp_contrato_otro_servicio = db.Contratos_Otros_Servicios.Find(contratos_Otros_Servicios.id_contrato_otro_servicio);
                Temp_contrato_otro_servicio.cantidad = contratos_Otros_Servicios.cantidad;
                Temp_contrato_otro_servicio.costo = contratos_Otros_Servicios.costo;
                Temp_contrato_otro_servicio.precio_venta_unitario = contratos_Otros_Servicios.precio_venta_unitario;
                Temp_contrato_otro_servicio.id_cat_otro_servicio = contratos_Otros_Servicios.id_cat_otro_servicio;
                Temp_contrato_otro_servicio.fecha_inicio = contratos_Otros_Servicios.fecha_inicio;
                Temp_contrato_otro_servicio.fecha_fin = contratos_Otros_Servicios.fecha_fin;
                Temp_contrato_otro_servicio.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                Temp_contrato_otro_servicio.fecha_modificacion = DateTime.Now;
                Temp_contrato_otro_servicio.id_razon_social_grupo_factura = contratos_Otros_Servicios.id_razon_social_grupo_factura;
                Temp_contrato_otro_servicio.id_ubicacion = contratos_Otros_Servicios.id_ubicacion;
                db.Entry(Temp_contrato_otro_servicio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details_RS", "Clientes", new { id_Razon_Social = Temp_contrato_otro_servicio.id_razon_social });
            }
            return View(contratos_Otros_Servicios);
        }

        // GET: Administracion/Contratos_Otros_Servicios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratos_Otros_Servicios contratos_Otros_Servicios = db.Contratos_Otros_Servicios.Where(x => x.activo && !x.eliminado && x.id_contrato_otro_servicio == id).Take(1).FirstOrDefault();
            if (contratos_Otros_Servicios == null)
            {
                return HttpNotFound();
            }
            return View(contratos_Otros_Servicios);
        }

        // POST: Administracion/Contratos_Otros_Servicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, String Comentario)
        {
            Contratos_Otros_Servicios contratos_Otros_Servicios = db.Contratos_Otros_Servicios.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            contratos_Otros_Servicios.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            contratos_Otros_Servicios.descripcion_eliminacion = Comentario;
            contratos_Otros_Servicios.eliminado = true;
            contratos_Otros_Servicios.activo = false;
            contratos_Otros_Servicios.fecha_eliminacion = DateTime.Now;
            db.Entry(contratos_Otros_Servicios).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", "Contratos_Servicios", new { id = contratos_Otros_Servicios.id_contrato_servicio });
        }

        // GET: Administracion/Contratos_Otros_Servicios/Delete/5
        public ActionResult Delete_Servicio(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Contratos_Otros_Servicios contratos_Otros_Servicios = db.Contratos_Otros_Servicios.Find(id);
            Contratos_Otros_Servicios contratos_Otros_Servicios = db.Contratos_Otros_Servicios.Where( x => x.activo && !x.eliminado && x.id_contrato_otro_servicio == id).Take(1).FirstOrDefault();
            if (contratos_Otros_Servicios == null)
            {
                return HttpNotFound();
            }
            return View(contratos_Otros_Servicios);
        }

        // POST: Administracion/Contratos_Otros_Servicios/Delete/5
        [HttpPost, ActionName("Delete_Servicio")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed_RS(int id, String Comentario)
        {
            Contratos_Otros_Servicios contratos_Otros_Servicios = db.Contratos_Otros_Servicios.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            contratos_Otros_Servicios.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            contratos_Otros_Servicios.descripcion_eliminacion = Comentario;
            contratos_Otros_Servicios.eliminado = true;
            contratos_Otros_Servicios.activo = false;
            contratos_Otros_Servicios.fecha_eliminacion = DateTime.Now;
            db.Entry(contratos_Otros_Servicios).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details_RS", "Clientes", new { id_Razon_Social = contratos_Otros_Servicios.id_razon_social });
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
