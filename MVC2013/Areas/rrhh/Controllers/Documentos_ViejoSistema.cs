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

namespace MVC2013.Areas.rrhh.Controllers
{
    public class Documentos_ViejoSistemaController : Controller
    {
        //private ProtalWeb2Doc db = new ProtalWeb2Doc();

        private AppEntities db = new AppEntities();

        // GET: Administracion/Archivoes
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ExisteEmpleado(int id_empleado)
        {
            var empleado = db.Documento_Viejo(id_empleado).FirstOrDefault();
            var empleado1 = db.Documento_Viejo_2014(id_empleado).FirstOrDefault();
            if (empleado1 == null && empleado == null)
            {
                return Json(new { empty = true }, JsonRequestBehavior.AllowGet);
            }
            var documento1 = new
            {
                nombre = "",
                id = 0,
                cv = false,
                solicitud = false,
                dpi = false,
                penales = false,
                infornet = false,
                poligrafia = false,
                estudios = false,
                academia = false,
                capacitacion = false,
                recomendacion = false,
                socioeconomico = false,
                curriculum = false,
                militar = false
            };
            if (empleado != null)
            {
                documento1 = new
                {
                    nombre = empleado.NombreEmpleado,
                    id = empleado.ptempleadoid,
                    cv = empleado.cv == null ? false : true,
                    solicitud = empleado.solicitud == null ? false : true,
                    dpi = empleado.dpi == null ? false : true,
                    penales = empleado.penales == null ? false : true,
                    infornet = empleado.infornet == null ? false : true,
                    poligrafia = empleado.poligrafia == null ? false : true,
                    estudios = empleado.estudios == null ? false : true,
                    academia = empleado.academia == null ? false : true,
                    capacitacion = empleado.capacitacion == null ? false : true,
                    recomendacion = empleado.recomendacion == null ? false : true,
                    socioeconomico = empleado.socioeconomico == null ? false : true,
                    curriculum = empleado.curriculum == null ? false : true,
                    militar = empleado.militar == null ? false : true
                };
            }
            var documento2 = new
            {
                nombre = "",
                id = 0,
                cv = false,
                solicitud = false,
                dpi = false,
                penales = false,
                infornet = false,
                poligrafia = false,
                estudios = false,
                academia = false,
                capacitacion = false,
                recomendacion = false,
                socioeconomico = false,
                curriculum = false,
                militar = false
            };

            if (empleado1 != null)
            {
                documento2 = new
                {
                    nombre = empleado1.NombreEmpleado,
                    id = empleado1.ptempleadoid,
                    cv = empleado1.cv == null ? false : true,
                    solicitud = empleado1.solicitud == null ? false : true,
                    dpi = empleado1.dpi == null ? false : true,
                    penales = empleado1.penales == null ? false : true,
                    infornet = empleado1.infornet == null ? false : true,
                    poligrafia = empleado1.poligrafia == null ? false : true,
                    estudios = empleado1.estudios == null ? false : true,
                    academia = empleado1.academia == null ? false : true,
                    capacitacion = empleado1.capacitacion == null ? false : true,
                    recomendacion = empleado1.recomendacion == null ? false : true,
                    socioeconomico = empleado1.socioeconomico == null ? false : true,
                    curriculum = empleado1.curriculum == null ? false : true,
                    militar = empleado1.militar == null ? false : true
                };
            }
            return Json(new { doc1 = documento1, doc2 = documento2 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Descargar1(int id, int val)
        {
            var empleado = db.Documento_Viejo(id).FirstOrDefault();
            switch(val)
            {
                case 1:
                    return GetFile(empleado.cv, "CV " + id.ToString(), "pdf");
                case 2:
                    return GetFile(empleado.solicitud, "SOLICITUD " + id.ToString(), "pdf");
                case 3:
                    return GetFile(empleado.dpi, "DPI " + id.ToString(), "pdf");
                case 4:
                    return GetFile(empleado.penales, "PENALES " + id.ToString(), "pdf");
                case 5:
                    return GetFile(empleado.infornet, "INFORNET " + id.ToString(), "pdf");
                case 6:
                    return GetFile(empleado.poligrafia, "POLIGRAFIA " + id.ToString(), "pdf");
                case 7:
                    return GetFile(empleado.estudios, "ESTUDIOS " + id.ToString(), "pdf");
                case 8:
                    return GetFile(empleado.academia, "ACADEMIA " + id.ToString(), "pdf");
                case 9:
                    return GetFile(empleado.capacitacion, "CAPACITACION " + id.ToString(), "pdf");
                case 10:
                    return GetFile(empleado.recomendacion, "RECOMENDACION " + id.ToString(), "pdf");
                case 11:
                    return GetFile(empleado.socioeconomico, "SOCIOECONOMICO " + id.ToString(), "pdf");
                case 12:
                    return GetFile(empleado.curriculum, "CURRICULUM " + id.ToString(), "pdf");
                case 13:
                    return GetFile(empleado.militar, "MILITAR " + id.ToString(), "pdf");

            }
            return null;
        }

        [HttpGet]
        public ActionResult Descargar2(int id, int val)
        {
            var empleado = db.Documento_Viejo_2014(id).FirstOrDefault();
            switch (val)
            {
                case 1:
                    return GetFile(empleado.cv, "CV " + id.ToString(), "pdf");
                case 2:
                    return GetFile(empleado.solicitud, "SOLICITUD " + id.ToString(), "pdf");
                case 3:
                    return GetFile(empleado.dpi, "DPI " + id.ToString(), "pdf");
                case 4:
                    return GetFile(empleado.penales, "PENALES " + id.ToString(), "pdf");
                case 5:
                    return GetFile(empleado.infornet, "INFORNET " + id.ToString(), "pdf");
                case 6:
                    return GetFile(empleado.poligrafia, "POLIGRAFIA " + id.ToString(), "pdf");
                case 7:
                    return GetFile(empleado.estudios, "ESTUDIOS " + id.ToString(), "pdf");
                case 8:
                    return GetFile(empleado.academia, "ACADEMIA " + id.ToString(), "pdf");
                case 9:
                    return GetFile(empleado.capacitacion, "CAPACITACION " + id.ToString(), "pdf");
                case 10:
                    return GetFile(empleado.recomendacion, "RECOMENDACION " + id.ToString(), "pdf");
                case 11:
                    return GetFile(empleado.socioeconomico, "SOCIOECONOMICO " + id.ToString(), "pdf");
                case 12:
                    return GetFile(empleado.curriculum, "CURRICULUM " + id.ToString(), "pdf");
                case 13:
                    return GetFile(empleado.militar, "MILITAR " + id.ToString(), "pdf");

            }
            return null;
        }

        public ActionResult GetFile(byte[] archivo, string name, string extension)
        {
            Response.Clear();
            Response.ContentType = "application/" + extension;
            Response.AddHeader("content-disposition", "attachment; filename=\"" + name + "." + extension + "\"");
            Response.BinaryWrite(archivo);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, Response.ContentType);
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
