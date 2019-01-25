using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Src.Comun.View;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Areas.EstadoFuerza.Controllers
{
    public class Estado_Fuerza_DetalleController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: EstadoFuerza/Estado_Fuerza_Detalle
        public ActionResult Index()
        {
            Estado_Fuerza ef = db.Estado_Fuerza.SingleOrDefault(e => e.activo && !e.eliminado && e.fecha_finalizacion == null);
            if(ef == null)
            {
                return HttpNotFound();
            }
            return View(ef);
        }

        public JsonResult ExisteEmpleado(int id_empleado)
        {
            object resultado = new object();
            Empleado empleado = db.Empleado.Find(id_empleado);
            if (empleado == null)
            {
                resultado = new { error = true };
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            resultado = new
            {
                error = false,
                id_empleado,
                nombre = empleado.primer_nombre + " " +
                    (!String.IsNullOrEmpty(empleado.segundo_nombre) ? empleado.segundo_nombre + " " : "") +
                    empleado.primer_apellido + " " +
                    (!String.IsNullOrEmpty(empleado.segundo_apellido) ? empleado.segundo_apellido : "")
            };
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Search(DateTime fecha_desde, DateTime? fecha_hasta, int? id_empleado)
        {
            List<object> resultados = new List<object>();
            if(!id_empleado.HasValue)
            {
                id_empleado = -1;
            }
            if (!fecha_hasta.HasValue)
            {
                fecha_hasta = DateTime.Now;
            }
            foreach (var ef in db.Consulta_Estado_Fuerza(fecha_desde, fecha_hasta, id_empleado.Value))
            {
                resultados.Add(new {
                    ef.id_empleado,
                    ef.cliente, 
                    ef.empleado, 
                    ef.id_estado_fuerza,
                    ef.id_estado_fuerza_detalle,
                    ef.observacion,
                    ef.razon_social,
                    ef.situacion,
                    ef.tipo_servicio,
                    ef.ubicacion,
                    fecha = ef.fecha.Value.ToString("dd/MM/yyyy") 
                });
            }
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        // GET: EstadoFuerza/Estado_Fuerza_Detalle/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado_Fuerza_Detalle estado_Fuerza_Detalle = db.Estado_Fuerza_Detalle.SingleOrDefault(e => e.id_estado_fuerza_detalle == id && e.activo);
            if (estado_Fuerza_Detalle == null)
            {
                return HttpNotFound();
            }
            return View(estado_Fuerza_Detalle);
        }

        // GET: EstadoFuerza/Estado_Fuerza_Detalle/Create
        public ActionResult Create()
        {
            ViewBag.id_situacion = new SelectList(db.Situacion.Where(e => !e.eliminado).Select(e => new { e.id_situacion, nombre = e.id_situacion.ToString() + " - " + e.nombre }), "id_situacion", "nombre");
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente.Where(e => !e.eliminado).Select(e => new { e.id_cat_tipo_agente, nombre = e.id_cat_tipo_agente.ToString() + " - " + e.nombre }), "id_cat_tipo_agente", "nombre");
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(e => !e.eliminado).Select(e => new { e.id_ubicacion, direccion = e.id_ubicacion.ToString() + " - " + e.direccion }), "id_ubicacion", "direccion"); 
            ViewBag.fecha = "";
            return View();
        }

        // POST: EstadoFuerza/Estado_Fuerza_Detalle/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Estado_Fuerza_Detalle estado_Fuerza_Detalle, DateTime fecha)
        {
            Estado_Fuerza ef = db.Estado_Fuerza.SingleOrDefault(e => e.fecha == fecha && !e.eliminado);
            if (ef == null)
            {
                ModelState.AddModelError("", "No existe un Estado de Fuerza para la fecha ingresada.");
            }
            else
            {
                //Validar que el registro no exista en el Estado de Fuerza
                var detalle_ef = ef.Estado_Fuerza_Detalle.SingleOrDefault(
                        e => e.activo && !e.eliminado &&  e.id_empleado == estado_Fuerza_Detalle.id_empleado);
                if (detalle_ef != null)
                {
                    ModelState.AddModelError("", "El registro ya existe en el Estado de Fuerza actual.");
                }
                if (ModelState.IsValid)
                {
                    estado_Fuerza_Detalle.id_estado_fuerza = ef.id_estado_fuerza;
                    estado_Fuerza_Detalle.fecha = ef.fecha;
                    estado_Fuerza_Detalle.activo = true;
                    estado_Fuerza_Detalle.eliminado = false;
                    estado_Fuerza_Detalle.fecha_creacion = DateTime.Now;
                    estado_Fuerza_Detalle.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Estado_Fuerza_Detalle.Add(estado_Fuerza_Detalle);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.fecha = fecha.ToString("dd/MM/yyyy");
            ViewBag.id_situacion = new SelectList(db.Situacion.Where(e => !e.eliminado).Select(e => new { e.id_situacion, nombre = e.id_situacion.ToString() + " - " + e.nombre }), "id_situacion", "nombre", estado_Fuerza_Detalle.id_situacion);
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente.Where(e => !e.eliminado).Select(e => new { e.id_cat_tipo_agente, nombre = e.id_cat_tipo_agente.ToString() + " - " + e.nombre }), "id_cat_tipo_agente", "nombre", estado_Fuerza_Detalle.id_cat_tipo_agente);
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(e => !e.eliminado).Select(e => new { e.id_ubicacion, direccion = e.id_ubicacion.ToString() + " - " + e.direccion }), "id_ubicacion", "direccion", estado_Fuerza_Detalle.id_ubicacion);
            return View(estado_Fuerza_Detalle);
        }

        // GET: EstadoFuerza/Estado_Fuerza_Detalle/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado_Fuerza_Detalle estado_Fuerza_Detalle = db.Estado_Fuerza_Detalle.SingleOrDefault(e => e.id_estado_fuerza_detalle == id && e.activo);
            if (estado_Fuerza_Detalle == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_situacion = new SelectList(db.Situacion.Where(e => !e.eliminado).Select(e => new { e.id_situacion, nombre = e.id_situacion.ToString() + " - " + e.nombre }), "id_situacion", "nombre", estado_Fuerza_Detalle.id_situacion);
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente.Where(e => !e.eliminado).Select(e => new { e.id_cat_tipo_agente, nombre = e.id_cat_tipo_agente.ToString() + " - " + e.nombre }), "id_cat_tipo_agente", "nombre", estado_Fuerza_Detalle.id_cat_tipo_agente);
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(e => !e.eliminado).Select(e => new { e.id_ubicacion, direccion = e.id_ubicacion.ToString() + " - " + e.direccion }), "id_ubicacion", "direccion", estado_Fuerza_Detalle.id_ubicacion);
            ViewBag.fecha = estado_Fuerza_Detalle.Estado_Fuerza.fecha.ToString("dd/MM/yyyy");
            return View(estado_Fuerza_Detalle);
        }

        // POST: EstadoFuerza/Estado_Fuerza_Detalle/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Estado_Fuerza_Detalle estado_Fuerza_Detalle, DateTime fecha)
        {
            ModelState.Clear();
            Estado_Fuerza ef = db.Estado_Fuerza.SingleOrDefault(e=>e.fecha == fecha && !e.eliminado);
            if(ef == null)
            {
                return HttpNotFound();
            }
            //Validar que el registro no exista en el Estado de Fuerza
            var detalle_ef = ef.Estado_Fuerza_Detalle.SingleOrDefault(
                    e => e.activo && !e.eliminado && e.id_empleado == estado_Fuerza_Detalle.id_empleado
                        && e.id_estado_fuerza_detalle != estado_Fuerza_Detalle.id_estado_fuerza_detalle);
            if (detalle_ef != null)
            {
                ModelState.AddModelError("", "El registro ya existe en el Estado de Fuerza actual.");
            }
            if (ModelState.IsValid)
            {
                Estado_Fuerza_Detalle efd = db.Estado_Fuerza_Detalle.SingleOrDefault(e => e.activo && e.id_estado_fuerza_detalle == estado_Fuerza_Detalle.id_estado_fuerza_detalle);
                if (efd == null)
                {
                    return HttpNotFound();
                }
                efd.id_estado_fuerza = ef.id_estado_fuerza;
                efd.fecha = ef.fecha;
                efd.id_empleado = estado_Fuerza_Detalle.id_empleado;
                efd.id_situacion = estado_Fuerza_Detalle.id_situacion;
                efd.id_cat_tipo_agente = estado_Fuerza_Detalle.id_cat_tipo_agente;
                efd.id_ubicacion = estado_Fuerza_Detalle.id_ubicacion;
                efd.observacion = estado_Fuerza_Detalle.observacion;
                efd.fecha_modificacion = DateTime.Now;
                efd.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Entry(efd).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_situacion = new SelectList(db.Situacion.Where(e => !e.eliminado).Select(e => new { e.id_situacion, nombre = e.id_situacion.ToString() + " - " + e.nombre }), "id_situacion", "nombre", estado_Fuerza_Detalle.id_situacion);
            ViewBag.id_cat_tipo_agente = new SelectList(db.Cat_Tipos_Agente.Where(e => !e.eliminado).Select(e => new { e.id_cat_tipo_agente, nombre = e.id_cat_tipo_agente.ToString() + " - " + e.nombre }), "id_cat_tipo_agente", "nombre", estado_Fuerza_Detalle.id_cat_tipo_agente);
            ViewBag.id_ubicacion = new SelectList(db.Ubicaciones.Where(e => !e.eliminado).Select(e => new { e.id_ubicacion, direccion = e.id_ubicacion.ToString() + " - " + e.direccion }), "id_ubicacion", "direccion", estado_Fuerza_Detalle.id_ubicacion);
            return View(estado_Fuerza_Detalle);
        }

        // GET: EstadoFuerza/Estado_Fuerza_Detalle/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado_Fuerza_Detalle estado_Fuerza_Detalle = db.Estado_Fuerza_Detalle.SingleOrDefault(e => e.id_estado_fuerza_detalle == id && e.activo);
            if (estado_Fuerza_Detalle == null)
            {
                return HttpNotFound();
            }
            return View(estado_Fuerza_Detalle);
        }

        // POST: EstadoFuerza/Estado_Fuerza_Detalle/Delete/5
        [HttpPost]
        public ActionResult Eliminar(int id_estado_fuerza_detalle)
        {
            try
            {
                Estado_Fuerza_Detalle estado_Fuerza_Detalle = db.Estado_Fuerza_Detalle.SingleOrDefault(e => e.activo && e.id_estado_fuerza_detalle == id_estado_fuerza_detalle);
                if(estado_Fuerza_Detalle == null)
                {
                    return HttpNotFound();
                }
                estado_Fuerza_Detalle.activo = false;
                estado_Fuerza_Detalle.eliminado = true;
                estado_Fuerza_Detalle.fecha_eliminacion = DateTime.Now;
                estado_Fuerza_Detalle.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Entry(estado_Fuerza_Detalle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Error, "Error durante la conexión con el servidor. Cambios no efectuados.");
                msg.ReturnUrl = Url.Action("EstadoFuerza");
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
        }

        

        public ViewResult Mensaje()
        {
            ContextMessage msg = (ContextMessage)TempData[User.Identity.Name];
            return View(ContextMessage.ViewLocation(this), msg);
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
