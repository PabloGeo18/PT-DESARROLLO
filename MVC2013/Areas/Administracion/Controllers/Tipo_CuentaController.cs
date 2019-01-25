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
    public class Tipo_CuentaController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Tipo_Cuenta
        public ActionResult Index()
        {
            return View(db.Tipo_Cuenta.Where(t=>!t.eliminado).OrderBy(t=>t.nombre).ToList());
        }

        // GET: Administracion/Tipo_Cuenta/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Cuenta tipo_Cuenta = db.Tipo_Cuenta.Find(id);
            if (tipo_Cuenta == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Cuenta);
        }

        // GET: Administracion/Tipo_Cuenta/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Tipo_Cuenta/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_tipo_cuenta,nombre")] Tipo_Cuenta tipo_Cuenta)
        {
            if (ModelState.IsValid)
            {
                tipo_Cuenta.activo = true;
                tipo_Cuenta.eliminado = false;
                tipo_Cuenta.fecha_creacion = DateTime.Now;
                tipo_Cuenta.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Tipo_Cuenta.Add(tipo_Cuenta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipo_Cuenta);
        }

        // GET: Administracion/Tipo_Cuenta/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Cuenta tipo_Cuenta = db.Tipo_Cuenta.Find(id);
            if (tipo_Cuenta == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Cuenta);
        }

        // POST: Administracion/Tipo_Cuenta/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_tipo_cuenta,nombre")] Tipo_Cuenta tipo_Cuenta)
        {
            if (ModelState.IsValid)
            {
                Tipo_Direccion edit_tipo_cuenta = db.Tipo_Direccion.Find(tipo_Cuenta.id_tipo_cuenta);
                edit_tipo_cuenta.nombre = tipo_Cuenta.nombre;
                edit_tipo_cuenta.fecha_modificacion = DateTime.Now;
                edit_tipo_cuenta.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Entry(edit_tipo_cuenta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipo_Cuenta);
        }

        // GET: Administracion/Tipo_Cuenta/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Cuenta tipo_Cuenta = db.Tipo_Cuenta.Find(id);
            if (tipo_Cuenta == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Cuenta);
        }

        // POST: Administracion/Tipo_Cuenta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tipo_Cuenta tipo_Cuenta = db.Tipo_Cuenta.Find(id);
            tipo_Cuenta.activo = false;
            tipo_Cuenta.eliminado = true;
            tipo_Cuenta.fecha_eliminacion = DateTime.Now;
            tipo_Cuenta.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            db.Entry(tipo_Cuenta).State = EntityState.Modified;
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
