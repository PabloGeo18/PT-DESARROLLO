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
using MVC2013.Src.Comun.View;
using MVC2013.Src.Seguridad.To;

namespace MVC2013.Areas.Comercializacion.Controllers
{
    public class Tmp_Propuesta_Fase_PuestoController : Controller
    {
        private Protal_webEntities db = new Protal_webEntities();

        public ViewResult Mensaje()
        {
            ContextMessage msg = TempData.ContainsKey(User.Identity.Name) && TempData[User.Identity.Name].GetType() == typeof(ContextMessage) ? (ContextMessage)TempData[User.Identity.Name] : new ContextMessage();
            return View(ContextMessage.ViewLocation(this), msg);
        }

        // GET: Comercializacion/Tmp_Propuesta_Fase_Puesto
        public ActionResult Index()
        {
            var pt_Tmp_Propuesta_Fase_Puesto = db.Pt_Tmp_Propuesta_Fase_Puesto.Include(p => p.Pt_Fases_Cotizacion).Include(p => p.Pt_Puestos);
            return View(pt_Tmp_Propuesta_Fase_Puesto.ToList());
        }

        // GET: Comercializacion/Tmp_Propuesta_Fase_Puesto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Tmp_Propuesta_Fase_Puesto pt_Tmp_Propuesta_Fase_Puesto = db.Pt_Tmp_Propuesta_Fase_Puesto.Find(id);
            if (pt_Tmp_Propuesta_Fase_Puesto == null)
            {
                return HttpNotFound();
            }
            return View(pt_Tmp_Propuesta_Fase_Puesto);
        }

        // GET: Comercializacion/Tmp_Propuesta_Fase_Puesto/Create
        public ActionResult Create(int? id)
        {
            List<object> resultado = new List<object>();
            CultureInfo gt = new CultureInfo("es-GT");
            int m = DateTime.Now.Month;
            int a = DateTime.Now.Year;
            Decimal total = 0;
            Decimal totalInsList = 0;
            Decimal totalInsNew = 0;
            List<Pt_Puesto_Insumos> puestoInsumos;
            Pt_Costos_Fijos_Mes_Anio costosFijosMN = db.Pt_Costos_Fijos_Mes_Anio.Where(x => x.activo && !x.eliminado && x.ccfma_anio == a && x.ccfma_mes==m).SingleOrDefault();
            Pt_Cantidad_Planilla_Mes_Anio planillaMN = db.Pt_Cantidad_Planilla_Mes_Anio.Where(x => x.activo && !x.eliminado && x.ccpma_anio == a && x.ccpma_mes == m).SingleOrDefault();
            ViewBag.costosFijos = costosFijosMN;
            ViewBag.planillaMes = planillaMN;
            Pt_Fases_Cotizacion faseCotizacion = db.Pt_Fases_Cotizacion.Find(id);
            Pt_Tmp_Cotizacion_Fase_Margen_Conf margenConf = db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Where(x => x.activo && !x.eliminado && x.ctcfm_cfas_id == faseCotizacion.cfas_id).SingleOrDefault();
            if (margenConf == null)
            {
                ViewBag.margenFase = 0;
            }
            else {
                ViewBag.margenFase = margenConf.ctcfm_margen;
            }
            List<Pt_Puestos> puestos = faseCotizacion.Pt_Tmp_Propuesta_Fase_Puesto.Where(tpfp => tpfp.ctpf_cfas_id==id && tpfp.activo && !tpfp.eliminado).Select(tpfp => tpfp.Pt_Puestos).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos> insumos = faseCotizacion.Pt_Tmp_Cotizacion_Fase_Insumos.Where(tpfp => tpfp.ctpfi_cfas_id == id && tpfp.activo && !tpfp.eliminado).ToList();
            List<Pt_Tmp_Cotizacion_Fase_Insumos_New> listInsertInsumos = faseCotizacion.Pt_Tmp_Cotizacion_Fase_Insumos_New.Where(tpfp => tpfp.ctpfin_cfas_id == id && tpfp.activo && !tpfp.eliminado).ToList();
            foreach (var ins in insumos) {
                if (ins.ctpfi_cantidad != null && ins.Pt_Insumos.cins_precio_venta != null && ins.Pt_Insumos.cins_porcentaje_depreciacion != null) {
                    totalInsList += (((ins.ctpfi_cantidad.Value * ins.Pt_Insumos.cins_precio_venta.Value) * (ins.Pt_Insumos.cins_porcentaje_depreciacion.Value / 100)));
                }
            }
            foreach (var ins in listInsertInsumos)
            {
                if (ins.ctpfin_cantidad != null && ins.ctpfin_precio_venta != null && ins.ctpfin_porcentaje_depreciacion != null) {
                    totalInsNew += (((ins.ctpfin_cantidad.Value * ins.ctpfin_precio_venta.Value) * (ins.ctpfin_porcentaje_depreciacion.Value / 100)));
                }
            }
            foreach (var p in puestos) {
                puestoInsumos = db.Pt_Puesto_Insumos.Where(pi => pi.cpin_cpue_id == p.cpue_id && pi.activo && !pi.eliminado).ToList();
                foreach (var sumTotal in puestoInsumos.Where(x => x.activo && !x.eliminado))
                {
                    total += (((sumTotal.cpin_cantidad.Value) * (sumTotal.Pt_Insumos.cins_precio_costo.Value)));
                }
                ViewBag.sumtotal = total.ToString("c",gt);
                ViewBag.sumtotal2 = total;
            }
            ViewBag.totalIns = (totalInsList + totalInsNew).ToString("c",gt);
            ViewBag.totalIns2 = (totalInsList + totalInsNew);
            //ViewBag.total = total.ToString("c", gt);
            //List<Pt_Pagos_Puesto> pagosPuesto = faseCotizacion.Pt_Tmp_Propuesta_Fase_Puesto.Where(tpfp => tpfp.ctpf_cpue_id.HasValue && tpfp.activo && !tpfp.eliminado).Select(s => s.Pt_Puestos.Pt_Pagos_Puesto).ToList();
            List<Pt_Tmp_Propuesta_Fase_Puesto> temp = db.Pt_Tmp_Propuesta_Fase_Puesto.Where(t => t.ctpf_cfas_id == id && t.activo && !t.eliminado).ToList();
            ViewBag.temp = puestos.Select(s=> s.Pt_Tmp_Propuesta_Fase_Puesto).ToList();
            ViewBag.fase = faseCotizacion;
            ViewBag.costos = new SelectList(db.Pt_Insumos.Where(ins => ins.activo && !ins.eliminado).OrderBy(x => x.cins_descripcion), "cins_id", "cins_descripcion");
            ViewBag.puesto = puestos;
            ViewBag.listInsertIns = listInsertInsumos;
            ViewBag.insumoList = insumos;
            ViewBag.pagosPuesto = puestos.Select(s => s.Pt_Pagos_Puesto).ToList();
            ViewBag.ctpf_cfas_id = new SelectList(db.Pt_Fases_Cotizacion.Where(fc=>fc.cfas_id==id), "cfas_id", "cfas_nombre");
            ViewBag.ctpf_cpue_id = new SelectList(db.Pt_Puestos, "cpue_id", "cpue_descripcion");
            return View();
        }

        // POST: Comercializacion/Tmp_Propuesta_Fase_Puesto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pt_Tmp_Propuesta_Fase_Puesto tmp_Propuesta_Fase_Puesto)
        {
            Pt_Tmp_Propuesta_Fase_Puesto tmp = db.Pt_Tmp_Propuesta_Fase_Puesto.Where(t => t.ctpf_cfas_id == tmp_Propuesta_Fase_Puesto.ctpf_cfas_id && t.ctpf_cpue_id==tmp_Propuesta_Fase_Puesto.ctpf_cpue_id && t.activo && !t.eliminado).SingleOrDefault();
            if (ModelState.IsValid)
            {
                if (tmp == null)
                {
                    UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                    tmp_Propuesta_Fase_Puesto.ctpf_personal = 1;
                    tmp_Propuesta_Fase_Puesto.ctpf_facConIVA = 0;
                    tmp_Propuesta_Fase_Puesto.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    tmp_Propuesta_Fase_Puesto.fecha_creacion = DateTime.Now;
                    tmp_Propuesta_Fase_Puesto.activo = true;
                    tmp_Propuesta_Fase_Puesto.eliminado = false;
                    db.Pt_Tmp_Propuesta_Fase_Puesto.Add(tmp_Propuesta_Fase_Puesto);
                    db.SaveChanges();
                }
                else {
                    ContextMessage msg = new ContextMessage(ContextMessage.Info, "Este puesto ya fue ingresado. Ingrese uno nuevo");
                    msg.ReturnUrl = Url.Action("Create");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
                
                ViewBag.pagosPuesto = db.Pt_Pagos_Puesto.Where(pp => pp.cppu_cpue_id == tmp_Propuesta_Fase_Puesto.ctpf_cpue_id).ToList();
                return RedirectToAction("Create/"+tmp_Propuesta_Fase_Puesto.ctpf_cfas_id);
            }
            ViewBag.ctpf_cfas_id = new SelectList(db.Pt_Fases_Cotizacion, "cfas_id", "cfas_nombre", tmp_Propuesta_Fase_Puesto.ctpf_cfas_id);
            ViewBag.ctpf_cpue_id = new SelectList(db.Pt_Puestos, "cpue_id", "cpue_descripcion", tmp_Propuesta_Fase_Puesto.ctpf_cpue_id);
            return View(tmp_Propuesta_Fase_Puesto);
        }


        [HttpPost]
        public ActionResult UpdateCantidadPersonal(int fase, int id, int cant) {
            Pt_Tmp_Propuesta_Fase_Puesto propuesta_Fase_Puesto = db.Pt_Tmp_Propuesta_Fase_Puesto.Where(x=> x.activo && !x.eliminado && x.ctpf_cfas_id == fase && x.ctpf_cpue_id==id).SingleOrDefault();
            Pt_Tmp_Propuesta_Fase_Puesto pfp = db.Pt_Tmp_Propuesta_Fase_Puesto.Find(propuesta_Fase_Puesto.ctpf_id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            pfp.ctpf_cfas_id = propuesta_Fase_Puesto.ctpf_cfas_id;
            pfp.ctpf_cpue_id = propuesta_Fase_Puesto.ctpf_cpue_id;
            pfp.ctpf_personal = cant;
            pfp.activo = true;
            pfp.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
            pfp.fecha_modificacion = DateTime.Now;
            pfp.eliminado = false;
            db.Entry(pfp).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Create/" + propuesta_Fase_Puesto.ctpf_cfas_id);
        }

        [HttpPost]
        public ActionResult ManejoMargen(int fase, decimal ctcfm_margen)
        {
            if (ctcfm_margen <= 0 || ctcfm_margen > 100) {
                return RedirectToAction("Create/" + fase);
            }
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Pt_Tmp_Cotizacion_Fase_Margen_Conf margenConf = db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Where(x => x.activo && !x.eliminado && x.ctcfm_cfas_id == fase).SingleOrDefault();
            Pt_Tmp_Cotizacion_Fase_Margen_Conf mc = new Pt_Tmp_Cotizacion_Fase_Margen_Conf();
            if (margenConf == null)
            {
                mc.ctcfm_cfas_id = fase;
                mc.ctcfm_margen = ctcfm_margen;
                mc.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                mc.fecha_creacion = DateTime.Now;
                mc.activo = true;
                mc.eliminado = false;
                db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Add(mc);
                db.SaveChanges();
                return RedirectToAction("Create/" + fase);
            }
            else {
                Pt_Tmp_Cotizacion_Fase_Margen_Conf mcEdit = db.Pt_Tmp_Cotizacion_Fase_Margen_Conf.Find(margenConf.ctcfm_id);
                mcEdit.ctcfm_cfas_id = fase;
                mcEdit.ctcfm_margen = ctcfm_margen;
                mcEdit.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                mcEdit.fecha_creacion = DateTime.Now;
                mcEdit.activo = true;
                mcEdit.eliminado = false;
                db.Entry(mcEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create/" + mcEdit.ctcfm_cfas_id);
            }
        }

        [HttpPost]
        public ActionResult UpdateFacConIVA(int fase, int id, decimal facConIVA)
        {
            Pt_Tmp_Propuesta_Fase_Puesto propuesta_Fase_Puesto = db.Pt_Tmp_Propuesta_Fase_Puesto.Where(x => x.activo && !x.eliminado && x.ctpf_cfas_id == fase && x.ctpf_cpue_id == id).SingleOrDefault();
            Pt_Tmp_Propuesta_Fase_Puesto pfp = db.Pt_Tmp_Propuesta_Fase_Puesto.Find(propuesta_Fase_Puesto.ctpf_id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            pfp.ctpf_cfas_id = propuesta_Fase_Puesto.ctpf_cfas_id;
            pfp.ctpf_cpue_id = propuesta_Fase_Puesto.ctpf_cpue_id;
            pfp.ctpf_facConIVA = facConIVA;
            pfp.activo = true;
            pfp.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
            pfp.fecha_modificacion = DateTime.Now;
            pfp.eliminado = false;
            db.Entry(pfp).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Create/" + propuesta_Fase_Puesto.ctpf_cfas_id);
        }
        // POST: Comercializacion/Tmp_Propuesta_Fase_Puesto/CreateInsumosTmp
        //Para crear los insumos con el modal
        [HttpPost]
        public ActionResult CreateInsumosTmp(Pt_Tmp_Cotizacion_Fase_Insumos_New tmp_Cot_FInsumos)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                tmp_Cot_FInsumos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                tmp_Cot_FInsumos.fecha_creacion = DateTime.Now;
                tmp_Cot_FInsumos.activo = true;
                tmp_Cot_FInsumos.eliminado = false;
                db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Add(tmp_Cot_FInsumos);
                db.SaveChanges();
                return RedirectToAction("Create/"+tmp_Cot_FInsumos.ctpfin_cfas_id);
            }

            return View(tmp_Cot_FInsumos);
        }

        // POST: Comercializacion/Tmp_Propuesta_Fase_Puesto/CreateInsumosGenerales
        //Para crear los insumos con la lista
        [HttpPost]
        public ActionResult CreateInsumosGenerales(Pt_Tmp_Cotizacion_Fase_Insumos tmp_Cot_Fase_Insumo)
        {
            Pt_Tmp_Cotizacion_Fase_Insumos tmp = db.Pt_Tmp_Cotizacion_Fase_Insumos.Where(t => t.ctpfi_cfas_id == tmp_Cot_Fase_Insumo.ctpfi_cfas_id && t.ctpfi_cins_id == tmp_Cot_Fase_Insumo.ctpfi_cins_id && t.activo && !t.eliminado).SingleOrDefault();
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                if (tmp == null)
                {
                    tmp_Cot_Fase_Insumo.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    tmp_Cot_Fase_Insumo.fecha_creacion = DateTime.Now;
                    tmp_Cot_Fase_Insumo.activo = true;
                    tmp_Cot_Fase_Insumo.eliminado = false;
                    db.Pt_Tmp_Cotizacion_Fase_Insumos.Add(tmp_Cot_Fase_Insumo);
                    db.SaveChanges();
                }
                else
                {
                    if (tmp_Cot_Fase_Insumo.ctpfi_cfas_id == tmp.ctpfi_cfas_id && tmp_Cot_Fase_Insumo.ctpfi_cins_id == tmp.ctpfi_cins_id)
                    {
                        tmp_Cot_Fase_Insumo.fecha_modificacion = DateTime.Now;
                        tmp_Cot_Fase_Insumo.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        tmp.ctpfi_cantidad = tmp.ctpfi_cantidad + 1;
                        db.Entry(tmp).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                Pt_Fases_Cotizacion faseCotizacion = db.Pt_Fases_Cotizacion.Find(tmp_Cot_Fase_Insumo.ctpfi_cfas_id);
                List<Pt_Insumos> insumos = faseCotizacion.Pt_Tmp_Cotizacion_Fase_Insumos.Where(tpfp => tpfp.ctpfi_cfas_id == tmp_Cot_Fase_Insumo.ctpfi_cfas_id && tpfp.activo && !tpfp.eliminado).Select(tpfp => tpfp.Pt_Insumos).ToList();
                ViewBag.ins = insumos;
                ViewBag.pagosPuesto = db.Pt_Pagos_Puesto.Where(pp => pp.cppu_cpue_id == tmp_Cot_Fase_Insumo.ctpfi_cfas_id).ToList();
                ViewBag.puestoInsumos = db.Pt_Puesto_Insumos.Where(pp => pp.cpin_cins_id == tmp_Cot_Fase_Insumo.ctpfi_cins_id).ToList();
                return RedirectToAction("Create/"+tmp_Cot_Fase_Insumo.ctpfi_cfas_id);
            }
            ViewBag.ctpf_cfas_id = new SelectList(db.Pt_Fases_Cotizacion, "cfas_id", "cfas_nombre", tmp_Cot_Fase_Insumo.ctpfi_cfas_id);
            ViewBag.ctpf_cpue_id = new SelectList(db.Pt_Puestos, "cpue_id", "cpue_descripcion", tmp_Cot_Fase_Insumo.ctpfi_cfas_id);
            return View(tmp_Cot_Fase_Insumo);
        }

        // GET: Comercializacion/Tmp_Propuesta_Fase_Puesto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Tmp_Propuesta_Fase_Puesto pt_Tmp_Propuesta_Fase_Puesto = db.Pt_Tmp_Propuesta_Fase_Puesto.Find(id);
            if (pt_Tmp_Propuesta_Fase_Puesto == null)
            {
                return HttpNotFound();
            }
            ViewBag.ctpf_cfas_id = new SelectList(db.Pt_Fases_Cotizacion, "cfas_id", "cfas_nombre", pt_Tmp_Propuesta_Fase_Puesto.ctpf_cfas_id);
            ViewBag.ctpf_cpue_id = new SelectList(db.Pt_Puestos, "cpue_id", "cpue_descripcion", pt_Tmp_Propuesta_Fase_Puesto.ctpf_cpue_id);
            return View(pt_Tmp_Propuesta_Fase_Puesto);
        }

        // POST: Comercializacion/Tmp_Propuesta_Fase_Puesto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ctpf_id,ctpf_cpue_id,ctpf_cfas_id,id_usuario_creacion,fecha_creacion,id_usuario_modificacion,fecha_modificacion,id_usuario_eliminacion,fecha_eliminacion,activo,eliminado")] Pt_Tmp_Propuesta_Fase_Puesto pt_Tmp_Propuesta_Fase_Puesto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pt_Tmp_Propuesta_Fase_Puesto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ctpf_cfas_id = new SelectList(db.Pt_Fases_Cotizacion, "cfas_id", "cfas_nombre", pt_Tmp_Propuesta_Fase_Puesto.ctpf_cfas_id);
            ViewBag.ctpf_cpue_id = new SelectList(db.Pt_Puestos, "cpue_id", "cpue_descripcion", pt_Tmp_Propuesta_Fase_Puesto.ctpf_cpue_id);
            return View(pt_Tmp_Propuesta_Fase_Puesto);
        }

        // GET: Comercializacion/Tmp_Propuesta_Fase_Puesto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Tmp_Propuesta_Fase_Puesto pt_Tmp_Propuesta_Fase_Puesto = db.Pt_Tmp_Propuesta_Fase_Puesto.Find(id);
            if (pt_Tmp_Propuesta_Fase_Puesto == null)
            {
                return HttpNotFound();
            }
            return View(pt_Tmp_Propuesta_Fase_Puesto);
        }

        // POST: Comercializacion/Tmp_Propuesta_Fase_Puesto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pt_Tmp_Propuesta_Fase_Puesto pt_Tmp_Propuesta_Fase_Puesto = db.Pt_Tmp_Propuesta_Fase_Puesto.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            pt_Tmp_Propuesta_Fase_Puesto.activo = false;
            pt_Tmp_Propuesta_Fase_Puesto.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            pt_Tmp_Propuesta_Fase_Puesto.fecha_eliminacion = DateTime.Now;
            pt_Tmp_Propuesta_Fase_Puesto.eliminado = true;
            db.Entry(pt_Tmp_Propuesta_Fase_Puesto).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Create/"+pt_Tmp_Propuesta_Fase_Puesto.ctpf_cfas_id);
        }

        public ActionResult DeleteInsmos(int NewId, int ListId)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Pt_Tmp_Cotizacion_Fase_Insumos_New tmpNewInsumo = db.Pt_Tmp_Cotizacion_Fase_Insumos_New.Find(NewId);
            Pt_Tmp_Cotizacion_Fase_Insumos tmpListInsumos = db.Pt_Tmp_Cotizacion_Fase_Insumos.Find(ListId);
            if (tmpNewInsumo != null) {
                tmpNewInsumo.activo = false;
                tmpNewInsumo.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                tmpNewInsumo.fecha_eliminacion = DateTime.Now;
                tmpNewInsumo.eliminado = true;
                db.Entry(tmpNewInsumo).State = EntityState.Modified;
                db.SaveChanges();
                var id = tmpNewInsumo.ctpfin_cfas_id;
                return RedirectToAction("Create/" + id);
            }
            if (tmpListInsumos != null) {
                tmpListInsumos.activo = false;
                tmpListInsumos.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                tmpListInsumos.fecha_eliminacion = DateTime.Now;
                tmpListInsumos.eliminado = true;
                db.Entry(tmpListInsumos).State = EntityState.Modified;
                db.SaveChanges();
                var id = tmpListInsumos.ctpfi_cfas_id;
                return RedirectToAction("Create/" + id);
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
