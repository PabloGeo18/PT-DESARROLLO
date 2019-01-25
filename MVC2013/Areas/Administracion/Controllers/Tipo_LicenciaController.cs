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
    public class Tipo_LicenciaController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Tipo_Licencia
        public ActionResult Index()
        {
            return View(db.Tipo_Licencia.Where(t=>!t.eliminado).OrderBy(t=>t.nombre).ToList());
        }

        // GET: Administracion/Tipo_Licencia/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Licencia tipo_Licencia = db.Tipo_Licencia.Find(id);
            if (tipo_Licencia == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Licencia);
        }

        // GET: Administracion/Tipo_Licencia/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Tipo_Licencia/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_tipo_licencia,nombre")] Tipo_Licencia tipo_Licencia)
        {
            if (ModelState.IsValid)
            {
                tipo_Licencia.activo = true;
                tipo_Licencia.eliminado = false;
                tipo_Licencia.fecha_creacion = DateTime.Now;
                tipo_Licencia.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Tipo_Licencia.Add(tipo_Licencia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipo_Licencia);
        }

        // GET: Administracion/Tipo_Licencia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Licencia tipo_Licencia = db.Tipo_Licencia.Find(id);
            if (tipo_Licencia == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Licencia);
        }

        // POST: Administracion/Tipo_Licencia/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_tipo_licencia,nombre")] Tipo_Licencia tipo_Licencia)
        {
            if (ModelState.IsValid)
            {
                Tipo_Licencia edittipo_licencia = db.Tipo_Licencia.Find(tipo_Licencia.id_tipo_licencia);
                edittipo_licencia.nombre = tipo_Licencia.nombre;
                edittipo_licencia.fecha_modificacion = DateTime.Now;
                edittipo_licencia.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Entry(edittipo_licencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipo_Licencia);
        }

        // GET: Administracion/Tipo_Licencia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Licencia tipo_Licencia = db.Tipo_Licencia.Find(id);
            if (tipo_Licencia == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Licencia);
        }

        // POST: Administracion/Tipo_Licencia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tipo_Licencia tipo_Licencia = db.Tipo_Licencia.Find(id);
            tipo_Licencia.activo = false;
            tipo_Licencia.eliminado = true;
            tipo_Licencia.fecha_eliminacion = DateTime.Now;
            tipo_Licencia.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            db.Entry(tipo_Licencia).State = EntityState.Modified;
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
