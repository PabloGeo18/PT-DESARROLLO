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

namespace MVC2013.Areas.rrhh.Controllers
{
    public class Motivo_BajaController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Motivo_Baja
        public ActionResult Index()
        {
            var motivo_Baja = db.Motivo_Baja.Where(m => m.activo);
            return View(motivo_Baja);
        }

        // GET: rrhh/Motivo_Baja/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Motivo_Baja motivo_Baja = db.Motivo_Baja.Find(id);
            if (motivo_Baja == null)
            {
                return HttpNotFound();
            }
            return View(motivo_Baja);
        }

        // GET: rrhh/Motivo_Baja/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: rrhh/Motivo_Baja/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_motivo_baja,descripcion,activo,eliminado,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion")] Motivo_Baja motivo_Baja)
        {
            if (ModelState.IsValid)
            {
                motivo_Baja.activo = true;
                motivo_Baja.eliminado = false;
                motivo_Baja.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                motivo_Baja.fecha_creacion = DateTime.Now;
                db.Motivo_Baja.Add(motivo_Baja);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(motivo_Baja);
        }

        // GET: rrhh/Motivo_Baja/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Motivo_Baja motivo_Baja = db.Motivo_Baja.Find(id);
            if (motivo_Baja == null)
            {
                return HttpNotFound();
            }
            return View(motivo_Baja);
        }

        // POST: rrhh/Motivo_Baja/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_motivo_baja,descripcion,activo,eliminado,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion")] Motivo_Baja motivo_Baja)
        {
            Motivo_Baja edit = db.Motivo_Baja.SingleOrDefault(m => m.id_motivo_baja == motivo_Baja.id_motivo_baja && m.activo);
            if (edit == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {

                motivo_Baja.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                motivo_Baja.fecha_modificacion = DateTime.Now;
                edit.descripcion = motivo_Baja.descripcion;
                db.Entry(edit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(edit);
        }

        // GET: rrhh/Motivo_Baja/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Motivo_Baja motivo_Baja = db.Motivo_Baja.Find(id);
            if (motivo_Baja == null)
            {
                return HttpNotFound();
            }
            return View(motivo_Baja);
        }

        // POST: rrhh/Motivo_Baja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Motivo_Baja motivo_Baja = db.Motivo_Baja.Find(id);
            motivo_Baja.eliminado = true;
            motivo_Baja.activo = false;
            motivo_Baja.fecha_eliminacion = DateTime.Now;
            motivo_Baja.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            db.Entry(motivo_Baja).State = EntityState.Modified;
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
