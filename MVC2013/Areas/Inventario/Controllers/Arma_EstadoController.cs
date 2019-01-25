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
    public class Arma_EstadoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Arma_Estado
        public ActionResult Index()
        {
            var arma_Estado = db.Arma_Estado.Include(a => a.Usuarios).Include(a => a.Usuarios1).Include(a => a.Usuarios2);
            return View(arma_Estado.ToList());
        }

        // GET: Inventario/Arma_Estado/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arma_Estado arma_Estado = db.Arma_Estado.Find(id);
            if (arma_Estado == null)
            {
                return HttpNotFound();
            }
            return View(arma_Estado);
        }

        // GET: Inventario/Arma_Estado/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Inventario/Arma_Estado/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_arma_estado,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Arma_Estado arma_Estado)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                arma_Estado.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                arma_Estado.fecha_creacion = DateTime.Now;
                arma_Estado.activo = true;
                arma_Estado.eliminado = false;
                db.Arma_Estado.Add(arma_Estado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Estado.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Estado.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Estado.id_usuario_modificacion);
            return View(arma_Estado);
        }

        // GET: Inventario/Arma_Estado/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arma_Estado arma_Estado = db.Arma_Estado.Find(id);
            if (arma_Estado == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Estado.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Estado.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Estado.id_usuario_modificacion);
            return View(arma_Estado);
        }

        // POST: Inventario/Arma_Estado/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_arma_estado,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Arma_Estado arma_Estado)
        {
            if (ModelState.IsValid)
            {
                Arma_Estado arma_estadoEdit = db.Arma_Estado.Find(arma_Estado.id_arma_estado);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                arma_estadoEdit.descripcion = arma_Estado.descripcion;
                arma_estadoEdit.activo = arma_Estado.activo;
                arma_estadoEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                arma_estadoEdit.fecha_modificacion = DateTime.Now;
                arma_estadoEdit.eliminado = false;
                db.Entry(arma_estadoEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Estado.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Estado.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", arma_Estado.id_usuario_modificacion);
            return View(arma_Estado);
        }

        // GET: Inventario/Arma_Estado/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arma_Estado arma_Estado = db.Arma_Estado.Find(id);
            if (arma_Estado == null)
            {
                return HttpNotFound();
            }
            return View(arma_Estado);
        }

        // POST: Inventario/Arma_Estado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Arma_Estado arma_Estado = db.Arma_Estado.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            arma_Estado.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            arma_Estado.fecha_eliminacion = DateTime.Now;
            arma_Estado.eliminado = true;
            arma_Estado.activo = false;
            db.Entry(arma_Estado).State = EntityState.Modified;
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
