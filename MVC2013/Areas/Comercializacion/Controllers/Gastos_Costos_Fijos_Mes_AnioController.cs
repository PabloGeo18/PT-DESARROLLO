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
    public class Gastos_Costos_Fijos_Mes_AnioController : Controller
    {
        private Protal_webEntities db = new Protal_webEntities();

        // GET: Comercializacion/Gastos_Costos_Fijos_Mes_Anio
        public ActionResult Index(int? id)
        {
            ViewBag.costos = db.Pt_Costos_Fijos_Mes_Anio.Where(x => x.activo && !x.eliminado && x.ccfma_id == id).SingleOrDefault();
            ViewBag.id = id;
            List<Pt_Gastos_Costos_Fijos_Mes_Anio> gastosCons = db.Pt_Gastos_Costos_Fijos_Mes_Anio.Where(x => x.activo && !x.eliminado && x.cgcf_ccfma_id == id && x.cgcf_consumible == true).ToList();
            List<Pt_Gastos_Costos_Fijos_Mes_Anio> gastosDepr = db.Pt_Gastos_Costos_Fijos_Mes_Anio.Where(x => x.activo && !x.eliminado && x.cgcf_ccfma_id == id && x.cgcf_depreciable == true).ToList();
            ViewBag.cons = gastosCons;
            ViewBag.depr = gastosDepr;
            return View();
        }

        // GET: Comercializacion/Gastos_Costos_Fijos_Mes_Anio/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Gastos_Costos_Fijos_Mes_Anio pt_Gastos_Costos_Fijos_Mes_Anio = db.Pt_Gastos_Costos_Fijos_Mes_Anio.Find(id);
            if (pt_Gastos_Costos_Fijos_Mes_Anio == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = pt_Gastos_Costos_Fijos_Mes_Anio.cgcf_ccfma_id;
            return View(pt_Gastos_Costos_Fijos_Mes_Anio);
        }

        // GET: Comercializacion/Gastos_Costos_Fijos_Mes_Anio/Create
        public ActionResult Create(int? id)
        {
            ViewBag.id = id;
            List<Pt_Gastos_Costos_Fijos_Mes_Anio> gastosCons = db.Pt_Gastos_Costos_Fijos_Mes_Anio.Where(x => x.activo && !x.eliminado && x.cgcf_ccfma_id == id && x.cgcf_consumible==true).ToList();
            List<Pt_Gastos_Costos_Fijos_Mes_Anio> gastosDepr = db.Pt_Gastos_Costos_Fijos_Mes_Anio.Where(x => x.activo && !x.eliminado && x.cgcf_ccfma_id == id && x.cgcf_depreciable==true).ToList();
            ViewBag.cons = gastosCons;
            ViewBag.depr = gastosDepr;
            ViewBag.cgcf_ccfma_id = new SelectList(db.Pt_Costos_Fijos_Mes_Anio, "ccfma_id", "ccfma_descripcion");
            return View();
        }

        // POST: Comercializacion/Gastos_Costos_Fijos_Mes_Anio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        public ActionResult CreateGastos(Pt_Gastos_Costos_Fijos_Mes_Anio gastos_Costos_Fijos_Mes_Anio)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                gastos_Costos_Fijos_Mes_Anio.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                gastos_Costos_Fijos_Mes_Anio.fecha_creacion = DateTime.Now;
                gastos_Costos_Fijos_Mes_Anio.activo = true;
                gastos_Costos_Fijos_Mes_Anio.eliminado = false;
                db.Pt_Gastos_Costos_Fijos_Mes_Anio.Add(gastos_Costos_Fijos_Mes_Anio);
                db.SaveChanges();
                return RedirectToAction("Index/" + gastos_Costos_Fijos_Mes_Anio.cgcf_ccfma_id);
            }

            ViewBag.cgcf_ccfma_id = new SelectList(db.Pt_Costos_Fijos_Mes_Anio, "ccfma_id", "ccfma_descripcion", gastos_Costos_Fijos_Mes_Anio.cgcf_ccfma_id);
            return View(gastos_Costos_Fijos_Mes_Anio);
        }

        // GET: Comercializacion/Gastos_Costos_Fijos_Mes_Anio/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Gastos_Costos_Fijos_Mes_Anio pt_Gastos_Costos_Fijos_Mes_Anio = db.Pt_Gastos_Costos_Fijos_Mes_Anio.Find(id);
            if (pt_Gastos_Costos_Fijos_Mes_Anio == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = pt_Gastos_Costos_Fijos_Mes_Anio.cgcf_ccfma_id;
            return View(pt_Gastos_Costos_Fijos_Mes_Anio);
        }

        // POST: Comercializacion/Gastos_Costos_Fijos_Mes_Anio/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pt_Gastos_Costos_Fijos_Mes_Anio gastos_Costos_Fijos_Mes_Anio)
        {
            if (ModelState.IsValid)
            {
                Pt_Gastos_Costos_Fijos_Mes_Anio gastos_Costos_Fijos_Mes_AnioEdit = db.Pt_Gastos_Costos_Fijos_Mes_Anio.Find(gastos_Costos_Fijos_Mes_Anio.cgcf_id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                gastos_Costos_Fijos_Mes_AnioEdit.cgcf_descripcion = gastos_Costos_Fijos_Mes_Anio.cgcf_descripcion;
                gastos_Costos_Fijos_Mes_AnioEdit.cgcf_precio_unitario = gastos_Costos_Fijos_Mes_Anio.cgcf_precio_unitario;
                gastos_Costos_Fijos_Mes_AnioEdit.cgcf_cantidad = gastos_Costos_Fijos_Mes_Anio.cgcf_cantidad;
                gastos_Costos_Fijos_Mes_AnioEdit.cgcf_consumible = gastos_Costos_Fijos_Mes_Anio.cgcf_consumible;
                gastos_Costos_Fijos_Mes_AnioEdit.cgcf_depreciable = gastos_Costos_Fijos_Mes_Anio.cgcf_depreciable;
                gastos_Costos_Fijos_Mes_AnioEdit.cgcf_porcentaje_depreciacion = gastos_Costos_Fijos_Mes_Anio.cgcf_porcentaje_depreciacion;
                gastos_Costos_Fijos_Mes_AnioEdit.activo = true;
                gastos_Costos_Fijos_Mes_AnioEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                gastos_Costos_Fijos_Mes_AnioEdit.fecha_modificacion = DateTime.Now;
                gastos_Costos_Fijos_Mes_AnioEdit.eliminado = false;
                db.Entry(gastos_Costos_Fijos_Mes_AnioEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index/" + gastos_Costos_Fijos_Mes_Anio.cgcf_ccfma_id);
            }
            ViewBag.cgcf_ccfma_id = new SelectList(db.Pt_Costos_Fijos_Mes_Anio, "ccfma_id", "ccfma_descripcion", gastos_Costos_Fijos_Mes_Anio.cgcf_ccfma_id);
            return View(gastos_Costos_Fijos_Mes_Anio);
        }

        // GET: Comercializacion/Gastos_Costos_Fijos_Mes_Anio/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Gastos_Costos_Fijos_Mes_Anio pt_Gastos_Costos_Fijos_Mes_Anio = db.Pt_Gastos_Costos_Fijos_Mes_Anio.Find(id);
            if (pt_Gastos_Costos_Fijos_Mes_Anio == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = pt_Gastos_Costos_Fijos_Mes_Anio.cgcf_ccfma_id;
            return View(pt_Gastos_Costos_Fijos_Mes_Anio);
        }

        // POST: Comercializacion/Gastos_Costos_Fijos_Mes_Anio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? gaid)
        {
            Pt_Gastos_Costos_Fijos_Mes_Anio pt_Gastos_Costos_Fijos_Mes_Anio = db.Pt_Gastos_Costos_Fijos_Mes_Anio.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            pt_Gastos_Costos_Fijos_Mes_Anio.activo = false;
            pt_Gastos_Costos_Fijos_Mes_Anio.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
            pt_Gastos_Costos_Fijos_Mes_Anio.fecha_modificacion = DateTime.Now;
            pt_Gastos_Costos_Fijos_Mes_Anio.eliminado = true;
            db.Entry(pt_Gastos_Costos_Fijos_Mes_Anio).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index/" + pt_Gastos_Costos_Fijos_Mes_Anio.cgcf_ccfma_id);
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
