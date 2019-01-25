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
using MVC2013.Src.Seguridad.To;

namespace MVC2013.Areas.Inventario.Controllers
{
    public class CalibresController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Calibres
        public ActionResult Index()
        {
            var calibres = db.Calibres.Include(c => c.Usuarios).Include(c => c.Usuarios1).Include(c => c.Usuarios2);
            return View(calibres.ToList());
        }

        // GET: Inventario/Calibres/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calibres calibres = db.Calibres.Find(id);
            if (calibres == null)
            {
                return HttpNotFound();
            }
            return View(calibres);
        }

        // GET: Inventario/Calibres/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Inventario/Calibres/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_calibre,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Calibres calibres)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                calibres.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                calibres.fecha_creacion = DateTime.Now;
                calibres.activo = true;
                calibres.eliminado = false;
                db.Calibres.Add(calibres);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", calibres.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", calibres.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", calibres.id_usuario_modificacion);
            return View(calibres);
        }

        // GET: Inventario/Calibres/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calibres calibres = db.Calibres.Find(id);
            if (calibres == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", calibres.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", calibres.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", calibres.id_usuario_modificacion);
            return View(calibres);
        }

        // POST: Inventario/Calibres/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_calibre,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Calibres calibres)
        {
            if (ModelState.IsValid)
            {
                Calibres calibresEdit = db.Calibres.Find(calibres.id_calibre);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                calibresEdit.descripcion = calibres.descripcion;
                calibresEdit.activo = calibres.activo;
                calibresEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                calibresEdit.fecha_modificacion = DateTime.Now;
                calibresEdit.eliminado = false;
                db.Entry(calibresEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", calibres.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", calibres.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", calibres.id_usuario_modificacion);
            return View(calibres);
        }

        // GET: Inventario/Calibres/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Calibres calibres = db.Calibres.Find(id);
            if (calibres == null)
            {
                return HttpNotFound();
            }
            return View(calibres);
        }

        // POST: Inventario/Calibres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Calibres calibres = db.Calibres.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            calibres.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            calibres.fecha_eliminacion = DateTime.Now;
            calibres.eliminado = true;
            calibres.activo = false;
            db.Entry(calibres).State = EntityState.Modified;
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
