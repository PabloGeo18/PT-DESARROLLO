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
    public class Cantidad_Planilla_Mes_AnioController : Controller
    {
        private Protal_webEntities db = new Protal_webEntities();

        public ViewResult Mensaje()
        {
            ContextMessage msg = TempData.ContainsKey(User.Identity.Name) && TempData[User.Identity.Name].GetType() == typeof(ContextMessage) ? (ContextMessage)TempData[User.Identity.Name] : new ContextMessage();
            return View(ContextMessage.ViewLocation(this), msg);
        }

        // GET: Comercializacion/Cantidad_Planilla_Mes_Anio
        public ActionResult Index()
        {
            int[] listanio;
            listanio = new int[] { 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };
            int a = DateTime.Now.Year;
            ViewBag.a = a;
            ViewBag.anios = listanio;
            var pt_Cantidad_Planilla_Mes_Anio = db.Pt_Cantidad_Planilla_Mes_Anio.Include(p => p.Pt_Meses);
            return View(pt_Cantidad_Planilla_Mes_Anio.ToList());
        }

        public JsonResult FiltroAnio(int? anio)
        {
            List<object> resultado = new List<object>();
            List<Pt_Cantidad_Planilla_Mes_Anio> costosFijosMN = db.Pt_Cantidad_Planilla_Mes_Anio.Where(x => x.activo && !x.eliminado && x.ccpma_anio == anio).OrderByDescending(x => x.ccpma_mes).ToList();
            int mes = DateTime.Now.Month;
            int ani_o = DateTime.Now.Year;
            String act = (mes > 9) ? act = ani_o.ToString() + mes.ToString() : act = ani_o.ToString() + "0" + mes.ToString();
            int compActual = Int32.Parse(act);

            foreach (var item in costosFijosMN)
            {
                String eval = (item.ccpma_mes > 9) ?
                    eval = item.ccpma_anio.ToString() + item.ccpma_mes.ToString() :
                    eval = item.ccpma_anio.ToString() + "0" + item.ccpma_mes.ToString();
                int compEvaluar = Int32.Parse(eval);
                resultado.Add(new
                {
                    id = item.ccpma_id,
                    descripcion = item.ccpma_descripcion,
                    mes = item.Pt_Meses.cmes_descripcion,
                    anio = item.ccpma_anio,
                    cantidad = item.ccpma_cantidad_planilla,
                    actualdate = compActual,
                    evaluadate = compEvaluar
                });
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        // GET: Comercializacion/Cantidad_Planilla_Mes_Anio/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Cantidad_Planilla_Mes_Anio pt_Cantidad_Planilla_Mes_Anio = db.Pt_Cantidad_Planilla_Mes_Anio.Find(id);
            if (pt_Cantidad_Planilla_Mes_Anio == null)
            {
                return HttpNotFound();
            }
            return View(pt_Cantidad_Planilla_Mes_Anio);
        }

        // GET: Comercializacion/Cantidad_Planilla_Mes_Anio/Create
        public ActionResult Create()
        {
            int[] listanio;
            listanio = new int[] { 2010, 2011, 2012, 2013, 2014, 2015, 2016, 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024, 2025, 2026, 2027, 2028, 2029, 2030 };
            ViewBag.anios = listanio;
            ViewBag.ccpma_mes = new SelectList(db.Pt_Meses, "cmes_id", "cmes_descripcion");
            return View();
        }

        // POST: Comercializacion/Cantidad_Planilla_Mes_Anio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pt_Cantidad_Planilla_Mes_Anio cantidad_Planilla_Mes_Anio)
        {
            if (ModelState.IsValid)
            {
                Pt_Cantidad_Planilla_Mes_Anio list = db.Pt_Cantidad_Planilla_Mes_Anio.Where(x => x.activo && !x.eliminado && x.ccpma_mes == cantidad_Planilla_Mes_Anio.ccpma_mes && x.ccpma_anio == cantidad_Planilla_Mes_Anio.ccpma_anio).SingleOrDefault();
                if (list == null)
                {
                    UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                    cantidad_Planilla_Mes_Anio.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    cantidad_Planilla_Mes_Anio.fecha_creacion = DateTime.Now;
                    cantidad_Planilla_Mes_Anio.activo = true;
                    cantidad_Planilla_Mes_Anio.eliminado = false;
                    db.Pt_Cantidad_Planilla_Mes_Anio.Add(cantidad_Planilla_Mes_Anio);
                    db.SaveChanges();
                }
                else
                {
                    ContextMessage msg = new ContextMessage(ContextMessage.Info, "Los costos para este mes, ya han sido ingresados. Por favor, ingrese costos de otro mes.");
                    msg.ReturnUrl = Url.Action("Index");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
                return RedirectToAction("Index");
            }

            ViewBag.ccpma_mes = new SelectList(db.Pt_Meses, "cmes_id", "cmes_descripcion", cantidad_Planilla_Mes_Anio.ccpma_mes);
            return View(cantidad_Planilla_Mes_Anio);
        }

        // GET: Comercializacion/Cantidad_Planilla_Mes_Anio/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Cantidad_Planilla_Mes_Anio pt_Cantidad_Planilla_Mes_Anio = db.Pt_Cantidad_Planilla_Mes_Anio.Find(id);
            if (pt_Cantidad_Planilla_Mes_Anio == null)
            {
                return HttpNotFound();
            }
            ViewBag.meses = new SelectList(db.Pt_Meses.Where(x => x.cmes_id == pt_Cantidad_Planilla_Mes_Anio.ccpma_mes), "cmes_id", "cmes_descripcion");
            return View(pt_Cantidad_Planilla_Mes_Anio);
        }

        // POST: Comercializacion/Cantidad_Planilla_Mes_Anio/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pt_Cantidad_Planilla_Mes_Anio cantidad_Planilla_Mes_Anio)
        {
            if (ModelState.IsValid)
            {
                int mes = DateTime.Now.Month;
                int anio = DateTime.Now.Year;
                String act = (mes>9) ? act = anio.ToString() + mes.ToString() : act = anio.ToString() + "0" + mes.ToString();
                int compActual = Int32.Parse(act);
                Pt_Cantidad_Planilla_Mes_Anio cantidad_Planilla_Mes_AnioEdit = db.Pt_Cantidad_Planilla_Mes_Anio.Find(cantidad_Planilla_Mes_Anio.ccpma_id);
                String eval = (cantidad_Planilla_Mes_Anio.ccpma_mes > 9) ? 
                    eval = cantidad_Planilla_Mes_Anio.ccpma_anio.ToString() + cantidad_Planilla_Mes_Anio.ccpma_mes.ToString() : 
                    eval = cantidad_Planilla_Mes_Anio.ccpma_anio.ToString() + "0" + cantidad_Planilla_Mes_Anio.ccpma_mes.ToString();
                int compEvaluar = Int32.Parse(eval);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                //if (compEvaluar > compActual)
                //{
                    cantidad_Planilla_Mes_AnioEdit.ccpma_cantidad_planilla = cantidad_Planilla_Mes_Anio.ccpma_cantidad_planilla;
                //}
                //else
                //{
                //    ContextMessage msg = new ContextMessage(ContextMessage.Info, "No se puede modificar la cantidad de la planilla para este mes. Sólo puede modificar meses futuros.");
                //    msg.ReturnUrl = Url.Action("Index");
                //    TempData[User.Identity.Name] = msg;
                //    return RedirectToAction("Mensaje");
                //}
                cantidad_Planilla_Mes_AnioEdit.ccpma_mes = cantidad_Planilla_Mes_Anio.ccpma_mes;
                cantidad_Planilla_Mes_AnioEdit.activo = true;
                cantidad_Planilla_Mes_AnioEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                cantidad_Planilla_Mes_AnioEdit.fecha_modificacion = DateTime.Now;
                cantidad_Planilla_Mes_AnioEdit.eliminado = false;
                db.Entry(cantidad_Planilla_Mes_AnioEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ccpma_mes = new SelectList(db.Pt_Meses, "cmes_id", "cmes_descripcion", cantidad_Planilla_Mes_Anio.ccpma_mes);
            return View(cantidad_Planilla_Mes_Anio);
        }

        // GET: Comercializacion/Cantidad_Planilla_Mes_Anio/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Cantidad_Planilla_Mes_Anio pt_Cantidad_Planilla_Mes_Anio = db.Pt_Cantidad_Planilla_Mes_Anio.Find(id);
            if (pt_Cantidad_Planilla_Mes_Anio == null)
            {
                return HttpNotFound();
            }
            return View(pt_Cantidad_Planilla_Mes_Anio);
        }

        // POST: Comercializacion/Cantidad_Planilla_Mes_Anio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pt_Cantidad_Planilla_Mes_Anio cantidad_Planilla_Mes_Anio = db.Pt_Cantidad_Planilla_Mes_Anio.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            cantidad_Planilla_Mes_Anio.activo = false;
            cantidad_Planilla_Mes_Anio.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            cantidad_Planilla_Mes_Anio.fecha_eliminacion = DateTime.Now;
            cantidad_Planilla_Mes_Anio.eliminado = true;
            db.Entry(cantidad_Planilla_Mes_Anio).State = EntityState.Modified;
            db.SaveChanges();
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
