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
    public class Costos_Fijos_Mes_AnioController : Controller
    {
        private Protal_webEntities db = new Protal_webEntities();

        public ViewResult Mensaje()
        {
            ContextMessage msg = TempData.ContainsKey(User.Identity.Name) && TempData[User.Identity.Name].GetType() == typeof(ContextMessage) ? (ContextMessage)TempData[User.Identity.Name] : new ContextMessage();
            return View(ContextMessage.ViewLocation(this), msg);
        }

        // GET: Comercializacion/Costos_Fijos_Mes_Anio
        public ActionResult Index()
        {
            int a = DateTime.Now.Year;
            int[] listanio;
            listanio = new int[] { 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };
            ViewBag.a = a;
            ViewBag.anios = listanio;
            ViewBag.meses = new SelectList(db.Pt_Meses, "cmes_id", "cmes_descripcion");
            List<Pt_Costos_Fijos_Mes_Anio> costosFijosMN = db.Pt_Costos_Fijos_Mes_Anio.Where(x => x.activo && !x.eliminado && x.ccfma_anio==a).OrderByDescending(x=>x.ccfma_mes).ToList();
            ViewBag.costosFijos = costosFijosMN;
            return View();
        }

        // GET: Comercializacion/Costos_Fijos_Mes_Anio/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Costos_Fijos_Mes_Anio pt_Costos_Fijos_Mes_Anio = db.Pt_Costos_Fijos_Mes_Anio.Find(id);
            if (pt_Costos_Fijos_Mes_Anio == null)
            {
                return HttpNotFound();
            }
            return View(pt_Costos_Fijos_Mes_Anio);
        }

        public JsonResult FiltroAnio(int? anio)
        {
            CultureInfo gt = new CultureInfo("es-GT");
            int mes = DateTime.Now.Month;
            int ani_o = DateTime.Now.Year;
            String act = (mes > 9) ? act = ani_o.ToString() + mes.ToString() : act = ani_o.ToString() + "0" + mes.ToString();
            int compActual = Int32.Parse(act);
            //Decimal totalCons = 0;
            //Decimal totalDepre = 0;
            List<object> resultado = new List<object>();
            
            List<Pt_Costos_Fijos_Mes_Anio> costosFijosMN = db.Pt_Costos_Fijos_Mes_Anio.Where(x => x.activo && !x.eliminado && x.ccfma_anio==anio).OrderByDescending(x => x.ccfma_mes).ToList();

            foreach (var item in costosFijosMN)
            {
                String eval = (item.ccfma_mes > 9) ?
                    eval = item.ccfma_anio.ToString() + item.ccfma_mes.ToString() :
                    eval = item.ccfma_anio.ToString() + "0" + item.ccfma_mes.ToString();
                int compEvaluar = Int32.Parse(eval);
                //List<Pt_Gastos_Costos_Fijos_Mes_Anio> gasCons = db.Pt_Gastos_Costos_Fijos_Mes_Anio.Where(x => x.activo && !x.eliminado && x.cgcf_consumible == true && x.cgcf_ccfma_id == item.ccfma_id).ToList();
                //List<Pt_Gastos_Costos_Fijos_Mes_Anio> gasDepre = db.Pt_Gastos_Costos_Fijos_Mes_Anio.Where(x => x.activo && !x.eliminado && x.cgcf_depreciable == true && x.cgcf_ccfma_id == item.ccfma_id).ToList();
                //foreach (var i in gasCons)
                //{
                //    totalCons += ((i.cgcf_cantidad.Value) * (i.cgcf_precio_unitario.Value));
                //}
                //foreach (var i in gasDepre)
                //{
                //    totalDepre += ((i.cgcf_cantidad.Value) * (i.cgcf_precio_unitario.Value) * ((i.cgcf_porcentaje_depreciacion.Value)/100));
                //}
                resultado.Add(new
                {
                    id = item.ccfma_id,
                    descripcion = item.ccfma_descripcion,
                    mes = item.Pt_Meses.cmes_descripcion,
                    anio = item.ccfma_anio,
                    ccfma_monto_gasto_administrativo = item.ccfma_monto_gasto_administrativo.Value.ToString("c",gt),
                    ccfma_monto_gasto_operativo = item.ccfma_monto_gasto_operativo.Value.ToString("c", gt),
                    actualdate = compActual,
                    evaluadate = compEvaluar
                    //sub_total_cons = totalCons.ToString("c", gt),
                    //sub_total_depre = totalDepre.ToString("c", gt),
                    //total = (totalCons+totalDepre).ToString("c",gt)
                });
                //totalCons = 0;
                //totalDepre = 0;
            }
            //ViewBag.anios = listanio;
            //ViewBag.tipoG = new SelectList(db.Pt_Tipo_Gasto, "ctga_id", "ctga_descripcion");
            //ViewBag.meses = new SelectList(db.Pt_Meses, "cmes_id", "cmes_descripcion");
            //ViewBag.costosFijos = costosFijosMN;


            //return View();
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        // GET: Comercializacion/Costos_Fijos_Mes_Anio/Create
        public ActionResult Create(int? anio)
        {
            int[] listanio;
            listanio = new int[] { 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };
            List<Pt_Costos_Fijos_Mes_Anio> costosFijosMN = db.Pt_Costos_Fijos_Mes_Anio.Where(x => x.activo && !x.eliminado).OrderBy(x => x.ccfma_descripcion).ToList();
            ViewBag.anios = listanio;
            ViewBag.tipoG = new SelectList(db.Pt_Tipo_Gasto, "ctga_id", "ctga_descripcion");
            ViewBag.meses = new SelectList(db.Pt_Meses, "cmes_id", "cmes_descripcion");
            //ViewBag.meses = listMeses;
            //ViewBag.tipoG = tipoGasto;
            ViewBag.costosFijos = costosFijosMN;
            return View();
        }

        // POST: Comercializacion/Costos_Fijos_Mes_Anio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pt_Costos_Fijos_Mes_Anio costosFijosMesAnio)
        {
            if (ModelState.IsValid)
            {
                Pt_Costos_Fijos_Mes_Anio list = db.Pt_Costos_Fijos_Mes_Anio.Where(x=>x.activo && !x.eliminado && x.ccfma_mes == costosFijosMesAnio.ccfma_mes && x.ccfma_anio == costosFijosMesAnio.ccfma_anio).SingleOrDefault();
                if (list==null)
                {
                    UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                    costosFijosMesAnio.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    costosFijosMesAnio.fecha_creacion = DateTime.Now;
                    costosFijosMesAnio.activo = true;
                    costosFijosMesAnio.eliminado = false;
                    db.Pt_Costos_Fijos_Mes_Anio.Add(costosFijosMesAnio);
                    db.SaveChanges();
                }
                else {
                    ContextMessage msg = new ContextMessage(ContextMessage.Info, "Los costos para este mes, ya han sido ingresados. Por favor, ingrese costos de otro mes.");
                    msg.ReturnUrl = Url.Action("Index");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
                return RedirectToAction("Index");
            }

            return View(costosFijosMesAnio);
        }

        // GET: Comercializacion/Costos_Fijos_Mes_Anio/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Costos_Fijos_Mes_Anio pt_Costos_Fijos_Mes_Anio = db.Pt_Costos_Fijos_Mes_Anio.Find(id);
            if (pt_Costos_Fijos_Mes_Anio == null)
            {
                return HttpNotFound();
            }
            ViewBag.tipoG = new SelectList(db.Pt_Tipo_Gasto, "ctga_id", "ctga_descripcion");
            ViewBag.meses = new SelectList(db.Pt_Meses.Where(x=>x.cmes_id==pt_Costos_Fijos_Mes_Anio.ccfma_mes), "cmes_id", "cmes_descripcion");
            return View(pt_Costos_Fijos_Mes_Anio);
        }

        // POST: Comercializacion/Costos_Fijos_Mes_Anio/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pt_Costos_Fijos_Mes_Anio costosFijosMesAnio)
        {
            if (ModelState.IsValid)
            {
                int mes = DateTime.Now.Month;
                int anio = DateTime.Now.Year;
                String act = (mes > 9) ? act = anio.ToString() + mes.ToString() : act = anio.ToString() + "0" + mes.ToString();
                int compActual = Int32.Parse(act);
                Pt_Costos_Fijos_Mes_Anio costosFijosMesAnioEdit = db.Pt_Costos_Fijos_Mes_Anio.Find(costosFijosMesAnio.ccfma_id);
                String eval = (costosFijosMesAnio.ccfma_mes > 9) ?
                    eval = costosFijosMesAnio.ccfma_anio.ToString() + costosFijosMesAnio.ccfma_mes.ToString() :
                    eval = costosFijosMesAnio.ccfma_anio.ToString() + "0" + costosFijosMesAnio.ccfma_mes.ToString();
                int compEvaluar = Int32.Parse(eval);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                //if (compEvaluar > compActual) {
                    costosFijosMesAnioEdit.ccfma_monto_gasto_administrativo = costosFijosMesAnio.ccfma_monto_gasto_administrativo;
                costosFijosMesAnioEdit.ccfma_monto_gasto_operativo = costosFijosMesAnio.ccfma_monto_gasto_operativo;
                //}
                //else
                //{
                //    ContextMessage msg = new ContextMessage(ContextMessage.Info, "No se puede modificar el monto para este mes. Sólo puede modificar meses futuros.");
                //    msg.ReturnUrl = Url.Action("Index");
                //    TempData[User.Identity.Name] = msg;
                //    return RedirectToAction("Mensaje");
                //}
                //costosFijosMesAnioEdit.ccfma_anio = costosFijosMesAnio.ccfma_anio;
                costosFijosMesAnioEdit.activo = true;
                costosFijosMesAnioEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                costosFijosMesAnioEdit.fecha_modificacion = DateTime.Now;
                costosFijosMesAnioEdit.eliminado = false;
                db.Entry(costosFijosMesAnioEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(costosFijosMesAnio);
        }

        // GET: Comercializacion/Costos_Fijos_Mes_Anio/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Costos_Fijos_Mes_Anio pt_Costos_Fijos_Mes_Anio = db.Pt_Costos_Fijos_Mes_Anio.Find(id);
            if (pt_Costos_Fijos_Mes_Anio == null)
            {
                return HttpNotFound();
            }
            return View(pt_Costos_Fijos_Mes_Anio);
        }

        // POST: Comercializacion/Costos_Fijos_Mes_Anio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pt_Costos_Fijos_Mes_Anio costos_Fijos_Mes_Anio = db.Pt_Costos_Fijos_Mes_Anio.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            costos_Fijos_Mes_Anio.activo = false;
            costos_Fijos_Mes_Anio.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            costos_Fijos_Mes_Anio.fecha_eliminacion = DateTime.Now;
            costos_Fijos_Mes_Anio.eliminado = true;
            db.Entry(costos_Fijos_Mes_Anio).State = EntityState.Modified;
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
