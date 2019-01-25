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
    public class ProfesionController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Profesions
        public ActionResult Index()
        {
            return View(db.Profesion.Where(p=>!p.eliminado).OrderBy(p=>p.nombre).ToList());
        }

        // GET: Administracion/Profesions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profesion profesion = db.Profesion.Find(id);
            if (profesion == null)
            {
                return HttpNotFound();
            }
            return View(profesion);
        }

        // GET: Administracion/Profesions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Profesions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_profesion,nombre")] Profesion profesion)
        {
            if (ModelState.IsValid)
            {
                profesion.activo = true;
                profesion.eliminado = false;
                profesion.fecha_creacion = DateTime.Now;
                profesion.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Profesion.Add(profesion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(profesion);
        }

        // GET: Administracion/Profesions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profesion profesion = db.Profesion.Find(id);
            if (profesion == null)
            {
                return HttpNotFound();
            }
            return View(profesion);
        }

        // POST: Administracion/Profesions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_profesion,nombre")] Profesion profesion)
        {
            if (ModelState.IsValid)
            {
                Profesion editprofesion = db.Profesion.Find(profesion.id_profesion);
                editprofesion.nombre = profesion.nombre;
                editprofesion.fecha_modificacion = DateTime.Now;
                editprofesion.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Entry(editprofesion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(profesion);
        }

        // GET: Administracion/Profesions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profesion profesion = db.Profesion.Find(id);
            if (profesion == null)
            {
                return HttpNotFound();
            }
            return View(profesion);
        }

        // POST: Administracion/Profesions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Profesion profesion = db.Profesion.Find(id);
            profesion.activo = false;
            profesion.eliminado = true;
            profesion.fecha_eliminacion = DateTime.Now;
            profesion.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            db.Entry(profesion).State = EntityState.Modified;
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
