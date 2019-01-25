using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using System.Globalization;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Areas.Customers.Controllers
{
    public class Gestion_CobroController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Customers/Gestion_Cobro
        public ActionResult Index()
        {
            var clientes = new List<SelectListItem>();

            foreach (var cliente in db.Clientes.Where(c=>c.activo && !c.eliminado).OrderBy(c=>c.nombre))
            {
                var group = new SelectListGroup() { Name = cliente.nombre };
                foreach (var razon_social in cliente.Razones_Sociales.Where(r=>r.activo && !r.eliminado).OrderBy(r=>r.razon_social))
                {
                    clientes.Add(new SelectListItem()
                    {
                        Text = razon_social.razon_social,
                        Value = razon_social.id_razon_social.ToString(),
                        Group = group
                    });
                }
            }
            ViewBag.id_razon_social = clientes;
            ViewBag.tipo_gestion = new SelectList(db.Cat_Tipos_Gestion.Where(e => e.activo), "id_cat_tipo_gestion", "nombre");
            ViewBag.tipo_respuesta_gestion = new SelectList(db.Cat_Tipos_Respuesta_Gestion.Where(e => e.activo), "id_cat_tipo_respuesta_gestion", "nombre");
            return View();
        }

        [HttpGet]
        public JsonResult GetProcesosFacturas(int id_razon_social)
        {
            var result = new List<object>();
            var razon_social = db.Razones_Sociales.SingleOrDefault(r => r.activo && r.id_razon_social == id_razon_social);
            if (razon_social == null) return Json(result, JsonRequestBehavior.AllowGet);
            foreach (var item in db.Gestion_Cobro.Where(e => e.id_razon_social == id_razon_social && !e.eliminado).Select(e => e.Procesos_Facturacion))
            {
                result.Add(new { item.id_proceso_facturacion, fecha = item.fecha_proceso.ToString("MMMM", CultureInfo.GetCultureInfo("es-GT")).ToUpper() + " " + item.fecha_proceso.Year });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGestionCobro(int id_razon_social, int id_proceso_facturacion)
        {
            var GestionCobro = db.Gestion_Cobro.SingleOrDefault(e => e.id_razon_social == id_razon_social && e.id_proceso_facturacion == id_proceso_facturacion && !e.eliminado);
            if (GestionCobro == null)
            {
                return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
            }
            string actividad = "Ninguna";
            bool tiene_actividad = false;
            foreach (var item in GestionCobro.Gestion_Cobro_Detalle.Where(e=>e.activo && !e.eliminado && !e.realizada))
            {
                if(item.fecha_proyectada.ToString("dd/MM/yyyy") == DateTime.Today.ToString("dd/MM/yyyy"))
                {
                    actividad = item.Cat_Tipos_Gestion1.nombre;
                    tiene_actividad = true;
                    break;
                }
            }
            List<object> facturacion = new List<object>();
            foreach (var item in db.Resumen_Gestion_Cobro(id_proceso_facturacion, id_razon_social))
            {
                facturacion.Add(new { item.nombre, total = item.total.ToString("C", CultureInfo.GetCultureInfo("es-GT")) });
            }

            return Json(new
            {
                GestionCobro.id_gestion_cobro,
                GestionCobro.id_proceso_facturacion,
                GestionCobro.id_razon_social,
                fecha_inicio = GestionCobro.fecha_inicio.ToString("dd/MM/yyyy"),
                fecha_fin = GestionCobro.fecha_fin.HasValue ? GestionCobro.fecha_fin.Value.ToString("dd/MM/yyyy") : "",
                nombre = GestionCobro.Razones_Sociales.razon_social + " - " + GestionCobro.Razones_Sociales.Clientes.nombre,
                actividad,
                tiene_actividad,
                facturacion
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGestionCobroDetalle(int id_gestion_cobro)
        {
            List<object> resultado = new List<object>();
            foreach (var detalle in db.Gestion_Cobro_Detalle.Where(e => e.id_gestion_cobro == id_gestion_cobro && e.activo && !e.eliminado).OrderBy(e=>e.fecha_proyectada))
            {
                resultado.Add(new
                {
                    detalle.id_gestion_cobro_detalle,
                    detalle.comentario,
                    fecha_proyectada = detalle.fecha_proyectada.ToString("dd/MM/yyyy"),
                    fecha_realizado = detalle.fecha_realizado.HasValue ? detalle.fecha_realizado.Value.ToString("dd/MM/yyyy") : "",
                    detalle.id_cat_tipo_gestion_proyectada,
                    gestion_realizada = detalle.id_cat_tipo_gestion_realizada.HasValue ? detalle.Cat_Tipos_Gestion.nombre : "",
                    gestion_proyectada = detalle.Cat_Tipos_Gestion1.nombre,
                    detalle.id_cat_tipo_gestion_realizada,
                    detalle.id_cat_tipo_respuesta_gestion,
                    respuesta = detalle.id_cat_tipo_respuesta_gestion.HasValue ? detalle.Cat_Tipos_Respuesta_Gestion.nombre : "",
                    detalle.realizada
                });
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult NuevaGestionDetalle(int id_gestion_cobro, int id_tipo_gestion_proyectada, DateTime fecha_proyectada)
        {
            try
            {
                Gestion_Cobro gc = db.Gestion_Cobro.SingleOrDefault(g => g.activo && g.id_gestion_cobro == id_gestion_cobro);
                if (gc == null)
                {
                    return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
                }
                Gestion_Cobro_Detalle gcb = new Gestion_Cobro_Detalle();
                gcb.activo = true;
                gcb.eliminado = false;
                gcb.fecha_creacion = DateTime.Now;
                gcb.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                gcb.id_gestion_cobro = gc.id_gestion_cobro;
                gcb.fecha_proyectada = fecha_proyectada;
                gcb.id_cat_tipo_gestion_proyectada = id_tipo_gestion_proyectada;
                db.Gestion_Cobro_Detalle.Add(gcb);
                db.SaveChanges();
                return Json(new { id = gcb.id_gestion_cobro_detalle
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EditarGestionDestalle(int id_gestion_cobro_detalle, int id_tipo_gestion_proyectada, DateTime fecha_proyectada,
            int? id_tipo_gestion_realizada, DateTime? fecha_gestion_realizada, int? id_tipo_respuesta_gestion, string comentario, bool realizada)
        {
            try
            {
                Gestion_Cobro_Detalle gcd = db.Gestion_Cobro_Detalle.SingleOrDefault(g => g.activo && g.id_gestion_cobro_detalle == id_gestion_cobro_detalle);
                if(gcd == null)
                {
                    return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
                }
                gcd.fecha_proyectada = fecha_proyectada;
                gcd.id_cat_tipo_gestion_proyectada = id_tipo_gestion_proyectada;
                gcd.id_cat_tipo_gestion_realizada = id_tipo_gestion_realizada;
                gcd.fecha_realizado = fecha_gestion_realizada;
                gcd.id_cat_tipo_respuesta_gestion = id_tipo_respuesta_gestion;
                gcd.comentario = comentario;
                gcd.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                gcd.fecha_modificacion = DateTime.Now;
                gcd.realizada = realizada;
                db.Entry(gcd).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { result = 2 }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EliminarGestionDetalle(int id)
        {
            Gestion_Cobro_Detalle gcb = db.Gestion_Cobro_Detalle.SingleOrDefault(s => s.activo && s.id_gestion_cobro_detalle == id);
            if (gcb == null)
            {
                return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
            }
            gcb.activo = false;
            gcb.eliminado = true;
            gcb.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            gcb.fecha_eliminacion = DateTime.Now;
            db.Entry(gcb).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { result = 2 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FinalizarGestionCobro(int id_gestion_cobro, DateTime fecha)
        {
            var gestion = db.Gestion_Cobro.SingleOrDefault(s => s.activo && s.fecha_fin == null && s.id_gestion_cobro == id_gestion_cobro);
            if (gestion == null)
            {
                return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
            }
            if (gestion.Gestion_Cobro_Detalle.Where(e => e.activo && !e.eliminado && !e.realizada).Count() > 0)
            {
                return Json(new { result = 2 }, JsonRequestBehavior.AllowGet);
            }
            gestion.fecha_fin = fecha;
            gestion.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            gestion.fecha_modificacion = DateTime.Now;
            db.Entry(gestion).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { result = 3 }, JsonRequestBehavior.AllowGet);
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
