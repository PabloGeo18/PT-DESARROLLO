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
    public class Ingreso_EstadoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Ingreso_Estado
        public ActionResult Index()
        {
            var ingreso_Estado = db.Ingreso_Estado.Include(i => i.Usuarios).Include(i => i.Usuarios1).Include(i => i.Usuarios2);
            return View(ingreso_Estado.ToList());
        }

        // GET: Inventario/Ingreso_Estado/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingreso_Estado ingreso_Estado = db.Ingreso_Estado.Find(id);
            if (ingreso_Estado == null)
            {
                return HttpNotFound();
            }
            return View(ingreso_Estado);
        }

        // GET: Inventario/Ingreso_Estado/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Inventario/Ingreso_Estado/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_ingreso_estado,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Ingreso_Estado ingreso_Estado)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                ingreso_Estado.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                ingreso_Estado.fecha_creacion = DateTime.Now;
                ingreso_Estado.activo = true;
                ingreso_Estado.eliminado = false;
                db.Ingreso_Estado.Add(ingreso_Estado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", ingreso_Estado.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", ingreso_Estado.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", ingreso_Estado.id_usuario_modificacion);
            return View(ingreso_Estado);
        }

        // GET: Inventario/Ingreso_Estado/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingreso_Estado ingreso_Estado = db.Ingreso_Estado.Find(id);
            if (ingreso_Estado == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", ingreso_Estado.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", ingreso_Estado.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", ingreso_Estado.id_usuario_modificacion);
            return View(ingreso_Estado);
        }

        // POST: Inventario/Ingreso_Estado/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_ingreso_estado,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Ingreso_Estado ingreso_Estado)
        {
            if (ModelState.IsValid)
            {
                Ingreso_Estado ingreso_EstadoEdit = db.Ingreso_Estado.Find(ingreso_Estado.id_ingreso_estado);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                ingreso_EstadoEdit.descripcion = ingreso_Estado.descripcion;
                ingreso_EstadoEdit.activo = ingreso_Estado.activo;
                ingreso_EstadoEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                ingreso_EstadoEdit.fecha_modificacion = DateTime.Now;
                ingreso_EstadoEdit.eliminado = false;
                db.Entry(ingreso_EstadoEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", ingreso_Estado.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", ingreso_Estado.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", ingreso_Estado.id_usuario_modificacion);
            return View(ingreso_Estado);
        }

        // GET: Inventario/Ingreso_Estado/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingreso_Estado ingreso_Estado = db.Ingreso_Estado.Find(id);
            if (ingreso_Estado == null)
            {
                return HttpNotFound();
            }
            return View(ingreso_Estado);
        }

        // POST: Inventario/Ingreso_Estado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ingreso_Estado ingreso_Estado = db.Ingreso_Estado.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            ingreso_Estado.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            ingreso_Estado.fecha_eliminacion = DateTime.Now;
            ingreso_Estado.eliminado = true;
            ingreso_Estado.activo = false;
            db.Entry(ingreso_Estado).State = EntityState.Modified;
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
