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
    public class Consumible_TipoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Consumible_Tipo
        public ActionResult Index()
        {
            var consumible_Tipo = db.Consumible_Tipo.Include(c => c.Usuarios).Include(c => c.Usuarios1).Include(c => c.Usuarios2);
            return View(consumible_Tipo.ToList());
        }

        // GET: Inventario/Consumible_Tipo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consumible_Tipo consumible_Tipo = db.Consumible_Tipo.Find(id);
            if (consumible_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(consumible_Tipo);
        }

        // GET: Inventario/Consumible_Tipo/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Inventario/Consumible_Tipo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_consumible_tipo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Consumible_Tipo consumible_Tipo)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                consumible_Tipo.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                consumible_Tipo.fecha_creacion = DateTime.Now;
                consumible_Tipo.activo = true;
                consumible_Tipo.eliminado = false;
                db.Consumible_Tipo.Add(consumible_Tipo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", consumible_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", consumible_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", consumible_Tipo.id_usuario_modificacion);
            return View(consumible_Tipo);
        }

        // GET: Inventario/Consumible_Tipo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consumible_Tipo consumible_Tipo = db.Consumible_Tipo.Find(id);
            if (consumible_Tipo == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", consumible_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", consumible_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", consumible_Tipo.id_usuario_modificacion);
            return View(consumible_Tipo);
        }

        // POST: Inventario/Consumible_Tipo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_consumible_tipo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Consumible_Tipo consumible_Tipo)
        {
            if (ModelState.IsValid)
            {
                Consumible_Tipo consumible_TipoEdit = db.Consumible_Tipo.Find(consumible_Tipo.id_consumible_tipo);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                consumible_TipoEdit.descripcion = consumible_Tipo.descripcion;
                consumible_TipoEdit.activo = consumible_Tipo.activo;
                consumible_TipoEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                consumible_TipoEdit.fecha_modificacion = DateTime.Now;
                consumible_TipoEdit.eliminado = false;
                db.Entry(consumible_TipoEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", consumible_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", consumible_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", consumible_Tipo.id_usuario_modificacion);
            return View(consumible_Tipo);
        }

        // GET: Inventario/Consumible_Tipo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consumible_Tipo consumible_Tipo = db.Consumible_Tipo.Find(id);
            if (consumible_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(consumible_Tipo);
        }

        // POST: Inventario/Consumible_Tipo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Consumible_Tipo consumible_Tipo = db.Consumible_Tipo.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            consumible_Tipo.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            consumible_Tipo.fecha_eliminacion = DateTime.Now;
            consumible_Tipo.eliminado = true;
            consumible_Tipo.activo = false;
            db.Entry(consumible_Tipo).State = EntityState.Modified;
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
