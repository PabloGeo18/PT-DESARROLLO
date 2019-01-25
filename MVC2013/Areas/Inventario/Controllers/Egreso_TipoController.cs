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
    public class Egreso_TipoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Egreso_Tipo
        public ActionResult Index()
        {
            var egreso_Tipo = db.Egreso_Tipo.Include(e => e.Usuarios).Include(e => e.Usuarios1).Include(e => e.Usuarios2);
            return View(egreso_Tipo.ToList());
        }

        // GET: Inventario/Egreso_Tipo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egreso_Tipo egreso_Tipo = db.Egreso_Tipo.Find(id);
            if (egreso_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(egreso_Tipo);
        }

        // GET: Inventario/Egreso_Tipo/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Inventario/Egreso_Tipo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_egreso_tipo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Egreso_Tipo egreso_Tipo)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                egreso_Tipo.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                egreso_Tipo.fecha_creacion = DateTime.Now;
                egreso_Tipo.activo = true;
                egreso_Tipo.eliminado = false;
                db.Egreso_Tipo.Add(egreso_Tipo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", egreso_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", egreso_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", egreso_Tipo.id_usuario_modificacion);
            return View(egreso_Tipo);
        }

        // GET: Inventario/Egreso_Tipo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egreso_Tipo egreso_Tipo = db.Egreso_Tipo.Find(id);
            if (egreso_Tipo == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", egreso_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", egreso_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", egreso_Tipo.id_usuario_modificacion);
            return View(egreso_Tipo);
        }

        // POST: Inventario/Egreso_Tipo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_egreso_tipo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Egreso_Tipo egreso_Tipo)
        {
            if (ModelState.IsValid)
            {
                Egreso_Tipo egreso_TipoEdit = db.Egreso_Tipo.Find(egreso_Tipo.id_egreso_tipo);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                egreso_TipoEdit.descripcion = egreso_Tipo.descripcion;
                egreso_TipoEdit.activo = egreso_Tipo.activo;
                egreso_TipoEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                egreso_TipoEdit.fecha_modificacion = DateTime.Now;
                egreso_TipoEdit.eliminado = false;
                db.Entry(egreso_TipoEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", egreso_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", egreso_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", egreso_Tipo.id_usuario_modificacion);
            return View(egreso_Tipo);
        }

        // GET: Inventario/Egreso_Tipo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Egreso_Tipo egreso_Tipo = db.Egreso_Tipo.Find(id);
            if (egreso_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(egreso_Tipo);
        }

        // POST: Inventario/Egreso_Tipo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Egreso_Tipo egreso_Tipo = db.Egreso_Tipo.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            egreso_Tipo.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            egreso_Tipo.fecha_eliminacion = DateTime.Now;
            egreso_Tipo.eliminado = true;
            egreso_Tipo.activo = false;
            db.Entry(egreso_Tipo).State = EntityState.Modified;
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
