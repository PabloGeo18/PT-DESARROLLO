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
    public class ProveedoresController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Proveedores
        public ActionResult Index()
        {
            var proveedores = db.Proveedores.Where(p => !p.eliminado).Where(p => p.activo).Include(p => p.Usuarios).Include(p => p.Usuarios1).Include(p => p.Usuarios2);
            return View(proveedores.ToList());
        }

        // GET: Inventario/Proveedores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proveedores proveedores = db.Proveedores.Find(id);
            if (proveedores == null)
            {
                return HttpNotFound();
            }
            return View(proveedores);
        }

        // GET: Inventario/Proveedores/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Inventario/Proveedores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_proveedor,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Proveedores proveedores)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                proveedores.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                proveedores.fecha_creacion = DateTime.Now;
                proveedores.activo = true;
                proveedores.eliminado = false;
                db.Proveedores.Add(proveedores);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", proveedores.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", proveedores.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", proveedores.id_usuario_modificacion);
            return View(proveedores);
        }

        // GET: Inventario/Proveedores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proveedores proveedores = db.Proveedores.Find(id);
            if (proveedores == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", proveedores.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", proveedores.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", proveedores.id_usuario_modificacion);
            return View(proveedores);
        }

        // POST: Inventario/Proveedores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_proveedor,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Proveedores proveedores)
        {
            if (ModelState.IsValid)
            {
                Proveedores proveedoresEdit = db.Proveedores.Find(proveedores.id_proveedor);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                proveedoresEdit.descripcion = proveedores.descripcion;
                proveedoresEdit.activo = proveedores.activo;
                proveedoresEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                proveedoresEdit.fecha_modificacion = DateTime.Now;
                proveedoresEdit.eliminado = false;
                db.Entry(proveedores).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", proveedores.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", proveedores.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", proveedores.id_usuario_modificacion);
            return View(proveedores);
        }

        // GET: Inventario/Proveedores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proveedores proveedores = db.Proveedores.Find(id);
            if (proveedores == null)
            {
                return HttpNotFound();
            }
            return View(proveedores);
        }

        // POST: Inventario/Proveedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Proveedores proveedores = db.Proveedores.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            proveedores.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            proveedores.fecha_eliminacion = DateTime.Now;
            proveedores.eliminado = true;
            proveedores.activo = false;
            db.Entry(proveedores).State = EntityState.Modified;
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
