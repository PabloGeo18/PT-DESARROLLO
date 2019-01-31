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
    public class Estado_TipoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Estado_Tipo
        public ActionResult Index()
        {
            return View(db.Estado_Tipo.ToList());
        }

        // GET: Inventario/Estado_Tipo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado_Tipo estado_Tipo = db.Estado_Tipo.Find(id);
            if (estado_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(estado_Tipo);
        }

        // GET: Inventario/Estado_Tipo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inventario/Estado_Tipo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Estado_Tipo estado_Tipo)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                estado_Tipo.activo = true;
                estado_Tipo.eliminado = false;
                estado_Tipo.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                estado_Tipo.fecha_creacion = DateTime.Now;
                db.Estado_Tipo.Add(estado_Tipo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(estado_Tipo);
        }

        // GET: Inventario/Estado_Tipo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado_Tipo estado_Tipo = db.Estado_Tipo.Find(id);
            if (estado_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(estado_Tipo);
        }

        // POST: Inventario/Estado_Tipo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Estado_Tipo estado_Tipo)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Estado_Tipo estado_TipoEdit = db.Estado_Tipo.Find(estado_Tipo.id_estado_tipo);
            if (ModelState.IsValid)
            {
                estado_TipoEdit.descripcion = estado_Tipo.descripcion;
                estado_TipoEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                estado_TipoEdit.fecha_modificacion = DateTime.Now;
                db.Entry(estado_Tipo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(estado_Tipo);
        }

        // GET: Inventario/Estado_Tipo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado_Tipo estado_Tipo = db.Estado_Tipo.Find(id);
            if (estado_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(estado_Tipo);
        }

        // POST: Inventario/Estado_Tipo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Estado_Tipo estadoTipo = db.Estado_Tipo.Find(id);
            estadoTipo.activo = false;
            estadoTipo.eliminado = true;
            estadoTipo.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            estadoTipo.fecha_eliminacion = DateTime.Now;
            db.Entry(estadoTipo).State = EntityState.Modified;
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
