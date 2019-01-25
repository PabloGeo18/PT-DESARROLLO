using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;

namespace MVC2013.Areas.Customers.Controllers
{
    public class Procesos_FacturacionController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Customers/Procesos_Facturacion
        public ActionResult Index()
        {
            var procesos_Facturacion = db.Procesos_Facturacion.OrderByDescending(x => x.fecha_proceso);
            return View(procesos_Facturacion.ToList());
        }

        // GET: Customers/Procesos_Facturacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Procesos_Facturacion procesos_Facturacion = db.Procesos_Facturacion.Find(id);
            if (procesos_Facturacion == null)
            {
                return HttpNotFound();
            }
            return View(procesos_Facturacion);
        }

        // GET: Customers/Procesos_Facturacion/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Customers/Procesos_Facturacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_proceso_facturacion,fecha_proceso,cantidad_detalles,total_facturar,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Procesos_Facturacion procesos_Facturacion)
        {
            if (ModelState.IsValid)
            {
                db.Procesos_Facturacion.Add(procesos_Facturacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", procesos_Facturacion.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", procesos_Facturacion.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", procesos_Facturacion.id_usuario_modificacion);
            return View(procesos_Facturacion);
        }

        // GET: Customers/Procesos_Facturacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Procesos_Facturacion procesos_Facturacion = db.Procesos_Facturacion.Find(id);
            if (procesos_Facturacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", procesos_Facturacion.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", procesos_Facturacion.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", procesos_Facturacion.id_usuario_modificacion);
            return View(procesos_Facturacion);
        }

        // POST: Customers/Procesos_Facturacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_proceso_facturacion,fecha_proceso,cantidad_detalles,total_facturar,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Procesos_Facturacion procesos_Facturacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(procesos_Facturacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", procesos_Facturacion.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", procesos_Facturacion.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", procesos_Facturacion.id_usuario_modificacion);
            return View(procesos_Facturacion);
        }

        // GET: Customers/Procesos_Facturacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Procesos_Facturacion procesos_Facturacion = db.Procesos_Facturacion.Find(id);
            if (procesos_Facturacion == null)
            {
                return HttpNotFound();
            }
            return View(procesos_Facturacion);
        }

        // POST: Customers/Procesos_Facturacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Procesos_Facturacion procesos_Facturacion = db.Procesos_Facturacion.Find(id);
            db.Procesos_Facturacion.Remove(procesos_Facturacion);
            db.SaveChanges();
            return RedirectToAction("Index");
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
