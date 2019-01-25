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
    public class Transaccion_Inventario_TipoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Transaccion_Inventario_Tipo
        public ActionResult Index()
        {
            var transaccion_Inventario_Tipo = db.Transaccion_Inventario_Tipo.Include(t => t.Usuarios).Include(t => t.Usuarios1).Include(t => t.Usuarios2);
            return View(transaccion_Inventario_Tipo.ToList());
        }

        // GET: Inventario/Transaccion_Inventario_Tipo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaccion_Inventario_Tipo transaccion_Inventario_Tipo = db.Transaccion_Inventario_Tipo.Find(id);
            if (transaccion_Inventario_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(transaccion_Inventario_Tipo);
        }

        // GET: Inventario/Transaccion_Inventario_Tipo/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Inventario/Transaccion_Inventario_Tipo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_transaccion_inventario_tipo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Transaccion_Inventario_Tipo transaccion_Inventario_Tipo)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                transaccion_Inventario_Tipo.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                transaccion_Inventario_Tipo.fecha_creacion = DateTime.Now;
                transaccion_Inventario_Tipo.activo = true;
                transaccion_Inventario_Tipo.eliminado = false;
                db.Transaccion_Inventario_Tipo.Add(transaccion_Inventario_Tipo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", transaccion_Inventario_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", transaccion_Inventario_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", transaccion_Inventario_Tipo.id_usuario_modificacion);
            return View(transaccion_Inventario_Tipo);
        }

        // GET: Inventario/Transaccion_Inventario_Tipo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaccion_Inventario_Tipo transaccion_Inventario_Tipo = db.Transaccion_Inventario_Tipo.Find(id);
            if (transaccion_Inventario_Tipo == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", transaccion_Inventario_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", transaccion_Inventario_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", transaccion_Inventario_Tipo.id_usuario_modificacion);
            return View(transaccion_Inventario_Tipo);
        }

        // POST: Inventario/Transaccion_Inventario_Tipo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_transaccion_inventario_tipo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Transaccion_Inventario_Tipo transaccion_Inventario_Tipo)
        {
            if (ModelState.IsValid)
            {
                Transaccion_Inventario_Tipo transaccion_Inventario_TipoEdit = db.Transaccion_Inventario_Tipo.Find(transaccion_Inventario_Tipo.id_transaccion_inventario_tipo);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                transaccion_Inventario_TipoEdit.descripcion = transaccion_Inventario_Tipo.descripcion;
                transaccion_Inventario_TipoEdit.activo = transaccion_Inventario_Tipo.activo;
                transaccion_Inventario_TipoEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                transaccion_Inventario_TipoEdit.fecha_modificacion = DateTime.Now;
                transaccion_Inventario_TipoEdit.eliminado = false;
                db.Entry(transaccion_Inventario_TipoEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", transaccion_Inventario_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", transaccion_Inventario_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", transaccion_Inventario_Tipo.id_usuario_modificacion);
            return View(transaccion_Inventario_Tipo);
        }

        // GET: Inventario/Transaccion_Inventario_Tipo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaccion_Inventario_Tipo transaccion_Inventario_Tipo = db.Transaccion_Inventario_Tipo.Find(id);
            if (transaccion_Inventario_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(transaccion_Inventario_Tipo);
        }

        // POST: Inventario/Transaccion_Inventario_Tipo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaccion_Inventario_Tipo transaccion_Inventario_Tipo = db.Transaccion_Inventario_Tipo.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            transaccion_Inventario_Tipo.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            transaccion_Inventario_Tipo.fecha_eliminacion = DateTime.Now;
            transaccion_Inventario_Tipo.eliminado = true;
            transaccion_Inventario_Tipo.activo = false;
            db.Entry(transaccion_Inventario_Tipo).State = EntityState.Modified;
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
