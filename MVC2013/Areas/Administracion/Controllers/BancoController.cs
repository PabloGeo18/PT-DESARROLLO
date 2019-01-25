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
    public class BancoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Bancoes
        public ActionResult Index()
        {
            return View(db.Banco.Where(b=>!b.eliminado).OrderBy(b=>b.nombre).ToList());
        }

        // GET: Administracion/Bancoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banco banco = db.Banco.Find(id);
            if (banco == null)
            {
                return HttpNotFound();
            }
            return View(banco);
        }

        // GET: Administracion/Bancoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Bancoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_banco,nombre")] Banco banco)
        {
            if (ModelState.IsValid)
            {
                banco.activo = true; banco.eliminado = false;
                banco.fecha_creacion = DateTime.Now;
                banco.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Banco.Add(banco);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(banco);
        }

        // GET: Administracion/Bancoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banco banco = db.Banco.Find(id);
            if (banco == null)
            {
                return HttpNotFound();
            }
            return View(banco);
        }

        // POST: Administracion/Bancoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_banco,nombre")] Banco banco)
        {
            if (ModelState.IsValid)
            {
                Banco editbanco = db.Banco.Find(banco.id_banco);
                editbanco.nombre = banco.nombre;
                editbanco.fecha_modificacion = DateTime.Now;
                editbanco.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Entry(editbanco).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(banco);
        }

        // GET: Administracion/Bancoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banco banco = db.Banco.Find(id);
            if (banco == null)
            {
                return HttpNotFound();
            }
            return View(banco);
        }

        // POST: Administracion/Bancoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Banco banco = db.Banco.Find(id);
            banco.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            banco.fecha_eliminacion = DateTime.Now;
            banco.activo = false;
            banco.eliminado = true;
            db.Entry(banco).State = EntityState.Modified;
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
