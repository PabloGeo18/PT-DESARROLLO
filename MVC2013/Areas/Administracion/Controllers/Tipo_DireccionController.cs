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
    public class Tipo_DireccionController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Tipo_Direccion
        public ActionResult Index()
        {
            return View(db.Tipo_Direccion.Where(t=>!t.eliminado).OrderBy(t=>t.nombre).ToList());
        }

        // GET: Administracion/Tipo_Direccion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Direccion tipo_Direccion = db.Tipo_Direccion.Find(id);
            if (tipo_Direccion == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Direccion);
        }

        // GET: Administracion/Tipo_Direccion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Tipo_Direccion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_tipo_direccion,nombre")] Tipo_Direccion tipo_Direccion)
        {
            if (ModelState.IsValid)
            {
                tipo_Direccion.activo = true;
                tipo_Direccion.eliminado = false;
                tipo_Direccion.fecha_creacion = DateTime.Now;
                tipo_Direccion.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Tipo_Direccion.Add(tipo_Direccion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipo_Direccion);
        }

        // GET: Administracion/Tipo_Direccion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Direccion tipo_Direccion = db.Tipo_Direccion.Find(id);
            if (tipo_Direccion == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Direccion);
        }

        // POST: Administracion/Tipo_Direccion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_tipo_direccion,nombre")] Tipo_Direccion tipo_Direccion)
        {
            if (ModelState.IsValid)
            {
                Tipo_Direccion edittipo_direccion = db.Tipo_Direccion.Find(tipo_Direccion.id_tipo_direccion);
                edittipo_direccion.nombre = tipo_Direccion.nombre;
                edittipo_direccion.fecha_modificacion = DateTime.Now;
                edittipo_direccion.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Entry(edittipo_direccion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipo_Direccion);
        }

        // GET: Administracion/Tipo_Direccion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Direccion tipo_Direccion = db.Tipo_Direccion.Find(id);
            if (tipo_Direccion == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Direccion);
        }

        // POST: Administracion/Tipo_Direccion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tipo_Direccion tipo_Direccion = db.Tipo_Direccion.Find(id);
            tipo_Direccion.activo = false;
            tipo_Direccion.eliminado = true;
            tipo_Direccion.fecha_eliminacion = DateTime.Now;
            tipo_Direccion.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            db.Entry(tipo_Direccion).State = EntityState.Modified;
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
