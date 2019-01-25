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
    public class Articulo_TipoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Articulo_Tipo
        public ActionResult Index()
        {
            var articulo_Tipo = db.Articulo_Tipo.Include(a => a.Usuarios).Include(a => a.Usuarios1).Include(a => a.Usuarios2);
            return View(articulo_Tipo.ToList());
        }

        // GET: Inventario/Articulo_Tipo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articulo_Tipo articulo_Tipo = db.Articulo_Tipo.Find(id);
            if (articulo_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(articulo_Tipo);
        }

        // GET: Inventario/Articulo_Tipo/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Inventario/Articulo_Tipo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_articulo_tipo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Articulo_Tipo articulo_Tipo)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                articulo_Tipo.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                articulo_Tipo.fecha_creacion = DateTime.Now;
                articulo_Tipo.activo = true;
                articulo_Tipo.eliminado = false;
                db.Articulo_Tipo.Add(articulo_Tipo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", articulo_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", articulo_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", articulo_Tipo.id_usuario_modificacion);
            return View(articulo_Tipo);
        }

        // GET: Inventario/Articulo_Tipo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articulo_Tipo articulo_Tipo = db.Articulo_Tipo.Find(id);
            if (articulo_Tipo == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", articulo_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", articulo_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", articulo_Tipo.id_usuario_modificacion);
            return View(articulo_Tipo);
        }

        // POST: Inventario/Articulo_Tipo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_articulo_tipo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Articulo_Tipo articulo_Tipo)
        {
            if (ModelState.IsValid)
            {
                Articulo_Tipo articulo_tipoEdit = db.Articulo_Tipo.Find(articulo_Tipo.id_articulo_tipo);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                articulo_tipoEdit.descripcion = articulo_Tipo.descripcion;
                articulo_tipoEdit.activo = articulo_Tipo.activo;
                articulo_tipoEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                articulo_tipoEdit.fecha_modificacion = DateTime.Now;
                articulo_tipoEdit.eliminado = false;
                db.Entry(articulo_tipoEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", articulo_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", articulo_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", articulo_Tipo.id_usuario_modificacion);
            return View(articulo_Tipo);
        }

        // GET: Inventario/Articulo_Tipo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articulo_Tipo articulo_Tipo = db.Articulo_Tipo.Find(id);
            if (articulo_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(articulo_Tipo);
        }

        // POST: Inventario/Articulo_Tipo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Articulo_Tipo articulo_Tipo = db.Articulo_Tipo.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            articulo_Tipo.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            articulo_Tipo.fecha_eliminacion = DateTime.Now;
            articulo_Tipo.eliminado = true;
            articulo_Tipo.activo = false;
            db.Entry(articulo_Tipo).State = EntityState.Modified;
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
