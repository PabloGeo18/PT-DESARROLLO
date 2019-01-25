using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Areas.Administracion.Controllers
{
    public class AccionesController : Controller
    {
        private AppEntities db = new AppEntities(Constantes.getDataSource());

        // GET: Administracion/Acciones
        public ActionResult Index()
        {
            var acciones = db.Acciones.Include(a => a.Controladores);
            return View(acciones.ToList());
        }

        // GET: Administracion/Acciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acciones acciones = db.Acciones.Find(id);
            if (acciones == null)
            {
                return HttpNotFound();
            }
            return View(acciones);
        }

        // GET: Administracion/Acciones/Create
        public ActionResult Create()
        {
            ViewBag.id_controlador = new SelectList(db.Controladores, "id_controlador", "nombre");
            return View();
        }

        // POST: Administracion/Acciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_accion,nombre,id_controlador")] Acciones acciones)
        {
            if (ModelState.IsValid)
            {
                db.Acciones.Add(acciones);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_controlador = new SelectList(db.Controladores, "id_controlador", "nombre", acciones.id_controlador);
            return View(acciones);
        }

        // GET: Administracion/Acciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acciones acciones = db.Acciones.Find(id);
            if (acciones == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_controlador = new SelectList(db.Controladores, "id_controlador", "nombre", acciones.id_controlador);
            return View(acciones);
        }

        // POST: Administracion/Acciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_accion,nombre,id_controlador")] Acciones acciones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(acciones).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_controlador = new SelectList(db.Controladores, "id_controlador", "nombre", acciones.id_controlador);
            return View(acciones);
        }

        // GET: Administracion/Acciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Acciones acciones = db.Acciones.Find(id);
            if (acciones == null)
            {
                return HttpNotFound();
            }
            return View(acciones);
        }

        // POST: Administracion/Acciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Acciones acciones = db.Acciones.Find(id);
            db.Acciones.Remove(acciones);
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
