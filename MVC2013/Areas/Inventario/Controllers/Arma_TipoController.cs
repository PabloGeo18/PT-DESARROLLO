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
    public class Arma_TipoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Arma_Tipo
        public ActionResult Index()
        {
            var arma_Tipo = db.Arma_Tipo.Include(a => a.Usuarios).Include(a => a.Usuarios1).Include(a => a.Usuarios2);
            return View(arma_Tipo.ToList());
        }

        // GET: Inventario/Arma_Tipo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arma_Tipo arma_Tipo = db.Arma_Tipo.Find(id);
            if (arma_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(arma_Tipo);
        }

        // GET: Inventario/Arma_Tipo/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Inventario/Arma_Tipo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_arma_tipo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Arma_Tipo arma_Tipo)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                arma_Tipo.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                arma_Tipo.fecha_creacion = DateTime.Now;
                arma_Tipo.activo = true;
                arma_Tipo.eliminado = false;
                db.Arma_Tipo.Add(arma_Tipo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Tipo.id_usuario_modificacion);
            return View(arma_Tipo);
        }

        // GET: Inventario/Arma_Tipo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arma_Tipo arma_Tipo = db.Arma_Tipo.Find(id);
            if (arma_Tipo == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Tipo.id_usuario_modificacion);
            return View(arma_Tipo);
        }

        // POST: Inventario/Arma_Tipo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_arma_tipo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Arma_Tipo arma_Tipo)
        {
            if (ModelState.IsValid)
            {
                Arma_Tipo arma_tipoEdit = db.Arma_Tipo.Find(arma_Tipo.id_arma_tipo);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                arma_tipoEdit.descripcion = arma_Tipo.descripcion;
                arma_tipoEdit.activo = arma_Tipo.activo;
                arma_tipoEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                arma_tipoEdit.fecha_modificacion = DateTime.Now;
                arma_tipoEdit.eliminado = false;
                db.Entry(arma_tipoEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Tipo.id_usuario_modificacion);
            return View(arma_Tipo);
        }

        // GET: Inventario/Arma_Tipo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arma_Tipo arma_Tipo = db.Arma_Tipo.Find(id);
            if (arma_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(arma_Tipo);
        }

        // POST: Inventario/Arma_Tipo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Arma_Tipo arma_Tipo = db.Arma_Tipo.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            arma_Tipo.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            arma_Tipo.fecha_eliminacion = DateTime.Now;
            arma_Tipo.eliminado = true;
            arma_Tipo.activo = false;
            db.Entry(arma_Tipo).State = EntityState.Modified;
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
