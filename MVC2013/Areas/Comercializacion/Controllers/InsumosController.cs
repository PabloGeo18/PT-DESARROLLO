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
    public class InsumosController : Controller
    {
        private Protal_webEntities db = new Protal_webEntities();

        // GET: Comercializacion/Insumos
        public ActionResult Index()
        {
            return View(db.Pt_Insumos.Where(x => x.eliminado == false).ToList().OrderBy(i => i.cins_descripcion));
        }

        // GET: Comercializacion/Insumos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Insumos pt_Insumos = db.Pt_Insumos.Find(id);
            if (pt_Insumos == null)
            {
                return HttpNotFound();
            }
            return View(pt_Insumos);
        }

        // GET: Comercializacion/Insumos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comercializacion/Insumos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Pt_Insumos insumos)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                insumos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                insumos.fecha_creacion = DateTime.Now;
                insumos.activo = true;
                insumos.eliminado = false;
                db.Pt_Insumos.Add(insumos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insumos);
        }

        // GET: Comercializacion/Insumos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Insumos pt_Insumos = db.Pt_Insumos.Find(id);
            if (pt_Insumos == null)
            {
                return HttpNotFound();
            }
            return View(pt_Insumos);
        }

        // POST: Comercializacion/Insumos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Pt_Insumos insumos)
        {
            if (ModelState.IsValid)
            {
                Pt_Insumos insumosEdit = db.Pt_Insumos.Find(insumos.cins_id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                insumosEdit.cins_descripcion = insumos.cins_descripcion;
                insumosEdit.cins_precio_costo = insumos.cins_precio_costo;
                insumosEdit.cins_precio_venta = insumos.cins_precio_venta;
                insumosEdit.cins_es_uniforme = insumos.cins_es_uniforme;
                insumosEdit.cins_talla = insumos.cins_talla;
                insumosEdit.cins_depreciacion = insumos.cins_depreciacion;
                insumosEdit.cins_porcentaje_depreciacion = insumos.cins_porcentaje_depreciacion;
                insumosEdit.activo = true;
                insumosEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                insumosEdit.fecha_modificacion = DateTime.Now;
                insumosEdit.eliminado = false;
                db.Entry(insumosEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insumos);
        }

        // GET: Comercializacion/Insumos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Insumos pt_Insumos = db.Pt_Insumos.Find(id);
            if (pt_Insumos == null)
            {
                return HttpNotFound();
            }
            return View(pt_Insumos);
        }

        // POST: Comercializacion/Insumos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pt_Insumos insumos = db.Pt_Insumos.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            insumos.activo = false;
            insumos.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            insumos.fecha_eliminacion = DateTime.Now;
            insumos.eliminado = true;
            db.Entry(insumos).State = EntityState.Modified;
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
