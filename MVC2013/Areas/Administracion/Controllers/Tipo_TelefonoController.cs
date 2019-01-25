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
    public class Tipo_TelefonoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Tipo_Telefono
        public ActionResult Index()
        {
            return View(db.Tipo_Telefono.Where(t=>!t.eliminado).OrderBy(t=>t.nombre).ToList());
        }

        // GET: Administracion/Tipo_Telefono/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Telefono tipo_Telefono = db.Tipo_Telefono.Find(id);
            if (tipo_Telefono == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Telefono);
        }

        // GET: Administracion/Tipo_Telefono/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Tipo_Telefono/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_tipo_telefono,nombre")] Tipo_Telefono tipo_Telefono)
        {
            if (ModelState.IsValid)
            {
                tipo_Telefono.activo = true;
                tipo_Telefono.eliminado = false;
                tipo_Telefono.fecha_creacion = DateTime.Now;
                tipo_Telefono.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Tipo_Telefono.Add(tipo_Telefono);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipo_Telefono);
        }

        // GET: Administracion/Tipo_Telefono/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Telefono tipo_Telefono = db.Tipo_Telefono.Find(id);
            if (tipo_Telefono == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Telefono);
        }

        // POST: Administracion/Tipo_Telefono/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_tipo_telefono,nombre")] Tipo_Telefono tipo_Telefono)
        {
            if (ModelState.IsValid)
            {
                Tipo_Telefono edittipo_telefono = db.Tipo_Telefono.Find(tipo_Telefono.id_tipo_telefono);
                edittipo_telefono.fecha_modificacion = DateTime.Now;
                edittipo_telefono.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                edittipo_telefono.nombre = tipo_Telefono.nombre;
                db.Entry(edittipo_telefono).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipo_Telefono);
        }

        // GET: Administracion/Tipo_Telefono/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Telefono tipo_Telefono = db.Tipo_Telefono.Find(id);
            if (tipo_Telefono == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Telefono);
        }

        // POST: Administracion/Tipo_Telefono/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tipo_Telefono tipo_Telefono = db.Tipo_Telefono.Find(id);
            tipo_Telefono.eliminado = true;
            tipo_Telefono.activo = false;
            tipo_Telefono.fecha_eliminacion = DateTime.Now;
            tipo_Telefono.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            db.Entry(tipo_Telefono).State = EntityState.Modified;
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
