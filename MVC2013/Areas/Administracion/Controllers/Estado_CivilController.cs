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
    public class Estado_CivilController : Controller
    {
        private AppEntities db = new AppEntities();


        // GET: Administracion/Estado_Civil
        public ActionResult Index()
        {   
            return View(db.Estado_Civil.Where(e=>!e.eliminado).OrderBy(e=>e.nombre).ToList());
        }

        // GET: Administracion/Estado_Civil/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado_Civil estado_Civil = db.Estado_Civil.Find(id);
            if (estado_Civil == null)
            {
                return HttpNotFound();
            }
            return View(estado_Civil);
        }

        // GET: Administracion/Estado_Civil/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Estado_Civil/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_estado_civil,nombre")] Estado_Civil estado_Civil)
        {
            if (ModelState.IsValid)
            {
                estado_Civil.fecha_creacion = DateTime.Now;
                estado_Civil.activo = true;
                estado_Civil.eliminado = false;
                estado_Civil.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Estado_Civil.Add(estado_Civil);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(estado_Civil);
        }

        // GET: Administracion/Estado_Civil/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado_Civil estado_Civil = db.Estado_Civil.Find(id);
            if (estado_Civil == null)
            {
                return HttpNotFound();
            }
            return View(estado_Civil);
        }

        // POST: Administracion/Estado_Civil/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_estado_civil,nombre")] Estado_Civil estado_Civil)
        {
            if (ModelState.IsValid)
            {
                Estado_Civil editestado_civil = db.Estado_Civil.Find(estado_Civil.id_estado_civil);
                editestado_civil.nombre = estado_Civil.nombre;
                editestado_civil.fecha_modificacion = DateTime.Now;
                editestado_civil.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Entry(editestado_civil).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(estado_Civil);
        }

        // GET: Administracion/Estado_Civil/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado_Civil estado_Civil = db.Estado_Civil.Find(id);
            if (estado_Civil == null)
            {
                return HttpNotFound();
            }
            return View(estado_Civil);
        }

        // POST: Administracion/Estado_Civil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Estado_Civil estado_Civil = db.Estado_Civil.Find(id);
            estado_Civil.eliminado = true;
            estado_Civil.activo = false;
            estado_Civil.fecha_eliminacion = DateTime.Now;
            estado_Civil.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            db.Entry(estado_Civil).State = EntityState.Modified;
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
