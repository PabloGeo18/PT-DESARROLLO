using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Src.Seguridad.To;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Areas.Inventario.Controllers
{
    public class Traslado_TipoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Traslado_Tipo
        public ActionResult Index()
        {
            return View(db.Traslado_Tipo.ToList());
        }

        // GET: Inventario/Traslado_Tipo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslado_Tipo traslado_Tipo = db.Traslado_Tipo.Find(id);
            if (traslado_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(traslado_Tipo);
        }

        // GET: Inventario/Traslado_Tipo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inventario/Traslado_Tipo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Traslado_Tipo traslado_Tipo)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                traslado_Tipo.activo = true;
                traslado_Tipo.eliminado = false;
                traslado_Tipo.id_usuario_creacion =usuarioTO.usuario.id_usuario;
                traslado_Tipo.fecha_creacion = DateTime.Now;
                db.Traslado_Tipo.Add(traslado_Tipo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(traslado_Tipo);
        }

        // GET: Inventario/Traslado_Tipo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslado_Tipo traslado_Tipo = db.Traslado_Tipo.Find(id);
            if (traslado_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(traslado_Tipo);
        }

        // POST: Inventario/Traslado_Tipo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Traslado_Tipo traslado_Tipo)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Traslado_Tipo trasladoTipoEdit = db.Traslado_Tipo.Find(traslado_Tipo.id_traslado_tipo);
            if (ModelState.IsValid)
            {
                trasladoTipoEdit.descripcion = traslado_Tipo.descripcion;
                trasladoTipoEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                trasladoTipoEdit.fecha_modificacion = DateTime.Now;
                db.Entry(trasladoTipoEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(traslado_Tipo);
        }

        // GET: Inventario/Traslado_Tipo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Traslado_Tipo traslado_Tipo = db.Traslado_Tipo.Find(id);
            if (traslado_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(traslado_Tipo);
        }

        // POST: Inventario/Traslado_Tipo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Traslado_Tipo traslado_Tipo = db.Traslado_Tipo.Find(id);
            traslado_Tipo.activo = false;
            traslado_Tipo.eliminado = true;
            traslado_Tipo.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            traslado_Tipo.fecha_eliminacion = DateTime.Now;
            db.Entry(traslado_Tipo).State = EntityState.Modified;
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
