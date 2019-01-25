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
    public class BodegasController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Bodegas
        public ActionResult Index()
        {
            var bodegas = db.Bodegas.Include(b => b.Usuarios).Include(b => b.Usuarios1).Include(b => b.Usuarios2);
            return View(bodegas.ToList());
        }

        // GET: Inventario/Bodegas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bodegas bodegas = db.Bodegas.Find(id);
            if (bodegas == null)
            {
                return HttpNotFound();
            }
            return View(bodegas);
        }

        // GET: Inventario/Bodegas/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Inventario/Bodegas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_bodega,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Bodegas bodegas)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                bodegas.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                bodegas.fecha_creacion = DateTime.Now;
                bodegas.activo = true;
                bodegas.eliminado = false;
                db.Bodegas.Add(bodegas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", bodegas.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", bodegas.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", bodegas.id_usuario_modificacion);
            return View(bodegas);
        }

        // GET: Inventario/Bodegas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bodegas bodegas = db.Bodegas.Find(id);
            if (bodegas == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", bodegas.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", bodegas.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", bodegas.id_usuario_modificacion);
            return View(bodegas);
        }

        // POST: Inventario/Bodegas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_bodega,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Bodegas bodegas)
        {
            if (ModelState.IsValid)
            {
                Bodegas bodegasEdit = db.Bodegas.Find(bodegas.id_bodega);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                bodegasEdit.descripcion = bodegas.descripcion;
                bodegasEdit.activo = bodegas.activo;
                bodegasEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                bodegasEdit.fecha_modificacion = DateTime.Now;
                bodegasEdit.eliminado = false;
                db.Entry(bodegasEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", bodegas.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", bodegas.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", bodegas.id_usuario_modificacion);
            return View(bodegas);
        }

        // GET: Inventario/Bodegas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bodegas bodegas = db.Bodegas.Find(id);
            if (bodegas == null)
            {
                return HttpNotFound();
            }
            return View(bodegas);
        }

        // POST: Inventario/Bodegas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bodegas bodegas = db.Bodegas.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            bodegas.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            bodegas.fecha_eliminacion = DateTime.Now;
            bodegas.eliminado = true;
            bodegas.activo = false;
            db.Entry(bodegas).State = EntityState.Modified;
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
