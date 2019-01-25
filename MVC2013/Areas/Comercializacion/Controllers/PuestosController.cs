using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Src.Comun.Util;
using MVC2013.Src.Entities;
using MVC2013.Src.Seguridad.To;
using Newtonsoft.Json;

namespace MVC2013.Areas.Comercializacion.Controllers
{
    public class PuestosController : Controller
    {
        private Protal_webEntities db = new Protal_webEntities();

        // GET: Comercializacion/Puestos
        public ActionResult Index()
        {
            return View(db.Pt_Puestos.Where(x => x.eliminado == false).ToList().OrderBy(p => p.cpue_descripcion));
        }

        // GET: Comercializacion/Puestos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Puestos pt_Puestos = db.Pt_Puestos.Find(id);
            if (pt_Puestos == null)
            {
                return HttpNotFound();
            }
            return View(pt_Puestos);
        }

        // GET: Comercializacion/Puestos/Create
        public ActionResult Create()
        {
            //ViewBag.armas = new SelectList(db.Pt_Insumos.Where(ins => ins.cins_es_arma == true), "cins_id", "cins_descripcion");
            //ViewBag.insumos = new SelectList(db.Pt_Insumos.Where(ins => ins.cins_es_insumo == true), "cins_id", "cins_descripcion");
            //ViewBag.uniformes = new SelectList(db.Pt_Insumos.Where(ins => ins.cins_es_uniforme == true), "cins_id", "cins_descripcion");
            //ViewBag.tipoPago = new SelectList(db.Pt_Tipo_Pagos, "ctpa_id", "ctpa_descripcion");
            return View();
        }

        // POST: Comercializacion/Puestos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pt_Puestos puestos)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                puestos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                puestos.fecha_creacion = DateTime.Now;
                puestos.activo = true;
                puestos.eliminado = false;
                db.Pt_Puestos.Add(puestos);
                db.SaveChanges();
                return RedirectToAction("Edit/"+puestos.cpue_id);
            }
            return View(puestos);
        }

        // GET: Comercializacion/Puestos/Edit/5
        public ActionResult Edit(int? id)
        {
            CultureInfo gt = new CultureInfo("es-GT");
            Decimal total = 0;
            Decimal totalC = 0;
            Decimal totalD = 0;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Puestos pt_Puestos = db.Pt_Puestos.Find(id);
            if (pt_Puestos == null)
            {
                return HttpNotFound();
            }
            List<Pt_Puesto_Insumos> puestoInsumos = db.Pt_Puesto_Insumos.Where(pi => pi.cpin_cpue_id == id && pi.activo && !pi.eliminado).ToList();
            foreach (var sumTotal in puestoInsumos.Where(x=>x.activo && !x.eliminado)) {
                total += (((sumTotal.cpin_cantidad.Value) * (sumTotal.Pt_Insumos.cins_precio_costo.Value)));
            }
            ViewBag.total = total.ToString("c",gt);
            List<Pt_Pagos_Puesto> pagosPuesto = db.Pt_Pagos_Puesto.Where(pp => pp.cppu_cpue_id == id && pp.activo && !pp.eliminado).ToList();
            ViewBag.costos = new SelectList(db.Pt_Insumos.Where(ins => ins.activo && !ins.eliminado).OrderBy(x=>x.cins_descripcion), "cins_id", "cins_descripcion");
            //ViewBag.insumos = new SelectList(db.Pt_Insumos.Where(ins => ins.cins_es_insumo == true), "cins_id", "cins_descripcion");
            //ViewBag.uniformes = new SelectList(db.Pt_Insumos.Where(ins => ins.cins_es_uniforme == true), "cins_id", "cins_descripcion");
            ViewBag.tipoPago = new SelectList(db.Pt_Tipo_Pagos, "ctpa_id", "ctpa_descripcion");
            ViewBag.puestoIns = puestoInsumos;
            ViewBag.pagosPues = pagosPuesto;
            return View(pt_Puestos);
        }

        // POST: Comercializacion/Puestos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pt_Puestos puestos)
        {
            if (ModelState.IsValid)
            {
                Pt_Puestos puestosEdit = db.Pt_Puestos.Find(puestos.cpue_id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                puestosEdit.cpue_descripcion = puestos.cpue_descripcion;
                puestosEdit.activo = true;
                puestosEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                puestosEdit.fecha_modificacion = DateTime.Now;
                puestosEdit.eliminado = false;
                db.Entry(puestosEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(puestos);
        }

        // GET: Comercializacion/Puestos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Puestos pt_Puestos = db.Pt_Puestos.Find(id);
            if (pt_Puestos == null)
            {
                return HttpNotFound();
            }
            return View(pt_Puestos);
        }

        // POST: Comercializacion/Puestos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pt_Puestos puestos = db.Pt_Puestos.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            puestos.activo = false;
            puestos.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            puestos.fecha_eliminacion = DateTime.Now;
            puestos.eliminado = true;
            db.Entry(puestos).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Comercializacion/Pagos/Create

        public ActionResult CrearPagosPuesto(Pt_Pagos_Puesto pagos)
        {
            if (pagos.cppu_porcentaje_calculo <= 0 || pagos.cppu_porcentaje_calculo > 100)
            {
                ViewBag.ErrorMessage = "Porcentaje inválido";
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                pagos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                pagos.fecha_creacion = DateTime.Now;
                pagos.activo = true;
                pagos.eliminado = false;
                db.Pt_Pagos_Puesto.Add(pagos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pagos);
        }

        public ActionResult CrearPuestoInsumos(Pt_Puesto_Insumos insumos)
        {
            Pt_Puesto_Insumos ins = db.Pt_Puesto_Insumos.Where(pi=>pi.cpin_cins_id==insumos.cpin_cins_id && pi.cpin_cpue_id==insumos.cpin_cpue_id && pi.activo && !pi.eliminado).SingleOrDefault();
            if (ModelState.IsValid)
            {
                if (ins == null)
                {
                    UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                    insumos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    insumos.fecha_creacion = DateTime.Now;
                    insumos.activo = true;
                    insumos.eliminado = false;
                    db.Pt_Puesto_Insumos.Add(insumos);
                    db.SaveChanges();
                }
                else {
                    if (insumos.cpin_cins_id == ins.cpin_cins_id && insumos.cpin_cpue_id == ins.cpin_cpue_id)
                    {
                        ins.cpin_cantidad = ins.cpin_cantidad + 1;
                        db.Entry(ins).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                ViewBag.armas = new SelectList(db.Pt_Insumos.Where(i => i.cins_es_arma== true && i.cins_id!=ins.cpin_cins_id), "cins_id", "cins_descripcion");
                ViewBag.insumos = new SelectList(db.Pt_Insumos.Where(i => i.cins_es_insumo == true && i.cins_id != ins.cpin_id), "cins_id", "cins_descripcion");
                ViewBag.uniformes = new SelectList(db.Pt_Insumos.Where(i => i.cins_es_uniforme == true && i.cins_id != ins.cpin_id), "cins_id", "cins_descripcion");
                return RedirectToAction("");
            }

            return View();
        }

        public ActionResult AumentarCantidadPuestoInsumos(Pt_Puesto_Insumos insumos)
        {
            if (ModelState.IsValid)
            {
                Pt_Puesto_Insumos insumosEdit = db.Pt_Puesto_Insumos.Find(insumos.cpin_id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                insumosEdit.cpin_cantidad = insumosEdit.cpin_cantidad+1;
                insumosEdit.activo = true;
                insumosEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                insumosEdit.fecha_modificacion = DateTime.Now;
                insumosEdit.eliminado = false;
                db.Entry(insumosEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("");
            }

            return View();
        }

        public ActionResult DisminuirCantidadPuestoInsumos(Pt_Puesto_Insumos insumos)
        {
            if (ModelState.IsValid)
            {
                Pt_Puesto_Insumos insumosEdit = db.Pt_Puesto_Insumos.Find(insumos.cpin_id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                insumosEdit.cpin_cantidad = insumosEdit.cpin_cantidad-1;
                insumosEdit.activo = true;
                insumosEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                insumosEdit.fecha_modificacion = DateTime.Now;
                insumosEdit.eliminado = false;
                db.Entry(insumosEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("");
            }

            return View();
        }

        public ActionResult EliminarPuestoInsumos(Pt_Puesto_Insumos insumos)
        {
            if (ModelState.IsValid)
            {
                Pt_Puesto_Insumos insumosEdit = db.Pt_Puesto_Insumos.Find(insumos.cpin_id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                insumosEdit.activo = false;
                insumosEdit.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                insumosEdit.fecha_eliminacion = DateTime.Now;
                insumosEdit.eliminado = true;
                db.Entry(insumosEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("");
            }

            return View();
        }

        public ActionResult EliminarPagoPuesto(Pt_Pagos_Puesto pagos)
        {
            if (ModelState.IsValid)
            {
                Pt_Pagos_Puesto pagosEdit = db.Pt_Pagos_Puesto.Find(pagos.cppu_id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                pagosEdit.activo = false;
                pagosEdit.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                pagosEdit.fecha_eliminacion = DateTime.Now;
                pagosEdit.eliminado = true;
                db.Entry(pagosEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("");
            }

            return View();
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
