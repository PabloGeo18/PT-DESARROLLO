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
    public class ModelosController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Modelos
        public ActionResult Index()
        {
            var modelos = db.Modelos.Include(m => m.Usuarios).Include(m => m.Usuarios1).Include(m => m.Usuarios2);
            return View(modelos.ToList());
        }

        // GET: Inventario/Modelos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Modelos modelos = db.Modelos.Find(id);
            if (modelos == null)
            {
                return HttpNotFound();
            }
            return View(modelos);
        }

        // GET: Inventario/Modelos/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Inventario/Modelos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_modelo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Modelos modelos)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                modelos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                modelos.fecha_creacion = DateTime.Now;
                modelos.activo = true;
                modelos.eliminado = false;
                db.Modelos.Add(modelos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", modelos.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", modelos.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", modelos.id_usuario_modificacion);
            return View(modelos);
        }

        // GET: Inventario/Modelos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Modelos modelos = db.Modelos.Find(id);
            if (modelos == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", modelos.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", modelos.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", modelos.id_usuario_modificacion);
            return View(modelos);
        }

        // POST: Inventario/Modelos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_modelo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Modelos modelos)
        {
            if (ModelState.IsValid)
            {
                Modelos modelosEdit = db.Modelos.Find(modelos.id_modelo);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                modelosEdit.descripcion = modelos.descripcion;
                modelosEdit.activo = modelos.activo;
                modelosEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                modelosEdit.fecha_modificacion = DateTime.Now;
                modelosEdit.eliminado = false;
                db.Entry(modelosEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", modelos.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", modelos.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", modelos.id_usuario_modificacion);
            return View(modelos);
        }

        // GET: Inventario/Modelos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Modelos modelos = db.Modelos.Find(id);
            if (modelos == null)
            {
                return HttpNotFound();
            }
            return View(modelos);
        }

        // POST: Inventario/Modelos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Modelos modelos = db.Modelos.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            modelos.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            modelos.fecha_eliminacion = DateTime.Now;
            modelos.eliminado = true;
            modelos.activo = false;
            db.Entry(modelos).State = EntityState.Modified;
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
