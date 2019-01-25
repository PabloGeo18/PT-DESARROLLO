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
    public class Uniforme_TipoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Uniforme_Tipo
        public ActionResult Index()
        {
            var uniforme_Tipo = db.Uniforme_Tipo.Include(u => u.Usuarios).Include(u => u.Usuarios1).Include(u => u.Usuarios2);
            return View(uniforme_Tipo.ToList());
        }

        // GET: Inventario/Uniforme_Tipo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Uniforme_Tipo uniforme_Tipo = db.Uniforme_Tipo.Find(id);
            if (uniforme_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(uniforme_Tipo);
        }

        // GET: Inventario/Uniforme_Tipo/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Inventario/Uniforme_Tipo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_uniforme_tipo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Uniforme_Tipo uniforme_Tipo)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                uniforme_Tipo.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                uniforme_Tipo.fecha_creacion = DateTime.Now;
                uniforme_Tipo.activo = true;
                uniforme_Tipo.eliminado = false;
                db.Uniforme_Tipo.Add(uniforme_Tipo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", uniforme_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", uniforme_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", uniforme_Tipo.id_usuario_modificacion);
            return View(uniforme_Tipo);
        }

        // GET: Inventario/Uniforme_Tipo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Uniforme_Tipo uniforme_Tipo = db.Uniforme_Tipo.Find(id);
            if (uniforme_Tipo == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", uniforme_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", uniforme_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", uniforme_Tipo.id_usuario_modificacion);
            return View(uniforme_Tipo);
        }

        // POST: Inventario/Uniforme_Tipo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_uniforme_tipo,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Uniforme_Tipo uniforme_Tipo)
        {
            if (ModelState.IsValid)
            {
                Uniforme_Tipo uniforme_TipoEdit = db.Uniforme_Tipo.Find(uniforme_Tipo.id_uniforme_tipo);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                uniforme_TipoEdit.descripcion = uniforme_Tipo.descripcion;
                uniforme_TipoEdit.activo = uniforme_Tipo.activo;
                uniforme_TipoEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                uniforme_TipoEdit.fecha_modificacion = DateTime.Now;
                uniforme_TipoEdit.eliminado = false;
                db.Entry(uniforme_TipoEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", uniforme_Tipo.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", uniforme_Tipo.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", uniforme_Tipo.id_usuario_modificacion);
            return View(uniforme_Tipo);
        }

        // GET: Inventario/Uniforme_Tipo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Uniforme_Tipo uniforme_Tipo = db.Uniforme_Tipo.Find(id);
            if (uniforme_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(uniforme_Tipo);
        }

        // POST: Inventario/Uniforme_Tipo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Uniforme_Tipo uniforme_Tipo = db.Uniforme_Tipo.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            uniforme_Tipo.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            uniforme_Tipo.fecha_eliminacion = DateTime.Now;
            uniforme_Tipo.eliminado = true;
            uniforme_Tipo.activo = false;
            db.Entry(uniforme_Tipo).State = EntityState.Modified;
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
