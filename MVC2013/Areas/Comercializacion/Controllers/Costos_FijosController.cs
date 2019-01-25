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

namespace MVC2013.Areas.Comercializacion.Controllers
{
    public class Costos_FijosController : Controller
    {
        private Protal_webEntities db = new Protal_webEntities();

        // GET: Comercializacion/Costos_Fijos
        public ActionResult Index()
        {
            return View(db.Pt_Costos_Fijos.Where(x => x.eliminado == false).ToList().OrderBy(or=>or.ccof_descripcion));
        }

        // GET: Comercializacion/Costos_Fijos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Costos_Fijos pt_Costos_Fijos = db.Pt_Costos_Fijos.Find(id);
            if (pt_Costos_Fijos == null)
            {
                return HttpNotFound();
            }
            return View(pt_Costos_Fijos);
        }

        // GET: Comercializacion/Costos_Fijos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comercializacion/Costos_Fijos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pt_Costos_Fijos costos_Fijos)
        {
            if (costos_Fijos.ccof_descripcion == null)
            {
                ModelState.AddModelError("ccof_descripcion", "ERROR: Este valor no puede ir vacío.");
            }
            if (costos_Fijos.ccof_precio_unitario == null)
            {
                ModelState.AddModelError("ccof_precio_unitario", "ERROR: Este valor no puede ir vacío. Y debe ser un número.");
            }

            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                costos_Fijos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                costos_Fijos.fecha_creacion = DateTime.Now;
                costos_Fijos.activo = true;
                costos_Fijos.eliminado = false;
                db.Pt_Costos_Fijos.Add(costos_Fijos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(costos_Fijos);
        }

        // GET: Comercializacion/Costos_Fijos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Costos_Fijos pt_Costos_Fijos = db.Pt_Costos_Fijos.Find(id);
            if (pt_Costos_Fijos == null)
            {
                return HttpNotFound();
            }
            return View(pt_Costos_Fijos);
        }

        // POST: Comercializacion/Costos_Fijos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pt_Costos_Fijos costos_Fijos)
        {
            if (costos_Fijos.ccof_descripcion == null)
            {
                ModelState.AddModelError("ccof_descripcion", "ERROR: Este valor no puede ir vacío.");
            }
            if (costos_Fijos.ccof_precio_unitario == null)
            {
                ModelState.AddModelError("ccof_precio_unitario", "ERROR: Este valor no puede ir vacío. Y debe ser un número.");
            }
            if (ModelState.IsValid)
            {
                Pt_Costos_Fijos costos_FijosEdit = db.Pt_Costos_Fijos.Find(costos_Fijos.ccof_id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                costos_FijosEdit.ccof_descripcion = costos_Fijos.ccof_descripcion;
                costos_FijosEdit.ccof_precio_unitario = costos_Fijos.ccof_precio_unitario;
                costos_FijosEdit.ccof_consumible = costos_Fijos.ccof_consumible;
                costos_FijosEdit.ccof_depreciable = costos_Fijos.ccof_depreciable;
                costos_FijosEdit.activo = true;
                costos_FijosEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                costos_FijosEdit.fecha_modificacion = DateTime.Now;
                costos_FijosEdit.eliminado = false;
                db.Entry(costos_FijosEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(costos_Fijos);
        }

        // GET: Comercializacion/Costos_Fijos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Costos_Fijos pt_Costos_Fijos = db.Pt_Costos_Fijos.Find(id);
            if (pt_Costos_Fijos == null)
            {
                return HttpNotFound();
            }
            return View(pt_Costos_Fijos);
        }

        // POST: Comercializacion/Costos_Fijos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pt_Costos_Fijos costos_Fijos = db.Pt_Costos_Fijos.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            costos_Fijos.activo = false;
            costos_Fijos.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
            costos_Fijos.fecha_modificacion = DateTime.Now;
            costos_Fijos.eliminado = true;
            db.Entry(costos_Fijos).State = EntityState.Modified;
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
