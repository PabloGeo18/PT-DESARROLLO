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
using MVC2013.Src.Comun.View;
using System.Globalization;
using MVC2013.Src.Sdc.Reports;

namespace MVC2013.Areas.rrhh.Controllers
{
    public class Vacacion_DetalleController : Controller
    {
        private AppEntities db = new AppEntities();

        public ActionResult Details(int id)
        {
            var vacacion_contrato = db.Vacacion_Contrato.SingleOrDefault(v=>v.id_vacacion_contrato == id && v.activo);
            if(vacacion_contrato == null)
            {
                return HttpNotFound();
            }
            return View(vacacion_contrato);
        }

        [HttpPost]
        public ActionResult Crear(DateTime fecha_inicio, DateTime fecha_fin, int cantidad_dias, bool tipo_vacacion, int id_vacacion_contrato, string costo)
        {
            var vacacion_contrato = db.Vacacion_Contrato.Find(id_vacacion_contrato);
            int dias_usados = 0;
            string error = "";
            if(vacacion_contrato.dias_tomados.HasValue)
            {
                dias_usados = vacacion_contrato.dias_tomados.Value;
            }
            if (vacacion_contrato.dias_total >= (dias_usados + cantidad_dias))
            {
                Vacacion_Detalle vacacion_detalle = new Vacacion_Detalle();
                vacacion_detalle.activo = true;
                vacacion_detalle.eliminado = false;
                vacacion_detalle.id_empleado = vacacion_contrato.id_empleado;
                vacacion_detalle.fecha_creacion = DateTime.Now;
                vacacion_detalle.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;

                vacacion_detalle.fecha_inicio = fecha_inicio;
                vacacion_detalle.fecha_fin = fecha_fin;
                vacacion_detalle.cantidad_dias = cantidad_dias;
                vacacion_detalle.tipo_vacacion = tipo_vacacion;
                vacacion_detalle.id_vacacion_contrato = id_vacacion_contrato;

                if (tipo_vacacion) //true implica que se pagaron las vacaciones al empleado
                {
                    vacacion_detalle.costo = Convert.ToDecimal(costo, CultureInfo.InvariantCulture);
                }
                if(vacacion_contrato.dias_tomados.HasValue)
                {
                    vacacion_contrato.dias_tomados += cantidad_dias;
                }
                else
                {
                    vacacion_contrato.dias_tomados = cantidad_dias;
                }
                try
                {
                    db.Vacacion_Detalle.Add(vacacion_detalle);
                    db.Entry(vacacion_contrato).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = id_vacacion_contrato });
                }
                catch 
                {
                    error = "Error en la conexión con el servidor. Operación no efectuada.";
                }      
            }
            else
            {
                error = "La cantidad de días ingresadas supera el límite disponible del empleado.";
            }
            ContextMessage msg = new ContextMessage(ContextMessage.Warning, error);
            msg.ReturnUrl = Url.Action("Details", new { id = id_vacacion_contrato });
            TempData[User.Identity.Name] = msg;
            return RedirectToAction("Mensaje");
        }
    
        [HttpPost]
        public ActionResult Editar(DateTime fecha_inicio, DateTime fecha_fin, int cantidad_dias, bool tipo_vacacion, string costo, int id_vacacion_detalle)
        {
            var vacacion_detalle = db.Vacacion_Detalle.Find(id_vacacion_detalle);
            string error = "";
            if(vacacion_detalle.cantidad_dias > cantidad_dias)
            {
                vacacion_detalle.Vacacion_Contrato.dias_tomados -= (vacacion_detalle.cantidad_dias - cantidad_dias);
            }
            else if(vacacion_detalle.cantidad_dias < cantidad_dias)
            {
                var dif = cantidad_dias - vacacion_detalle.cantidad_dias;
                if (vacacion_detalle.Vacacion_Contrato.dias_total >= (vacacion_detalle.cantidad_dias + dif))
                {
                    vacacion_detalle.cantidad_dias = cantidad_dias;
                    vacacion_detalle.Vacacion_Contrato.dias_tomados += dif;
                }
                else
                {
                    error = "La cantidad de días ingresadas supera el límite disponible del empleado.";
                }
            }
            if(String.IsNullOrEmpty(error))
            {
                vacacion_detalle.cantidad_dias = cantidad_dias;
                vacacion_detalle.tipo_vacacion = tipo_vacacion;
                vacacion_detalle.fecha_inicio = fecha_inicio;
                vacacion_detalle.fecha_fin = fecha_fin;
                if(tipo_vacacion)
                {
                    vacacion_detalle.costo = Convert.ToDecimal(costo, CultureInfo.InvariantCulture);
                }
                else
                {
                    vacacion_detalle.costo = null;
                }
                try
                {
                    db.Entry(vacacion_detalle).State = EntityState.Modified;
                    db.Entry(vacacion_detalle.Vacacion_Contrato).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = vacacion_detalle.id_vacacion_contrato });
                }
                catch
                {
                    error = "Error en la conexión con el servidor. Cambios no efectuados.";
                }
            }
            ContextMessage msg = new ContextMessage(ContextMessage.Warning, error);
            msg.ReturnUrl = Url.Action("Details", new { id = vacacion_detalle.id_vacacion_contrato });
            TempData[User.Identity.Name] = msg;
            return RedirectToAction("Mensaje");
        }

        public ActionResult Editar(int id_vacacion_detalle)
        {
            return RedirectToAction("Details", new { id = id_vacacion_detalle });
        }

        [HttpPost]
        public JsonResult Eliminar (int id)
        {
            try
            {
                var id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                var vacacion_detalle = db.Vacacion_Detalle.SingleOrDefault(v=>v.id_vacacion_detalle == id && v.activo);
                if(vacacion_detalle == null)
                {
                    return Json(new { response = false, msg = "No se encontro el registro." }, JsonRequestBehavior.AllowGet);
                }
                vacacion_detalle.fecha_eliminacion = DateTime.Now;
                vacacion_detalle.id_usuario_eliminacion = id_usuario;
                vacacion_detalle.activo = false;
                vacacion_detalle.eliminado = true;
                vacacion_detalle.Vacacion_Contrato.dias_tomados -= vacacion_detalle.cantidad_dias;
                db.Entry(vacacion_detalle).State = EntityState.Modified;
                db.Entry(vacacion_detalle.Vacacion_Contrato).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { response = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { response = false, msg = "Error en la conexión con el servidor. Cambios no guardados." }, JsonRequestBehavior.AllowGet);
            }

        }

        public ViewResult Mensaje() 
        {
            ContextMessage msg = (ContextMessage)TempData[User.Identity.Name];
            return View(ContextMessage.ViewLocation(this), msg);
        }

        public FileStreamResult GetConstanciaDetalle(int id)
        {
            int idValido = 0;
            Vacacion_Detalle vacacion_detalle = db.Vacacion_Detalle.SingleOrDefault(v => v.id_vacacion_detalle == id && v.activo);
            if (vacacion_detalle != null)
            {
                idValido = vacacion_detalle.id_vacacion_detalle;
            }
            string parametros = "&id_vacacion_detalle=" + idValido.ToString();
            string reporte = "rpt_Constancia_Vacaciones_Parcial";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Constancia de Vacaciones Parcial " + vacacion_detalle.id_vacacion_detalle +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult GetConstanciaGeneral(int id)
        {
            int idValido = 0;
            Vacacion_Contrato vacacion_contrato = db.Vacacion_Contrato.SingleOrDefault(v => v.id_vacacion_contrato == id && v.activo);
            if (vacacion_contrato != null)
            {
                idValido = vacacion_contrato.id_vacacion_contrato;
            }
            string parametros = "&id_vacacion_contrato=" + idValido.ToString();
            string reporte = "rpt_Constancia_Vacaciones";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Constancia de Vacaciones " + idValido.ToString() +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
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
