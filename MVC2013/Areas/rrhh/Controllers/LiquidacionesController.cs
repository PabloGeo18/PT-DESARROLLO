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

namespace MVC2013.Areas.rrhh.Controllers
{
    public class LiquidacionesController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Liquidaciones
        public ActionResult Index()
        {
            var liquidaciones = db.Liquidaciones;
            return View(liquidaciones.ToList());
        }

        // GET: rrhh/Liquidaciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Liquidaciones liquidaciones = db.Liquidaciones.Find(id);
            if (liquidaciones == null)
            {
                return HttpNotFound();
            }
            return View(liquidaciones);
        }

        // GET: rrhh/Liquidaciones/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_empleado = new SelectList(db.Empleado, "id_empleado", "primer_nombre");
            return View();
        }

        // POST: rrhh/Liquidaciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_liquidacion,id_empleado,vacaciones_pendientes,fecha_ultimo_pago,indeminizacion,sueldo_pendiente,bono_14_pendiente,aguinaldo_pendiente,Total,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Liquidaciones liquidaciones)
        {
            if (ModelState.IsValid)
            {
                db.Liquidaciones.Add(liquidaciones);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", liquidaciones.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", liquidaciones.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", liquidaciones.id_usuario_modificacion);
            ViewBag.id_empleado = new SelectList(db.Empleado, "id_empleado", "primer_nombre", liquidaciones.id_empleado);
            return View(liquidaciones);
        }

        // GET: rrhh/Liquidaciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Liquidaciones liquidaciones = db.Liquidaciones.Find(id);
            if (liquidaciones == null)
            {
                return HttpNotFound();
            }
            ViewBag.empleado = liquidaciones.Empleado.primer_nombre + " " + liquidaciones.Empleado.segundo_nombre + " " + liquidaciones.Empleado.primer_apellido + " " + liquidaciones.Empleado.segundo_apellido;
            return View(liquidaciones);
        }

        // POST: rrhh/Liquidaciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_liquidacion,id_empleado,vacaciones_pendientes,fecha_ultimo_pago,indeminizacion,sueldo_pendiente,bono_14_pendiente,aguinaldo_pendiente,Total,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Liquidaciones liquidaciones)
        {
            if (ModelState.IsValid)
            {
                Liquidaciones LiquidacionesEdit = db.Liquidaciones.Find(liquidaciones.id_liquidacion);
                LiquidacionesEdit.id_empleado = liquidaciones.id_empleado;
                LiquidacionesEdit.vacaciones_pendientes = liquidaciones.vacaciones_pendientes;
                LiquidacionesEdit.fecha_ultimo_pago = liquidaciones.fecha_ultimo_pago;
                LiquidacionesEdit.indeminizacion = liquidaciones.indeminizacion;
                LiquidacionesEdit.sueldo_pendiente = liquidaciones.sueldo_pendiente;
                LiquidacionesEdit.bono_14_pendiente = liquidaciones.bono_14_pendiente;
                LiquidacionesEdit.aguinaldo_pendiente = liquidaciones.aguinaldo_pendiente;
                LiquidacionesEdit.Total = liquidaciones.Total;
                LiquidacionesEdit.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                LiquidacionesEdit.fecha_modificacion = DateTime.Now;
                db.Entry(liquidaciones).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_empleado = new SelectList(db.Empleado, "id_empleado", "primer_nombre", liquidaciones.id_empleado);
            return View(liquidaciones);
        }

        // GET: rrhh/Liquidaciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Liquidaciones liquidaciones = db.Liquidaciones.Find(id);
            if (liquidaciones == null)
            {
                return HttpNotFound();
            }
            return View(liquidaciones);
        }

        // POST: rrhh/Liquidaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Liquidaciones liquidaciones = db.Liquidaciones.Find(id);
            db.Liquidaciones.Remove(liquidaciones);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Finiquito(int id)
        {
            //return Redirect("http://172.16.46.4/ReportServer/Pages/ReportViewer.aspx?/RRHH/rpt_finiquito&finiquito=" + id);
            return Redirect("http://172.16.46.4/ReportServer/Pages/ReportViewer.aspx?/AdmSeg_PT/rpt_finiquito&finiquito=" + id);
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
