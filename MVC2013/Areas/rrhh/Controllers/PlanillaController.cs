using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using LinqToExcel;
using System.IO;
using MVC2013.Src.Comun.Util;
using System.Globalization;
using MVC2013.Src.Sdc.Reports;
using System.Web.Configuration;
using MVC2013.Areas.rrhh.Models;
using MVC2013.Src.Comun.View;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace MVC2013.Areas.rrhh.Controllers
{
    public class PlanillaController : Controller
    {
        private AppEntities db = new AppEntities();

        #region Planilla

        [HttpPost]
        public ActionResult EliminarPlanilla (int id)
        {
            Encabezado_Planilla ep = db.Encabezado_Planilla.SingleOrDefault(e => !e.eliminado && e.id_encabezado_planilla == id);
            if(ep == null)
            {
                return HttpNotFound();
            }
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    db.sp_Eliminar_Planilla(id, Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario);
                    db.SaveChanges();
                    tran.Commit();
                    ContextMessage msg = new ContextMessage(ContextMessage.Success, "La planilla fue eliminada correctamente.");
                    msg.ReturnUrl = Url.Action("Index");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "No se pudo eliminar correctamente la planilla. Error: " + ex.Message);
                    msg.ReturnUrl = Url.Action("Index");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }
        }

        public ActionResult Index()
        {
            db.Database.CommandTimeout=1200;
            var encabezado = db.rpt_Resumen_Planillas_Activas();
            ViewBag.id_tipo_planilla = new SelectList(db.Tipo_Planilla.Where(e => !e.eliminado), "id_tipo_planilla", "nombre");
            ViewBag.id_empresa = new SelectList(db.Empresa.Where(e => !e.eliminado && e.Encabezado_Planilla.Where(a => a.activo && a.id_tipo_planilla < 3).Count() == 0), "id_empresa", "nombre");
            return View(encabezado);
        }

        public ActionResult Historico()
        {
            var encabezado = db.Encabezado_Planilla.Where(e => !e.eliminado && !e.activo);
            return View(encabezado);
        }

        public ActionResult Editar_Planilla(int id, int tab)
        {
            var empleado_encabezado = db.Empleado_Encabezado_Planilla.SingleOrDefault(e=>e.id_empleado_encabezado_planilla == id);
            EmpleadoPlanilla empleado_planilla = new EmpleadoPlanilla();
            empleado_planilla.empleado_encabezado = empleado_encabezado;
            empleado_planilla.planilla = null;
            ViewBag.empleado_nombre =
                empleado_encabezado.Empleado.primer_nombre + " " +
                (!String.IsNullOrEmpty(empleado_encabezado.Empleado.segundo_nombre) ? empleado_encabezado.Empleado.segundo_nombre + " " : "") +
                empleado_encabezado.Empleado.primer_apellido + " " +
                (!String.IsNullOrEmpty(empleado_encabezado.Empleado.segundo_apellido) ? empleado_encabezado.Empleado.segundo_apellido : "");

            ViewBag.tipos_bono = new SelectList(db.Tipo_Bono.Where(t => !t.eliminado), "id_tipo_bono", "nombre");
            ViewBag.tipos_descuento = new SelectList(db.Tipo_Descuento.Where(t => !t.eliminado), "id_tipo_descuento", "nombre");
            empleado_planilla.bonos = db.Obtener_Bonificaciones(id, empleado_encabezado.id_encabezado_planilla, empleado_encabezado.Encabezado_Planilla.id_tipo_planilla);
            empleado_planilla.descuentos = db.Obtener_Descuentos(id, empleado_encabezado.id_encabezado_planilla, empleado_encabezado.Encabezado_Planilla.id_tipo_planilla);
            decimal total_bonificacion = 0, total_descuento = 0;
            if (empleado_planilla.bonos.ToList().Count > 0)
            {
                total_bonificacion = empleado_planilla.bonos.Select(s => s.total).Sum();
            }
            if (empleado_planilla.descuentos.ToList().Count > 0)
            {
                total_descuento = empleado_planilla.descuentos.Select(e => e.total).Sum();
            }
            ViewBag.total_descuento = total_descuento.ToString("C", CultureInfo.GetCultureInfo("es-GT"));
            ViewBag.total_bonificacion = total_bonificacion.ToString("C", CultureInfo.GetCultureInfo("es-GT"));
            ViewBag.tab = tab;
            if (empleado_encabezado.Encabezado_Planilla.id_tipo_planilla < 3) // Primera y Segunda Quincena
            {
                decimal salario = empleado_encabezado.Contratacion.Salario.sueldo_base * empleado_encabezado.dias_trabajados / 30;
                ViewBag.salario = salario.ToString("C", CultureInfo.GetCultureInfo("es-GT"));
                ViewBag.total = (salario + total_bonificacion - total_descuento).ToString("C", CultureInfo.GetCultureInfo("es-GT"));
                if (empleado_encabezado.id_ubicacion.HasValue)
                {
                    ViewBag.id_ubicacion = empleado_encabezado.id_ubicacion.Value;
                }
                else
                {
                    ViewBag.id_ubicacion = 0;
                }
                ViewBag.ubicaciones = new SelectList(db.Ubicaciones.Where(u => !u.eliminado).Select(u => new { u.id_ubicacion, ubicacion = u.id_ubicacion.ToString() + " - " + u.direccion }), "id_ubicacion", "ubicacion", empleado_encabezado.id_ubicacion);
            }
            else
            {
                decimal? bonificacion = db.Calcular_Bono14_Aguinaldo(id, empleado_encabezado.id_encabezado_planilla).First();
                ViewBag.bonificacion = bonificacion.GetValueOrDefault(0).ToString("C", CultureInfo.GetCultureInfo("es-GT"));
                ViewBag.total = (bonificacion.GetValueOrDefault(0) + total_bonificacion - total_descuento).ToString("C", CultureInfo.GetCultureInfo("es-GT"));
            }
            //Enviar la forma de pago si la planilla ya se finalizo
            if(!empleado_encabezado.Encabezado_Planilla.activo)
            {
                empleado_planilla.planilla = db.Planilla.Where(p => p.activo && !p.eliminado && p.id_empleado_encabezado_planilla == empleado_encabezado.id_empleado_encabezado_planilla).First();
                ViewBag.tipo_pago = new SelectList(db.Tipo_Pago.Where(t => t.activo), "id_tipo_pago", "nombre", empleado_encabezado.id_tipo_pago);
                ViewBag.banco = new SelectList(db.Banco.Where(t => t.activo), "id_banco", "nombre", empleado_encabezado.id_banco);
            }
            ViewBag.acreditacion = (int)Catalogos.Tipo_Pago.Acreditacion;
            return View("Details", empleado_planilla);
        }

        [HttpPost]
        public ActionResult Editar_Ubicacion(Empleado_Encabezado_Planilla empleado_encabezado)
        {
            Empleado_Encabezado_Planilla ep = db.Empleado_Encabezado_Planilla.SingleOrDefault(e => e.id_empleado_encabezado_planilla == empleado_encabezado.id_empleado_encabezado_planilla);
            if (ep == null)
            {
                return HttpNotFound();
            }
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    //Si cambian los dias trabajados implica modificar el bono extra, decreto y el igss del empleado
                    int dias_trabajados = ep.dias_trabajados;
                    ep.dias_trabajados = empleado_encabezado.dias_trabajados;
                    if(ep.id_ubicacion != empleado_encabezado.id_ubicacion) //Modificar la ubicacion del contrato
                    {
                        ep.Contratacion.id_ubicacion = empleado_encabezado.id_ubicacion;
                        ep.Contratacion.fecha_modificacion = DateTime.Now;
                        ep.Contratacion.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        db.Entry(ep.Contratacion).State = EntityState.Modified;
                    }
                    ep.id_ubicacion = empleado_encabezado.id_ubicacion;
                    ep.fecha_modificacion = DateTime.Now;
                    ep.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(ep).State = EntityState.Modified;
                    db.SaveChanges();
                    //Recalcular Bono Extra, Decreto e Igss del Empleado en la Planilla
                    /*if (dias_trabajados != empleado_encabezado.dias_trabajados)
                    {
                        ActualizarEmpleadoPlanilla(ep);
                    }*/
                    tran.Commit();
                    return RedirectToAction("Editar_Planilla", new { id = empleado_encabezado.id_empleado_encabezado_planilla, tab = 0 });
                }
                catch
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "No se pudo guardar los cambios realizados.");
                    msg.ReturnUrl = Url.Action("Editar_Planilla", new { id = empleado_encabezado.id_empleado_encabezado_planilla, tab = 0 });
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }
        }

        public void ActualizarEmpleadoPlanilla(Empleado_Encabezado_Planilla ep)
        {
            //Bono Extra
            int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            var bono_extra = db.Bonificacion.SingleOrDefault(b => b.activo && b.id_empleado_encabezado_planilla == ep.id_empleado_encabezado_planilla && b.id_tipo_bono == (int)Catalogos.Tipo_Bono.Bono_Extra);
            if (bono_extra != null)
            {
                if(ep.Contratacion.Salario.bono_extra.HasValue)
                {
                    bono_extra.total = ep.Contratacion.Salario.bono_extra.Value * ep.dias_trabajados / 30;
                    bono_extra.fecha_modificacion = DateTime.Now;
                    bono_extra.id_usuario_modificacion = id_usuario;
                    db.Entry(bono_extra).State = EntityState.Modified;
                }
            }
            var bono_decreto = db.Bonificacion.SingleOrDefault(b => b.activo && b.id_empleado_encabezado_planilla == ep.id_empleado_encabezado_planilla && b.id_tipo_bono == (int)Catalogos.Tipo_Bono.Bono_Decreto);
            if (bono_decreto != null)
            {
                if (ep.Contratacion.Salario.bono_decreto.HasValue)
                {
                    bono_decreto.total = ep.Contratacion.Salario.bono_decreto.Value * ep.dias_trabajados / 30;
                    bono_decreto.fecha_modificacion = DateTime.Now;
                    bono_decreto.id_usuario_modificacion = id_usuario;
                    db.Entry(bono_decreto).State = EntityState.Modified;
                }
            }
            /*var igss = db.Descuento.SingleOrDefault(d => d.activo && d.id_empleado_encabezado_planilla == ep.id_empleado_encabezado_planilla && d.id_tipo_descuento == (int)Catalogos.Tipo_Descuento.Igss);
            if (igss != null)
            {
                CalcularIgss(ep);
            }*/
        }

        [HttpPost]
        public ActionResult EditarFormaPago(EmpleadoPlanilla empleado_planilla)
        {
            try
            {
                Empleado_Encabezado_Planilla empleado_encabezado = db.Empleado_Encabezado_Planilla.SingleOrDefault(e => e.id_empleado_encabezado_planilla == empleado_planilla.empleado_encabezado.id_empleado_encabezado_planilla);
                empleado_encabezado.id_tipo_pago = empleado_planilla.empleado_encabezado.id_tipo_pago;
                switch (empleado_encabezado.id_tipo_pago)
                {
                    case (int)Catalogos.Tipo_Pago.Acreditacion:
                        empleado_encabezado.id_banco = empleado_planilla.empleado_encabezado.id_banco;
                        empleado_encabezado.agencia = empleado_planilla.empleado_encabezado.agencia;
                        empleado_encabezado.cuenta = empleado_planilla.empleado_encabezado.cuenta;
                        empleado_encabezado.numero = empleado_planilla.empleado_encabezado.numero;
                        break;
                    case (int)Catalogos.Tipo_Pago.Cheque:
                        empleado_encabezado.id_banco = empleado_planilla.empleado_encabezado.id_banco;
                        empleado_encabezado.numero = empleado_planilla.empleado_encabezado.numero;
                        empleado_encabezado.agencia = null;
                        empleado_encabezado.cuenta = null;
                        break;
                    default:
                        empleado_encabezado.id_banco = null;
                        empleado_encabezado.agencia = null;
                        empleado_encabezado.cuenta = null;
                        empleado_encabezado.numero = null;
                        break;

                }
                empleado_encabezado.fecha_modificacion = DateTime.Now;
                empleado_encabezado.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Entry(empleado_encabezado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Editar_Planilla", new { id = empleado_planilla.empleado_encabezado.id_empleado_encabezado_planilla, tab = 0 });
            }
            catch
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Error, "La forma de pago no pudo ser modificada.");
                msg.ReturnUrl = Url.Action("Editar_Planilla", new { id = empleado_planilla.empleado_encabezado.id_empleado_encabezado_planilla, tab = 0 });
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
        }

        public ActionResult Ver_Planilla(int id)
        {
            Encabezado_Planilla encabezado_planilla = db.Encabezado_Planilla.SingleOrDefault(e=>e.id_encabezado_planilla == id);
            if(encabezado_planilla == null)
            {
                return HttpNotFound();
            }
            if (encabezado_planilla.activo) //Pre planilla
            {
                PlanillaDetalle planilladetalle = new PlanillaDetalle();
                planilladetalle.encabezado_planilla = encabezado_planilla;
                if(encabezado_planilla.id_tipo_planilla < 3) //Primera y Segunda Quincena
                {
                    planilladetalle.resumen_pre_planilla = db.Resumen_Empleados_Pre_Planilla(id);
                }
                else //Bono 14 y Aguinaldo
                {
                    planilladetalle.resumen_bono_aguinaldo = db.Resumen_Planilla_Bono_Aguinaldo(id);
                }
                return View("Resumen_Pre_Planilla", planilladetalle);
            }
            else //Planilla Finalizada
            {
                PlanillaDetalle planilladetalle = new PlanillaDetalle();
                planilladetalle.encabezado_planilla = encabezado_planilla;
                planilladetalle.resumen_planilla = db.Resumen_Empleados_Planilla(id);
                return View("Resumen_Planilla_Final", planilladetalle);
            }
        }


        [HttpPost]
        public ActionResult Generar_Planilla(int id_empresa, int id_tipo_planilla, DateTime fechaInicioEstadoFuerza, DateTime fechaFinEstadoFuerza)
        {
            ModelState.Clear();
            int dias_previos = int.Parse(db.Parametros_Sistema.Find(19).valor);
            var encabezado_planilla = db.Encabezado_Planilla.Where(e => !e.eliminado && e.id_empresa == id_empresa && id_tipo_planilla == e.id_tipo_planilla && e.fecha_apertura.Month == DateTime.Now.Month && e.fecha_apertura.Year == DateTime.Now.Year).ToList();
            if (encabezado_planilla.Count > 0)
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Info, "La planilla seleccionada ya ha sido creada previamente.");
                msg.ReturnUrl = Url.Action("Index");
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
                //ModelState.AddModelError("", "La planilla seleccionada ya ha sido creada previamente.");
            }
            else
            {
                /*switch (id_tipo_planilla)
                {
                    case 1: //Primera quincena
                        if (!(DateTime.Now.Day > (15 - dias_previos) && DateTime.Now.Day < 16))
                        {
                            ModelState.AddModelError("", "Todavia no es disponible generar la primera quincena de " + ObtenerMes(DateTime.Now.Month));
                        }
                        break;
                    case 2: //Segunda quincena
                        var dias_en_mes = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                        if (!(DateTime.Now.Day > (dias_en_mes - dias_previos) && DateTime.Now.Day < dias_en_mes + 1))
                        {
                            ModelState.AddModelError("", "Todavia no es disponible generar la segunda quincena de " + ObtenerMes(DateTime.Now.Month));
                        }
                        break;
                }*/
            }
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    if (id_tipo_planilla < 3) //Primera y Segunda Quincena
                    {

                        db.Guardar_Detalle_Planilla(id_empresa, id_tipo_planilla, Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario,
                            fechaInicioEstadoFuerza, fechaFinEstadoFuerza);
                        tran.Commit();
                    }
                    if (id_tipo_planilla > 2) //Bono14 y Aguinaldo
                    {
                        db.Generar_Planilla_Aguinaldo_Bono(id_empresa, id_tipo_planilla, Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario);
                        tran.Commit();
                        
                    }
                    return RedirectToAction("Index");
                }
                catch
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "No se pudo generar correctamente la planilla.");
                    msg.ReturnUrl = Url.Action("Index");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }
        }

        [HttpGet]
        public JsonResult GetFechaInicio_EstadoFuerza(int id_empresa)
        {
            var planilla = db.Encabezado_Planilla.Where(e => !e.activo && !e.eliminado && e.id_empresa == id_empresa && e.id_tipo_planilla < 3).OrderByDescending(e => e.fecha_creacion).FirstOrDefault();
            if (planilla == null)
            {
                return Json(new { fecha = "No determinada" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { fecha = planilla.fechaFin_EstadoFuerza.Value.AddDays(1).ToString("dd/MM/yyyy") }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetFecha_TipoPlanilla(int id_tipo_planilla)
        {
            string fechaInicio, fechaFin;
            switch (id_tipo_planilla)
            {
                case (int)Catalogos.Tipo_Planilla.Primera_Quincena:
                    fechaInicio = "01/" + DateTime.Now.ToString("MM/yyyy");
                    fechaFin = "15/" + DateTime.Now.ToString("MM/yyyy");
                    break;
                case (int)Catalogos.Tipo_Planilla.Segunda_Quincena:
                    fechaInicio = "16/" + DateTime.Now.ToString("MM/yyyy");
                    fechaFin = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month).ToString() + "/" + DateTime.Now.ToString("MM/yyyy");
                    break;
                case (int)Catalogos.Tipo_Planilla.Bono_14:
                    fechaInicio = "Julio - " + (DateTime.Now.Year - 1).ToString();
                    fechaFin = "Julio - " + (DateTime.Now.Year).ToString();
                    break;
                case (int)Catalogos.Tipo_Planilla.Aguinaldo:
                    fechaInicio = "Diciembre - " + (DateTime.Now.Year - 1).ToString();
                    fechaFin = "Diciembre - " + (DateTime.Now.Year).ToString();
                    break;
                default:
                    fechaInicio = ""; fechaFin = "";
                    break;
            }
            return Json(new { fechaI = fechaInicio, fechaF = fechaFin }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Finalizar_planilla(int id)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                Encabezado_Planilla encabezado = db.Encabezado_Planilla.Find(id);
                try
                {
                    encabezado.fecha_modificacion = DateTime.Now;
                    encabezado.fecha_finalizacion = DateTime.Now;
                    int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    encabezado.id_usuario_modificacion = id_usuario;
                    encabezado.activo = false;
                    db.Entry(encabezado).State = EntityState.Modified;
                    db.Finalizar_Planilla(id, id_usuario, encabezado.id_tipo_planilla);
                    db.SaveChanges();
                    tran.Commit();
                    return Json(new { response = true });
                }
                catch 
                {
                    tran.Rollback();
                    return Json(new { response = false, msg = "Cambios no guardados." });
                }
            }
        }

        public ActionResult Generar_Planilla()
        {
            return RedirectToAction("Index");
        }
        #endregion

        #region Eliminacion de Cargas a Planilla

        [HttpPost]
        public ActionResult EliminarBonificacionesCargadas(int id, int id_tipo_bono)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    db.EliminarBonificacionesCargadas(id, Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario, id_tipo_bono);
                    tran.Commit();
                    ContextMessage msg = new ContextMessage(ContextMessage.Success, "Se eliminaron las bonificaciones cargadas seleccionadas");
                    msg.ReturnUrl = Url.Action("Index");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
                catch
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "No se pudieron eliminar las bonificaciones seleccionadas");
                    msg.ReturnUrl = Url.Action("Index");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }
        }

        [HttpPost]
        public ActionResult EliminarDescuentosCargados(int id, int id_tipo_descuento)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    db.EliminarDescuentosCargados(id, Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario, id_tipo_descuento);
                    tran.Commit();
                    ContextMessage msg = new ContextMessage(ContextMessage.Success, "Se eliminaron los descuentos seleccionados.");
                    msg.ReturnUrl = Url.Action("Index");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
                catch
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "No se pudieron eliminar los descuentos seleccionados.");
                    msg.ReturnUrl = Url.Action("Index");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }
        }

        #endregion

        #region Modificación de Planilla

        public ActionResult Modificacion(int id)
        {
            var encabezado_planilla = db.Encabezado_Planilla.SingleOrDefault(e => e.id_encabezado_planilla == id && !e.eliminado);
            if (encabezado_planilla == null)
            {
                return HttpNotFound();
            }
            return View(encabezado_planilla);
        }

        [HttpGet]
        public JsonResult AnalizarCampo(int campo, string texto, int id_planilla)
        {
            List<object> resultado = new List<object>();
            List<int> lista_empleados = db.Encabezado_Planilla.SingleOrDefault(e => e.id_encabezado_planilla == id_planilla).Empleado_Encabezado_Planilla.Where(e => e.activo).Select(s => s.id_empleado).ToList();
            List<int> empleados_encontrados = new List<int>();
            int rCorrectos = 0, rIncorrectos = 0;
            //Dias Trabajados
            if (campo == 1)
            {
                string[] lineas = texto.Split('\n');
                int total = 0;
                foreach (var dato in lineas)
                {
                    if (String.IsNullOrEmpty(dato)) continue;
                    int id_empleado = 0;
                    int valor = 0;
                    string error = "";
                    bool estado = true;
                    if (dato.Split('\t').Count() != 2)
                    {
                        estado = false;
                        error = "Línea no pudo ser analizada.";
                        resultado.Add(new { id_empleado = 0, valor = 0, estado = estado, error = error });
                        continue;
                    }
                    //Verificar empleado
                    if (int.TryParse(dato.Split('\t')[0], out id_empleado))
                    {
                        if (empleados_encontrados.Contains(id_empleado))
                        {
                            error = "Ya existe un registro con ese empleado.";
                            estado = false;
                        }
                        else if (!lista_empleados.Contains(id_empleado))
                        {
                            error = "Empleado no existe en la planilla. ";
                            estado = false;
                        }
                        else
                        {
                            empleados_encontrados.Add(id_empleado);
                        }
                    }
                    else
                    {
                        error = "El formato del código de empleado es incorrecto.\n";
                        estado = false;
                    }
                    //Verificar valor
                    if (!int.TryParse(dato.Split('\t')[1], out valor))
                    {
                        error += "El formato del valor es incorrecto.\n";
                        estado = false;
                    }
                    if (estado)
                    {
                        rCorrectos++;
                        total += valor;
                        resultado.Insert(resultado.Count, new { id_empleado = id_empleado, valor = valor, estado = estado, error = error });
                    }
                    else
                    {
                        rIncorrectos++;
                        resultado.Insert(0, new { id_empleado = id_empleado, valor = valor, estado = estado, error = error });
                    }
                }
                return Json(new { resultado = resultado, rCorr = rCorrectos, rInc = rIncorrectos, tot = total }, JsonRequestBehavior.AllowGet);
            }
            //Bonificaciones y Descuentos
            else
            {
                string[] lineas = texto.Split('\n');
                decimal total = 0;
                foreach (var dato in lineas)
                {
                    if (String.IsNullOrEmpty(dato)) continue;
                    int id_empleado = 0;
                    decimal valor = 0;
                    string error = "";
                    bool estado = true;
                    if (dato.Split('\t').Count() != 2)
                    {
                        estado = false;
                        error = "Línea no pudo ser analizada.";
                        resultado.Add(new { id_empleado = 0, valor = 0, estado = estado, error = error });
                        continue;
                    }
                    //Verificar empleado
                    if (int.TryParse(dato.Split('\t')[0], out id_empleado))
                    {
                        if (empleados_encontrados.Contains(id_empleado))
                        {
                            error = "Ya existe un registro con ese empleado. ";
                            estado = false;
                        }
                        else if (!lista_empleados.Contains(id_empleado))
                        {
                            error += "Empleado no existe en la planilla. ";
                            estado = false;
                        }
                        else
                        {
                            empleados_encontrados.Add(id_empleado);
                        }
                    }
                    else
                    {
                        error = "El formato del código de empleado es incorrecto.\n";
                        estado = false;
                    }
                    //Verificar valor
                    if (!decimal.TryParse(dato.Split('\t')[1], NumberStyles.Any, CultureInfo.InvariantCulture, out valor))
                    {
                        error += "El formato del valor es incorrecto.\n";
                        estado = false;
                    }
                    if (estado)
                    {
                        rCorrectos++;
                        total += Math.Round(valor, 2);
                        resultado.Insert(resultado.Count, (new { id_empleado = id_empleado, valor = Math.Round(valor, 2), estado = estado, error = error }));
                    }
                    else
                    {
                        rIncorrectos++;
                        resultado.Insert(0, (new { id_empleado = id_empleado, valor = Math.Round(valor, 2), estado = estado, error = error }));
                    }

                }
                return Json(new { resultado = resultado, rCorr = rCorrectos, rInc = rIncorrectos, tot = total }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GuardarResultados(int tipo_campo, bool[] estado_registro, int id)
        {

            string[] id_empleado = Request["id_empleado"].Split(',');
            string[] valor = Request["valor"].Split(',');
            if (tipo_campo == 1) //Dias Trabajados
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        for (int i = 0; i < estado_registro.Length; i++)
                        {
                            int empleado = int.Parse(id_empleado[i]);
                            if (!estado_registro[i]) continue;//Obviar registros incorrectos
                            Empleado_Encabezado_Planilla ep = db.Empleado_Encabezado_Planilla.SingleOrDefault(e => e.activo && !e.eliminado && e.id_empleado == empleado && e.id_encabezado_planilla == id);
                            if (ep == null) continue;
                            ep.dias_trabajados = Convert.ToInt32(valor[i], CultureInfo.InvariantCulture);
                            ep.fecha_modificacion = DateTime.Now;
                            ep.id_usuario_modificacion = id_usuario;
                            db.Entry(ep).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        tran.Commit();
                        ContextMessage msg = new ContextMessage(ContextMessage.Success, "Las modificaciones en planilla se guardaron correctamente.");
                        msg.ReturnUrl = Url.Action("Modificacion", new { id = id });
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        ContextMessage msg = new ContextMessage(ContextMessage.Error, "Las modificaciones en planilla no pudieron ser guardadas. Error: " + ex.Message);
                        msg.ReturnUrl = Url.Action("Modificacion", new { id = id });
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                }
            }
            //Horas Extras      Boleta Extras
            else if (tipo_campo == 2 || tipo_campo == 3) //Bonificaciones
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        int tipo_bono = 0;
                        tipo_bono = (tipo_campo == 2) ? tipo_bono = 1 : tipo_bono = 4;
                        for (int i = 0; i < estado_registro.Length; i++)
                        {
                            int empleado = int.Parse(id_empleado[i]);
                            if (!estado_registro[i]) continue;//Obviar registros incorrectos
                            Empleado_Encabezado_Planilla ep = db.Empleado_Encabezado_Planilla.SingleOrDefault(e => e.activo && !e.eliminado && e.id_empleado == empleado && e.id_encabezado_planilla == id);
                            if (ep == null) continue;
                            ep.Bonificacion.Where(e => e.activo && !e.eliminado && e.id_tipo_bono == tipo_bono).ToList().ForEach(e => { e.activo = false; e.eliminado = true; e.fecha_eliminacion = DateTime.Now; e.id_usuario_eliminacion = id_usuario; db.Entry(e).State = EntityState.Modified; });
                            decimal total = Convert.ToDecimal(valor[i], CultureInfo.InvariantCulture);
                            if (total > 0)
                            {
                                Bonificacion bono = new Bonificacion();
                                bono.activo = true;
                                bono.automatico = false;
                                bono.eliminado = false;
                                bono.id_empleado = ep.id_empleado;
                                bono.total = total;
                                bono.fecha_creacion = bono.fecha = DateTime.Now;
                                bono.id_usuario_creacion = id_usuario;
                                bono.id_tipo_bono = tipo_bono;
                                bono.id_empleado_encabezado_planilla = ep.id_empleado_encabezado_planilla;
                                bono.fecha = DateTime.Now;
                                bono.cantidad = 1;
                                db.Bonificacion.Add(bono);
                            }
                        }
                        db.SaveChanges();
                        tran.Commit();
                        ContextMessage msg = new ContextMessage(ContextMessage.Success, "Las modificaciones en planilla se guardaron correctamente.");
                        msg.ReturnUrl = Url.Action("Modificacion", new { id = id });
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        ContextMessage msg = new ContextMessage(ContextMessage.Error, "Las modificaciones en planilla no pudieron ser guardadas. Error: " + ex.Message);
                        msg.ReturnUrl = Url.Action("Modificacion", new { id = id });
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                }
            }
            else //Descuentos
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        int tipo_descuento = tipo_campo - 2;
                        for (int i = 0; i < estado_registro.Length; i++)
                        {
                            int empleado = int.Parse(id_empleado[i]);
                            if (!estado_registro[i]) continue;//Obviar registros incorrectos
                            Empleado_Encabezado_Planilla ep = db.Empleado_Encabezado_Planilla.SingleOrDefault(e => e.activo && !e.eliminado && e.id_empleado == empleado && e.id_encabezado_planilla == id);
                            if (ep == null) continue;
                            ep.Descuento.Where(e => e.activo && !e.eliminado && e.id_tipo_descuento == tipo_descuento).ToList().ForEach(e => { e.activo = false; e.eliminado = true; e.fecha_eliminacion = DateTime.Now; e.id_usuario_eliminacion = id_usuario; db.Entry(e).State = EntityState.Modified; });
                            decimal total = Convert.ToDecimal(valor[i], CultureInfo.InvariantCulture);
                            if (total > 0)
                            {
                                Descuento descuento = new Descuento();
                                descuento.activo = true;
                                descuento.eliminado = false;
                                descuento.automatico = false;
                                descuento.total = total;
                                descuento.id_empleado = ep.id_empleado;
                                descuento.fecha_creacion = descuento.fecha = DateTime.Now;
                                descuento.id_usuario_creacion = id_usuario;
                                descuento.id_tipo_descuento = tipo_descuento;
                                descuento.id_empleado_encabezado_planilla = ep.id_empleado_encabezado_planilla;
                                descuento.fecha = DateTime.Now;
                                descuento.cantidad = 1;
                                db.Descuento.Add(descuento);
                            }
                        }
                        db.SaveChanges();
                        tran.Commit();
                        ContextMessage msg = new ContextMessage(ContextMessage.Success, "Las modificaciones en planilla se guardaron correctamente.");
                        msg.ReturnUrl = Url.Action("Modificacion", new { id = id });
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        ContextMessage msg = new ContextMessage(ContextMessage.Error, "Las modificaciones en planilla no pudieron ser guardadas. Error: " + ex.Message);
                        msg.ReturnUrl = Url.Action("Modificacion", new { id = id });
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                }
            }
        }

        #endregion     

        #region Mantenimiento Bonificaciones

        [HttpPost]
        public ActionResult Agregar_Bonificacion(int tipos_bono, int cantidad, DateTime fecha, string total, int id_empleado_encabezado_planilla)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Bonificacion bonificacion = NuevaBonificacion();
                    bonificacion.cantidad = cantidad;
                    bonificacion.id_tipo_bono = tipos_bono;
                    bonificacion.fecha = fecha;
                    bonificacion.automatico = false;
                    bonificacion.total = Convert.ToDecimal(total, CultureInfo.InvariantCulture);
                    bonificacion.id_empleado_encabezado_planilla = id_empleado_encabezado_planilla;
                    Empleado_Encabezado_Planilla ep = db.Empleado_Encabezado_Planilla.Find(id_empleado_encabezado_planilla);
                    bonificacion.id_empleado = ep.id_empleado;
                    db.Bonificacion.Add(bonificacion);
                    db.SaveChanges();
                    tran.Commit();
                }
                catch
                {
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "La bonificación no pudo ser guardada.");
                    msg.ReturnUrl = Url.Action("Editar_Planilla", new { id = id_empleado_encabezado_planilla, tab = 1 });
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
                return RedirectToAction("Editar_Planilla", new { id = id_empleado_encabezado_planilla, tab = 1 });
            }
        }


        [HttpPost]
        public ActionResult Editar_Bonificacion(int id_bonificacion, int tipos_bono, int cantidad, DateTime fecha, string total)
        {
            Bonificacion bonificacion = db.Bonificacion.Find(id_bonificacion);
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    bonificacion.id_tipo_bono = tipos_bono;
                    bonificacion.total = Convert.ToDecimal(total, CultureInfo.InvariantCulture);
                    bonificacion.fecha = fecha;
                    bonificacion.cantidad = cantidad;
                    bonificacion.fecha_modificacion = DateTime.Now;
                    bonificacion.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(bonificacion).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "La bonificación no pudo ser guardada.");
                    msg.ReturnUrl = Url.Action("Editar_Planilla", new { id = bonificacion.id_empleado_encabezado_planilla, tab = 1 });
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }
            return RedirectToAction("Editar_Planilla", new { id = bonificacion.id_empleado_encabezado_planilla, tab = 1 });
        }

        [HttpPost]
        public JsonResult Eliminar_Bonificacion(int id)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Bonificacion bonificacion = db.Bonificacion.Find(id);
                    bonificacion.fecha_eliminacion = DateTime.Now;
                    bonificacion.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    bonificacion.activo = false;
                    bonificacion.eliminado = true;
                    db.Entry(bonificacion).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return Json(new { msg = "", response = true });
                }
                catch
                {
                    tran.Rollback();
                    return Json(new { msg = "Cambios no guardados.", response = false });
                }
            }
        }
        #endregion

        #region Mantenimiento Descuentos
        [HttpPost]
        public ActionResult Agregar_Descuento(int tipos_descuento, DateTime fecha, string total, int id_empleado_encabezado_planilla, int? id_prestamo, int? cantidad)
        {
            ContextMessage msg;
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Descuento descuento = NuevoDescuento();
                    descuento.id_tipo_descuento = tipos_descuento;
                    descuento.fecha = fecha;
                    descuento.cantidad = cantidad;
                    descuento.automatico = false;
                    descuento.total = Convert.ToDecimal(total, CultureInfo.InvariantCulture);
                    descuento.id_empleado_encabezado_planilla = id_empleado_encabezado_planilla;
                    Empleado_Encabezado_Planilla ep = db.Empleado_Encabezado_Planilla.Find(id_empleado_encabezado_planilla);
                    descuento.id_empleado = ep.id_empleado;
                    /*Tipo_Descuento tipo_descuento = db.Tipo_Descuento.Find(tipos_descuento);
                    if (descuento.id_tipo_descuento == (int)Catalogos.Tipo_Descuento.Prestamo || id_prestamo.HasValue)
                    {
                        if (descuento.id_tipo_descuento == (int)Catalogos.Tipo_Descuento.Prestamo && id_prestamo.HasValue)
                        {
                            Descuento_Periodico prestamo = db.Descuento_Periodico.Find(id_prestamo);
                            if (prestamo != null)
                            {
                                Empleado_Encabezado_Planilla empleado_encabezado = db.Empleado_Encabezado_Planilla.Find(id_empleado_encabezado_planilla);
                                if (prestamo.id_empleado != empleado_encabezado.id_empleado)
                                {
                                    msg = new ContextMessage(ContextMessage.Error, "El formato del descuento es incorrecto.");
                                    tran.Rollback();
                                }
                                else
                                {
                                    descuento.id_descuento_periodico = id_prestamo.Value;
                                    db.Descuento.Add(descuento);
                                    db.SaveChanges();
                                    tran.Commit();
                                    return RedirectToAction("Editar_Planilla", new { id = id_empleado_encabezado_planilla, tab = 2 });
                                }
                            }
                            else
                            {
                                msg = new ContextMessage(ContextMessage.Error, "El formato del descuento es incorrecto.");
                                tran.Rollback();
                            }
                        }
                        else
                        {
                            msg = new ContextMessage(ContextMessage.Error, "El formato del descuento es incorrecto.");
                            tran.Rollback();
                        }
                    }
                    else*/
                    //{
                        db.Descuento.Add(descuento);
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Editar_Planilla", new { id = id_empleado_encabezado_planilla, tab = 2 });
                    //}
                }
                catch
                {
                    msg = new ContextMessage(ContextMessage.Error, "El descuento no pudo ser guardado.");
                    tran.Rollback();
                }
            }
            msg.ReturnUrl = Url.Action("Editar_Planilla", new { id = id_empleado_encabezado_planilla, tab = 2 });
            TempData[User.Identity.Name] = msg;
            return RedirectToAction("Mensaje");
        }

        [HttpPost]
        public ActionResult Editar_Descuento(int id_descuento, int? prestamo, int tipos_descuento, DateTime fecha, string total, int? cantidad)
        {
            ContextMessage msg;
            Descuento descuento = db.Descuento.Find(id_descuento);
            Tipo_Descuento tipo_descuento = db.Tipo_Descuento.Find(tipos_descuento);
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    descuento.id_tipo_descuento = tipos_descuento;
                    descuento.total = Convert.ToDecimal(total, CultureInfo.InvariantCulture);
                    descuento.fecha = fecha;
                    descuento.cantidad = cantidad;
                    descuento.fecha_modificacion = DateTime.Now;
                    descuento.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    //if (descuento.id_tipo_descuento == (int)Catalogos.Tipo_Descuento.Prestamo) //
                    /*{
                        if (prestamo.HasValue) //
                        {
                            Descuento_Periodico descuento_periodico = db.Descuento_Periodico.Find(prestamo.Value);
                            Empleado_Encabezado_Planilla empleado_encabezado = db.Empleado_Encabezado_Planilla.Find(descuento.id_empleado_encabezado_planilla);
                            if (descuento_periodico != null)
                            {
                                if (descuento_periodico.id_empleado == empleado_encabezado.id_empleado)
                                {
                                    descuento.id_descuento_periodico = prestamo.Value;
                                    db.Entry(descuento).State = EntityState.Modified;
                                    db.SaveChanges();
                                    tran.Commit();
                                    return RedirectToAction("Editar_Planilla", new { id = descuento.id_empleado_encabezado_planilla, tab = 2 });
                                }
                                else
                                {
                                    msg = new ContextMessage(ContextMessage.Error, "El formato del descuento es incorrecto.");
                                    tran.Rollback();
                                }
                            }
                            else
                            {
                                msg = new ContextMessage(ContextMessage.Error, "El formato del descuento es incorrecto..");
                            }
                        }
                        else
                        {
                            msg = new ContextMessage(ContextMessage.Error, "El formato del descuento es incorrecto.");
                            tran.Rollback();
                        }*/
                    //}
                    //else
                    //{
                        descuento.id_descuento_periodico = null;
                        db.Entry(descuento).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Editar_Planilla", new { id = descuento.id_empleado_encabezado_planilla, tab = 2 });
                    //}
                }
                catch
                {
                    msg = new ContextMessage(ContextMessage.Error, "El descuento no pudo ser guardado.");
                    tran.Rollback();
                }
            }
            msg.ReturnUrl = Url.Action("Editar_Planilla", new { id = descuento.id_empleado_encabezado_planilla, tab = 2 });
            TempData[User.Identity.Name] = msg;
            return RedirectToAction("Mensaje");
        }

        [HttpPost]
        public JsonResult Eliminar_Descuento(int id)
        {
            Descuento descuento = db.Descuento.Find(id);
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    if(descuento.id_descuento_periodico.HasValue)
                    {
                        descuento.id_empleado_encabezado_planilla = null;
                        descuento.fecha_modificacion = DateTime.Now;
                        descuento.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    }
                    else
                    {
                        descuento.activo = false;
                        descuento.eliminado = true;
                        descuento.fecha_eliminacion = DateTime.Now;
                        descuento.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    }
                    db.Entry(descuento).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return Json(new { msg = "", response = true });
                }
                catch
                {
                    tran.Rollback();
                    return Json(new { msg = "Cambios no guardados.", response = false });
                }
            }
        }
        #endregion

        #region Subir Archivos de Bonificacion, Descuento, Dias trabajados y Tipo de Pago
        [HttpPost]
        public ActionResult Subir_Correlativo_Boleta(HttpPostedFileBase file, int id)
        {
            int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            var filepath = Server.MapPath("~/Archivos/Temp/" + id_usuario.ToString() + "_correlativo_" + file.FileName);
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            file.SaveAs(filepath);
            var excel = new ExcelQueryFactory(filepath);
            var worksheetNames = excel.GetWorksheetNames();
            string errores = "";
            string error_especial = "";
            int no_fila = 0;
            bool errores_detectados = false;
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var sheet = worksheetNames.First();
                    no_fila = 1;
                    errores += "Errores en descuentos, filas número: ";
                    error_especial = "Error fatal en boletas, fila: ";
                    var worksheet = from a in excel.Worksheet(sheet) select a;
                    foreach (var a in worksheet)
                    {
                        string codigo_empleado = a["Codigo Empleado"];
                        string correlativo = a["Correlativo Boleta"];
                        //Ingorar lineas en blanco
                        if (String.IsNullOrEmpty(codigo_empleado) && String.IsNullOrEmpty(correlativo))
                        {
                            continue;
                        }
                        int id_empleado = Convert.ToInt32(codigo_empleado);
                        int correlativo_boleta = Convert.ToInt32(correlativo);

                        var empleado_encabezado = db.Empleado_Encabezado_Planilla.SingleOrDefault(e => e.id_empleado == id_empleado && e.id_encabezado_planilla == id);
                        if (empleado_encabezado == null)
                        {
                            errores += no_fila.ToString() + ", ";
                            errores_detectados = true;
                            continue;
                        }
                        empleado_encabezado.correlativo_boleta = correlativo_boleta;
                        db.Entry(empleado_encabezado).State = EntityState.Modified;
                        db.SaveChanges();
                        no_fila++;
                    }
                }
                catch
                {
                    tran.Rollback();
                    error_especial += no_fila.ToString();
                    System.IO.File.Delete(filepath);
                    return Json(new { response = false, error = error_especial });
                }
                tran.Commit();
                System.IO.File.Delete(filepath);
                return Json(new { response = true, error_detectado = errores_detectados, errores = errores });
            }
        }

        [HttpPost]
        public ActionResult Subir_Descuentos(HttpPostedFileBase file, int id)
        {
            int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            var filepath = Server.MapPath("~/Archivos/Temp/" + id_usuario.ToString() + "_descuento_" + file.FileName);
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            file.SaveAs(filepath);
            var excel = new ExcelQueryFactory(filepath);
            var worksheetNames = excel.GetWorksheetNames();
            try
            {
                var sheet = worksheetNames.First();
                var worksheet = from a in excel.Worksheet(sheet) select a;
                List<object> resultados = new List<object>();
                List<int> lista_empleados = db.Encabezado_Planilla.SingleOrDefault(e => e.id_encabezado_planilla == id).Empleado_Encabezado_Planilla.Where(e => e.activo).Select(s => s.id_empleado).ToList();
                List<int> empleados_encontrados = new List<int>();
                decimal total_descuento = 0;
                int rCorrectos = 0, rIncorrectos = 0;
                foreach (var a in worksheet)
                {
                    string error = "", descuento = "";
                    bool estado = true;
                    int id_empleado = 0, id_tipo_descuento = 0, unidades = 0;
                    decimal total = 0;
                    DateTime fecha = DateTime.Now;
                    //Ignorar lineas en blanco
                    if (String.IsNullOrEmpty(a["Código Empleado"]) && String.IsNullOrEmpty(a["Tipo de Descuento"]) && String.IsNullOrEmpty(a["Código Préstamo"]) && String.IsNullOrEmpty(a["Cantidad"]) && String.IsNullOrEmpty(a["Fecha"]) && String.IsNullOrEmpty(a["Total"]))
                    {
                        continue;
                    }
                    if (!int.TryParse(a["Código Empleado"], out id_empleado))
                    {
                        error += "Formato de empleado incorrecto; ";
                        estado = false;
                    }
                    else
                    {
                        if (empleados_encontrados.Contains(id_empleado))
                        {
                            error += "Ya existe un registro con ese empleado; ";
                            estado = false;
                        }
                        else if (!lista_empleados.Contains(id_empleado))
                        {
                            error += "Empleado no existe en la planilla; ";
                            estado = false;
                        }
                        else
                        {
                            empleados_encontrados.Add(id_empleado);
                        }
                    }
                    if (!int.TryParse(a["Tipo de Descuento"], out id_tipo_descuento))
                    {
                        error += "Formato de descuento incorrecto; ";
                        estado = false;
                    }
                    else
                    {
                        Tipo_Descuento Tipo_Descuento = db.Tipo_Descuento.Find(id_tipo_descuento);
                        if (Tipo_Descuento == null)
                        {
                            error += "Descuento no encontrado; ";
                            estado = false;
                        }
                        else
                        {
                            descuento = Tipo_Descuento.nombre;
                        }
                    }
                    if (!int.TryParse(a["Cantidad"], out unidades))
                    {
                        error += "Formato de cantidad incorrrecta; ";
                        estado = false;
                    }
                    if (!(decimal.TryParse(a["Total"], out total)))
                    {
                        error += "El total tiene formato incorrecto; ";
                        estado = false;
                    }
                    if (!DateTime.TryParse(a["Fecha"], out fecha))
                    {
                        error += "Fecha con formato incorrecto; ";
                        estado = false;
                    }
                    //Registro correcto
                    if (estado)
                    {
                        rCorrectos++;
                        total_descuento += Math.Round(total, 2);
                        resultados.Insert(resultados.Count, new { id_descuento = id_tipo_descuento, id_empleado = id_empleado, descuento = descuento, unidades = unidades, fecha = fecha.ToString("dd/MM/yyyy"), estado = estado, error = "", total = Math.Round(total, 2) });
                    }
                    else
                    {
                        rIncorrectos++;
                        resultados.Insert(0, new { id_descuento = 0, id_empleado = a["Código Empleado"].Value, descuento = a["Tipo de Descuento"].Value.ToString(), unidades = a["Cantidad"].Value, fecha = a["Fecha"].Value.ToString(), estado = estado, error = error, total = a["Total"].Value });
                    }
                }
                System.IO.File.Delete(filepath);
                return Json(new { resultados, mensaje = "Registros Correctos: " + rCorrectos.ToString() + " - Registros Incorrectos: " + rIncorrectos.ToString() + " - Descuento Ingresado: " + total_descuento.ToString("C", CultureInfo.GetCultureInfo("es-GT")) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                System.IO.File.Delete(filepath);
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Subir_Bonificaciones(HttpPostedFileBase file, int id)
        {
            int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            var filepath = Server.MapPath("~/Archivos/Temp/" + id_usuario.ToString() + "_bonificacion_" + file.FileName);
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            file.SaveAs(filepath);
            var excel = new ExcelQueryFactory(filepath);
            var worksheetNames = excel.GetWorksheetNames();
            try
            {
                var sheet = worksheetNames.First();
                var worksheet = from a in excel.Worksheet(sheet) select a;
                List<object> resultados = new List<object>();
                List<int> lista_empleados = db.Encabezado_Planilla.SingleOrDefault(e => e.id_encabezado_planilla == id).Empleado_Encabezado_Planilla.Where(e => e.activo).Select(s => s.id_empleado).ToList();
                List<int> empleados_encontrados = new List<int>();
                decimal tota_bonificacion = 0;
                int rCorrectos = 0, rIncorrectos = 0;
                foreach (var a in worksheet)
                {
                    string error = "";
                    bool estado = true;
                    string bono = "";
                    int id_empleado = 0, id_tipo_bono = 0, unidades = 0;
                    DateTime fecha = DateTime.Now;
                    decimal total;
                    //Ingorar lineas en blanco
                    if (String.IsNullOrEmpty(a["Código Empleado"]) && String.IsNullOrEmpty(a["Tipo de Bono"]) && String.IsNullOrEmpty(a["Unidades"]) && String.IsNullOrEmpty(a["Total"]) && String.IsNullOrEmpty(a["Fecha"]))
                    {
                        continue;
                    }
                    if (!int.TryParse(a["Código Empleado"], out id_empleado))
                    {
                        error += "Formato de empleado incorrecto; ";
                        estado = false;
                    }
                    else
                    {
                        if (empleados_encontrados.Contains(id_empleado))
                        {
                            error += "Ya existe un registro con ese empleado; ";
                            estado = false;
                        }
                        else if (!lista_empleados.Contains(id_empleado))
                        {
                            error += "Empleado no existe en la planilla; ";
                            estado = false;
                        }
                        else
                        {
                            empleados_encontrados.Add(id_empleado);
                        }
                    }
                    if (!int.TryParse(a["Tipo de Bono"], out id_tipo_bono))
                    {
                        error += "Formato de bonificación incorrecta; ";
                        estado = false;
                    }
                    else
                    {
                        Tipo_Bono tipo_Bono = db.Tipo_Bono.Find(id_tipo_bono);
                        if (tipo_Bono == null)
                        {
                            error += "Bonificación no encontrada; ";
                            estado = false;
                        }
                        else
                        {
                            bono = tipo_Bono.nombre;
                        }
                    }
                    if (!int.TryParse(a["Unidades"], out unidades))
                    {
                        error += "Formato de unidades incorrrecta; ";
                        estado = false;
                    }
                    if (!(decimal.TryParse(a["Total"], out total)))
                    {
                        error += "El total tiene formato incorrecto; ";
                        estado = false;
                    }
                    if (!DateTime.TryParse(a["Fecha"], out fecha))
                    {
                        error += "Fecha con formato incorrecto; ";
                        estado = false;
                    }
                    //Registro correcto
                    if (estado)
                    {
                        rCorrectos++;
                        tota_bonificacion += Math.Round(total, 2);
                        resultados.Insert(resultados.Count, new { id_bono = id_tipo_bono, id_empleado = id_empleado, bono = bono, unidades = unidades, fecha = fecha.ToString("dd/MM/yyyy"), estado = estado, error = "", total = Math.Round(total, 2) });
                    }
                    else
                    {
                        rIncorrectos++;
                        resultados.Insert(0, new { id_bono = 0, id_empleado = a["Código Empleado"].Value, bono = a["Tipo de Bono"].Value.ToString(), unidades = a["Unidades"].Value, fecha = a["Fecha"].Value.ToString(), estado = estado, error = error, total = a["Total"].Value });
                    }
                }
                System.IO.File.Delete(filepath);
                return Json(new { resultados, mensaje = "Registros Correctos: " + rCorrectos.ToString() + " - Registros Incorrectos: " + rIncorrectos.ToString() + " - Bonificación ingresada: " + tota_bonificacion.ToString("C", CultureInfo.GetCultureInfo("es-GT")) }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                System.IO.File.Delete(filepath);
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GuardarDescuentos(int id)
        {
            string[] id_empleado = Request["id_empleado"].Split(',');
            string[] fecha = Request["fecha"].Split(',');
            string[] total = Request["total"].Split(',');
            string[] unidades = Request["unidades"].Split(',');
            string[] tipo_descuento = Request["tipo_descuento"].Split(',');
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < id_empleado.Length; i++)
                    {
                        Descuento descuento = NuevoDescuento();
                        descuento.id_tipo_descuento = Convert.ToInt32(tipo_descuento[i]);
                        descuento.fecha = Convert.ToDateTime(fecha[i]);
                        descuento.automatico = false;
                        int empleado = Convert.ToInt32(id_empleado[i]);
                        var empleado_encabezado = db.Empleado_Encabezado_Planilla.SingleOrDefault(e => e.activo && !e.eliminado && e.id_encabezado_planilla == id && e.id_empleado == empleado);
                        if (empleado_encabezado == null) continue;
                        descuento.id_empleado_encabezado_planilla = empleado_encabezado.id_empleado_encabezado_planilla;
                        descuento.id_empleado = empleado_encabezado.id_empleado;
                        descuento.cantidad = Convert.ToInt32(unidades[i]);
                        descuento.total = Convert.ToDecimal(total[i], CultureInfo.InvariantCulture);
                        db.Descuento.Add(descuento);
                    }
                    db.SaveChanges();
                    tran.Commit();
                    ContextMessage msg = new ContextMessage(ContextMessage.Success, "Los descuentos se guardaron en la planilla correctamente.");
                    msg.ReturnUrl = Url.Action("Index");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "los descuentos no pudieron ser guardados en la planilla correctamente. Error: " + ex.Message);
                    msg.ReturnUrl = Url.Action("Index");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }
        }

        [HttpPost]
        public ActionResult GuardarBonificaciones(int id)
        {
            string[] id_empleado = Request["id_empleado"].Split(',');
            string[] fecha = Request["fecha"].Split(',');
            string[] total = Request["total"].Split(',');
            string[] unidades = Request["unidades"].Split(',');
            string[] tipo_bono = Request["tipo_bono"].Split(',');
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    for (int i = 0; i < id_empleado.Length; i++)
                    {
                        Bonificacion bonificacion = NuevaBonificacion();
                        bonificacion.id_tipo_bono = Convert.ToInt32(tipo_bono[i]);
                        bonificacion.fecha = Convert.ToDateTime(fecha[i]);
                        bonificacion.automatico = false;
                        int empleado = Convert.ToInt32(id_empleado[i]);
                        var empleado_encabezado = db.Empleado_Encabezado_Planilla.SingleOrDefault(e => e.activo && !e.eliminado && e.id_encabezado_planilla == id && e.id_empleado == empleado);
                        if (empleado_encabezado == null) continue;
                        bonificacion.id_empleado_encabezado_planilla = empleado_encabezado.id_empleado_encabezado_planilla;
                        bonificacion.id_empleado = empleado_encabezado.id_empleado;
                        bonificacion.cantidad = Convert.ToInt32(unidades[i]);
                        bonificacion.total = Convert.ToDecimal(total[i], CultureInfo.InvariantCulture);
                        db.Bonificacion.Add(bonificacion);
                    }
                    db.SaveChanges();
                    tran.Commit();
                    ContextMessage msg = new ContextMessage(ContextMessage.Success, "Las bonificaciones se guardaron en la planilla correctamente.");
                    msg.ReturnUrl = Url.Action("Index");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "Las bonificaciones no pudieron ser guardadas en la planilla correctamente. Error: " + ex.Message);
                    msg.ReturnUrl = Url.Action("Index");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }
        }

        [HttpPost]
        public ActionResult Subir_Dias_Trabajados(HttpPostedFileBase file, int id)
        {
            int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            var filepath = Server.MapPath("~/Archivos/Temp/" + id_usuario.ToString() + "_dias_trabajados_" + file.FileName);
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            file.SaveAs(filepath);
            var excel = new ExcelQueryFactory(filepath);
            var worksheetNames = excel.GetWorksheetNames();
            string errores = "";
            string error_especial = "";
            int no_fila = 1;
            bool errores_detectados = false;
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var sheet = worksheetNames.First();
                    errores += "Errores en bonificaciones, filas número: ";
                    error_especial = "Error fatal en bonificaciones, fila: ";
                    var worksheet = from a in excel.Worksheet(sheet) select a;
                    no_fila = 1;
                    foreach (var a in worksheet)
                    {
                        //No tomar en cuenta lineas en blanco
                        if (String.IsNullOrEmpty(a["Codigo Empleado"]) || String.IsNullOrEmpty(a["Dias trabajados"]))
                        {
                            continue;
                        }
                        int id_empleado, dias_trabajados;
                        if (!int.TryParse(a["Codigo Empleado"], out id_empleado))
                        {
                            errores += no_fila.ToString() + ", ";
                            errores_detectados = true;
                            no_fila++;
                            continue;
                        }
                        if (!int.TryParse(a["Dias trabajados"], out dias_trabajados))
                        {
                            errores += no_fila.ToString() + ", ";
                            errores_detectados = true;
                            no_fila++;
                            continue;
                        }
                        var empleado_encabezado = db.Empleado_Encabezado_Planilla.SingleOrDefault(e => e.id_empleado == id_empleado && e.id_encabezado_planilla == id);
                        if (empleado_encabezado == null)
                        {
                            errores += no_fila.ToString() + ", ";
                            errores_detectados = true;
                            continue;
                        }
                        else
                        {
                            empleado_encabezado.dias_trabajados = dias_trabajados;
                            db.Entry(empleado_encabezado).State = EntityState.Modified;
                        }
                        no_fila++;
                    }
                    db.SaveChanges();
                    tran.Commit();
                    System.IO.File.Delete(filepath);
                    return Json(new { response = true, error_detectado = errores_detectados, errores = errores });
                }
                catch
                {
                    tran.Rollback();
                    error_especial += no_fila.ToString();
                    System.IO.File.Delete(filepath);
                    return Json(new { response = false, error = error_especial });
                }
            }
        }


        [HttpPost]
        public ActionResult Subir_Tipo_Pago(HttpPostedFileBase file, int id)
        {
            int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            var filepath = Server.MapPath("~/Archivos/Temp/" + id_usuario.ToString() + "_tipopago_" + file.FileName);
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            file.SaveAs(filepath);
            var excel = new ExcelQueryFactory(filepath);
            var worksheetNames = excel.GetWorksheetNames();
            string errores = "";
            int no_fila = 1;
            bool errores_detectados = false;
            var empleados = db.Empleado_Encabezado_Planilla.Where(e => e.id_encabezado_planilla == id).Select(e => e.id_empleado).ToList();
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    var sheet = worksheetNames.First();
                    errores += "Errores encontrados, filas número: ";
                    var worksheet = from a in excel.Worksheet(sheet) select a;
                    no_fila = 1;
                    foreach (var a in worksheet)
                    {
                        int id_empleado, id_tipo_pago, id_banco, agencia, no;
                        long cuenta;
                        //Ingorar lineas en blanco
                        if (String.IsNullOrEmpty(a["Empleado"]) && String.IsNullOrEmpty(a["Tipo Pago"]) && String.IsNullOrEmpty(a["Banco"]) && String.IsNullOrEmpty(a["Agencia"]) && String.IsNullOrEmpty(a["Cuenta"]) && String.IsNullOrEmpty(a["No"]))
                        {
                            continue;
                        }
                        if (!int.TryParse(a["Empleado"], out id_empleado))
                        {
                            errores += no_fila.ToString() + ", ";
                            errores_detectados = true;
                            no_fila++;
                            continue;
                        }

                        if (!empleados.Contains(id_empleado))
                        {
                            errores += no_fila.ToString() + ", ";
                            errores_detectados = true;
                            no_fila++;
                            continue;
                        }
                        if (!int.TryParse(a["Tipo Pago"], out id_tipo_pago))
                        {
                            errores += no_fila.ToString() + ", ";
                            errores_detectados = true;
                            no_fila++;
                            continue;
                        }

                        Tipo_Pago tipo_pago = db.Tipo_Pago.SingleOrDefault(t => t.id_tipo_pago == id_tipo_pago && t.activo && !t.eliminado);
                        if (tipo_pago == null)
                        {
                            errores += no_fila.ToString() + ", ";
                            errores_detectados = true;
                            no_fila++;
                            continue;
                        }
                        var empleado_encabezado = db.Empleado_Encabezado_Planilla.SingleOrDefault(e => e.id_empleado == id_empleado && e.id_encabezado_planilla == id);
                        empleado_encabezado.id_tipo_pago = id_tipo_pago;
                        if (id_tipo_pago == (int)Catalogos.Tipo_Pago.Acreditacion)
                        {
                            if (!int.TryParse(a["Banco"], out id_banco))
                            {
                                errores += no_fila.ToString() + ", ";
                                errores_detectados = true;
                                no_fila++;
                                continue;
                            }

                            Banco banco = db.Banco.SingleOrDefault(b => b.activo && !b.eliminado && b.id_banco == id_banco);
                            if (banco == null)
                            {
                                errores += no_fila.ToString() + ", ";
                                errores_detectados = true;
                                no_fila++;
                                continue;
                            }
                            if (!int.TryParse(a["Agencia"], out agencia))
                            {
                                errores += no_fila.ToString() + ", ";
                                errores_detectados = true;
                                no_fila++;
                                continue;
                            }
                            cuenta = long.Parse(a["Cuenta"]);
                            /*if (!int.TryParse(a["Cuenta"], out cuenta))
                            {
                                errores += no_fila.ToString() + ", ";
                                errores_detectados = true;
                                no_fila++;
                                continue;
                            }*/
                            if (!int.TryParse(a["No"], out no))
                            {
                                errores += no_fila.ToString() + ", ";
                                errores_detectados = true;
                                no_fila++;
                                continue;
                            }
                            empleado_encabezado.id_banco = id_banco;
                            empleado_encabezado.agencia = agencia;
                            empleado_encabezado.numero = no;
                            empleado_encabezado.cuenta = cuenta;
                            db.Entry(empleado_encabezado).State = EntityState.Modified;
                        }
                        if (id_tipo_pago == (int)Catalogos.Tipo_Pago.Cheque)
                        {
                            if (!int.TryParse(a["Banco"], out id_banco))
                            {
                                errores += no_fila.ToString() + ", ";
                                errores_detectados = true;
                                no_fila++;
                                continue;
                            }

                            Banco banco = db.Banco.SingleOrDefault(b => b.activo && !b.eliminado && b.id_banco == id_banco);
                            if (banco == null)
                            {
                                errores += no_fila.ToString() + ", ";
                                errores_detectados = true;
                                no_fila++;
                                continue;
                            }
                            if (!int.TryParse(a["No"], out no))
                            {
                                errores += no_fila.ToString() + ", ";
                                errores_detectados = true;
                                no_fila++;
                                continue;
                            }
                            empleado_encabezado.id_banco = id_banco;
                            empleado_encabezado.numero = no;
                            db.Entry(empleado_encabezado).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        no_fila++;
                    }
                    tran.Commit();
                    return Json(new { response = true, error_detectado = errores_detectados, errores = errores });
                }
                catch
                {
                    tran.Rollback();
                    return Json(new { response = false, error = "" });
                }
            }
        }

        #endregion

        #region Metodos Extras
        public Planilla NuevaPlanilla()
        {
            Planilla planilla = new Planilla();
            planilla.activo = true;
            planilla.eliminado = false;
            planilla.fecha_creacion = DateTime.Now;
            planilla.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return planilla;
        }

        public Descuento NuevoDescuento()
        {
            Descuento descuento = new Descuento();
            descuento.activo = true;
            descuento.eliminado = false;
            descuento.fecha_creacion = DateTime.Now;
            descuento.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return descuento;
        }

        public Bonificacion NuevaBonificacion()
        {
            Bonificacion bonificacion = new Bonificacion();
            bonificacion.activo = true;
            bonificacion.eliminado = false;
            bonificacion.fecha_creacion = DateTime.Now;
            bonificacion.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return bonificacion;
        }

        public Encabezado_Planilla NuevoEncabezado()
        {
            Encabezado_Planilla encabezado = new Encabezado_Planilla();
            encabezado.activo = true;
            encabezado.eliminado = false;
            encabezado.fecha_creacion = encabezado.fecha_apertura = DateTime.Now;
            encabezado.id_usuario_creacion = encabezado.usuario_apertura = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return encabezado;
        }

        public Empleado_Encabezado_Planilla NuevoEmpleado_Encabezado()
        {
            Empleado_Encabezado_Planilla empleado_encabezado = new Empleado_Encabezado_Planilla();
            empleado_encabezado.activo = true;
            empleado_encabezado.eliminado = false;
            empleado_encabezado.fecha_creacion = DateTime.Now;
            empleado_encabezado.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return empleado_encabezado;
        }
        

        public ViewResult Mensaje()
        {
            ContextMessage msg = (ContextMessage)TempData[User.Identity.Name];
            return View(ContextMessage.ViewLocation(this), msg);
        }

        public string ObtenerMes(int mes)
        {
            switch (mes)
            {
                case 1:
                    return "Enero";
                case 2:
                    return "Febrero";
                case 3:
                    return "Marzo";
                case 4:
                    return "Abril";
                case 5:
                    return "Mayo";
                case 6:
                    return "Junio";
                case 7:
                    return "Julio";
                case 8:
                    return "Agosto";
                case 9:
                    return "Septiembre";
                case 10:
                    return "Octubre";
                case 11:
                    return "Noviembre";
                case 12:
                    return "Diciembre";
                default:
                    return "";
            }
        }
        #endregion

        #region Reportes

        //Reporte de Planilla
        public FileStreamResult GetExcel_Planilla(int id)
        {
            Encabezado_Planilla encabezado = db.Encabezado_Planilla.SingleOrDefault(e => e.id_encabezado_planilla == id && !e.eliminado);
            if (encabezado == null)
            {
                return null;
            }
            FileInfo templateFile = new FileInfo(Server.MapPath("~/Archivos/Templates/TemplatePlanilla.xlsx"));
            string nameFile = "Planilla-" + encabezado.id_encabezado_planilla;
            FileInfo newFile = new FileInfo(Server.MapPath("~/Archivos/Temp/" + nameFile + ".xlsx"));
            if (newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(Server.MapPath("~/Archivos/Temp/" + nameFile + ".xlsx"));
            }

            using (ExcelPackage package = new ExcelPackage(newFile, templateFile))
            {

                db.Database.CommandTimeout = 120;
                var planilla = db.rpt_Pre_Planilla(id).ToList();
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                worksheet.Cells["A1"].Value = encabezado.activo ? "Pre Planilla" : "Planilla Final";
                worksheet.Cells["A2"].Value = encabezado.Empresa.nombre;
                worksheet.Cells["A3"].Value = encabezado.Tipo_Planilla.nombre.ToUpper() + " " + encabezado.fecha_apertura.ToString("MMMM", CultureInfo.GetCultureInfo("es-GT")).ToUpper() + " " + encabezado.fecha_apertura.Year;
                string fila = "";
                //Write Excel File
                for (int i = 0; i < planilla.Count; i++)
                {
                    fila = (i + 5).ToString();
                    var registro = planilla[i];
                    worksheet.Cells["A" + fila].Value = registro.codigo;
                    worksheet.Cells["B" + fila].Value = registro.dpi;
                    worksheet.Cells["C" + fila].Value = registro.empleado;
                    worksheet.Cells["D" + fila].Value = registro.fecha_nacimiento.Value.ToString("dd/MM/yyyy");
                    worksheet.Cells["E" + fila].Value = registro.fecha_alta.Value.ToString("dd/MM/yyyy");
                    worksheet.Cells["F" + fila].Value = registro.fecha_baja.HasValue ? registro.fecha_baja.Value.ToString("dd/MM/yyyy") : "";
                    worksheet.Cells["G" + fila].Value = registro.sueldo_mensual;
                    worksheet.Cells["H" + fila].Value = registro.bono_mensual;
                    worksheet.Cells["I" + fila].Value = registro.bono_incentivo_mensual;
                    worksheet.Cells["J" + fila].Value = registro.dias_trabajados;
                    worksheet.Cells["K" + fila].Value = registro.dias_suspension;
                    worksheet.Cells["L" + fila].Formula = "=ROUND(G" + fila + "*J" + fila + "/30,2)";
                    worksheet.Cells["M" + fila].Formula = "=ROUND(H" + fila + "*J" + fila + "/30,2)";
                    worksheet.Cells["N" + fila].Formula = "=ROUND(I" + fila + "*J" + fila + "/30,2)";
                    worksheet.Cells["O" + fila].Value = registro.horas_extras;
                    worksheet.Cells["P" + fila].Value = registro.boleta_extra;
                    worksheet.Cells["Q" + fila].Formula = "=L" + fila + " + M" + fila + " + N" + fila + " + O" + fila + " + P" + fila;
                    worksheet.Cells["R" + fila].Formula = registro.prestaciones == 1 ? "=ROUND((L" + fila + "+O" + fila + ")*0.0483,2)" : "=0";
                    worksheet.Cells["S" + fila].Value = registro.alimentacion;
                    worksheet.Cells["T" + fila].Value = registro.carnet;
                    worksheet.Cells["U" + fila].Value = registro.faltas;
                    worksheet.Cells["V" + fila].Value = registro.cuadra;
                    worksheet.Cells["W" + fila].Value = registro.antecedentes;
                    worksheet.Cells["X" + fila].Value = registro.varios;
                    worksheet.Cells["Y" + fila].Value = registro.judicial;
                    worksheet.Cells["Z" + fila].Value = registro.prestamo_bancario;
                    worksheet.Cells["AA" + fila].Value = registro.prestamo_emergente;
                    worksheet.Cells["AB" + fila].Formula = "=S" + fila + " + T" + fila + " + U" + fila + " + V" + fila + " + W" + fila + " + X" + fila
                        + " + Y" + fila + " + Z" + fila + " + AA" + fila;
                    worksheet.Cells["AC" + fila].Value = registro.prestamo_interno;
                    worksheet.Cells["AD" + fila].Value = registro.anticipos;
                    worksheet.Cells["AE" + fila].Value = registro.uniformes;
                    worksheet.Cells["AF" + fila].Value = registro.isr;
                    worksheet.Cells["AG" + fila].Value = registro.telefono;
                    worksheet.Cells["AH" + fila].Value = registro.multas_transito;
                    worksheet.Cells["AI" + fila].Value = registro.daños_vehiculo;
                    worksheet.Cells["AJ" + fila].Value = registro.gasto_medico;
                    worksheet.Cells["AK" + fila].Value = registro.prestamo_bantrab;
                    worksheet.Cells["AL" + fila].Value = registro.anticipo_emergente;
                    worksheet.Cells["AM" + fila].Formula = "=R" + fila + " + AB" + fila + " + SUM(AC" + fila + ":AL" + fila + ")";
                    worksheet.Cells["AN" + fila].Formula = "=Q" + fila + " - AM" + fila;
                    worksheet.Cells["AO" + fila].Value = registro.estado;
                    worksheet.Cells["AP" + fila].Value = registro.ubicacion;
                    worksheet.Cells["AQ" + fila].Value = registro.cliente;
                    worksheet.Cells["AR" + fila].Value = registro.puesto;
                    worksheet.Cells["AS" + fila].Value = registro.banco;
                    worksheet.Cells["AT" + fila].Value = registro.agencia;
                    worksheet.Cells["AU" + fila].Value = registro.cuenta;
                    worksheet.Cells["AV" + fila].Value = registro.numero;
                    worksheet.Cells["AW" + fila].Value = registro.empresa;
                    worksheet.Cells["AX" + fila].Value = registro.tipo_pago;
                    worksheet.Cells["AY" + fila].Value = registro.direcciont;
                }

                //Totales
                fila = (5 + planilla.Count).ToString();
                string lastRow = (5 + planilla.Count - 1).ToString();
                worksheet.Cells["A" + fila].Value = "Totales";
                worksheet.Cells["C" + fila].Value = planilla.Count;
                worksheet.Cells["G" + fila].Formula = "=SUM(G5:G" + lastRow + ")";
                worksheet.Cells["H" + fila].Formula = "=SUM(H5:H" + lastRow + ")";
                worksheet.Cells["I" + fila].Formula = "=SUM(I5:I" + lastRow + ")";
                worksheet.Cells["J" + fila].Formula = "=SUM(J5:J" + lastRow + ")";
                worksheet.Cells["K" + fila].Formula = "=SUM(K5:K" + lastRow + ")";
                worksheet.Cells["L" + fila].Formula = "=SUM(L5:L" + lastRow + ")";
                worksheet.Cells["M" + fila].Formula = "=SUM(M5:M" + lastRow + ")";
                worksheet.Cells["N" + fila].Formula = "=SUM(N5:N" + lastRow + ")";
                worksheet.Cells["O" + fila].Formula = "=SUM(O5:O" + lastRow + ")";
                worksheet.Cells["P" + fila].Formula = "=SUM(P5:P" + lastRow + ")";
                worksheet.Cells["Q" + fila].Formula = "=SUM(Q5:Q" + lastRow + ")";
                worksheet.Cells["R" + fila].Formula = "=SUM(R5:R" + lastRow + ")";
                worksheet.Cells["S" + fila].Formula = "=SUM(S5:S" + lastRow + ")";
                worksheet.Cells["T" + fila].Formula = "=SUM(T5:T" + lastRow + ")";
                worksheet.Cells["U" + fila].Formula = "=SUM(U5:U" + lastRow + ")";
                worksheet.Cells["V" + fila].Formula = "=SUM(V5:V" + lastRow + ")";
                worksheet.Cells["W" + fila].Formula = "=SUM(W5:W" + lastRow + ")";
                worksheet.Cells["X" + fila].Formula = "=SUM(X5:X" + lastRow + ")";
                worksheet.Cells["Y" + fila].Formula = "=SUM(Y5:Y" + lastRow + ")";
                worksheet.Cells["Z" + fila].Formula = "=SUM(Z5:Z" + lastRow + ")";
                worksheet.Cells["AA" + fila].Formula = "=SUM(AA5:AA" + lastRow + ")";
                worksheet.Cells["AB" + fila].Formula = "=SUM(AB5:AB" + lastRow + ")";
                worksheet.Cells["AC" + fila].Formula = "=SUM(AC5:AC" + lastRow + ")";
                worksheet.Cells["AD" + fila].Formula = "=SUM(AD5:AD" + lastRow + ")";
                worksheet.Cells["AE" + fila].Formula = "=SUM(AE5:AE" + lastRow + ")";
                worksheet.Cells["AF" + fila].Formula = "=SUM(AF5:AF" + lastRow + ")";
                worksheet.Cells["AG" + fila].Formula = "=SUM(AG5:AG" + lastRow + ")";
                worksheet.Cells["AH" + fila].Formula = "=SUM(AH5:AH" + lastRow + ")";
                worksheet.Cells["AI" + fila].Formula = "=SUM(AI5:AI" + lastRow + ")";
                worksheet.Cells["AJ" + fila].Formula = "=SUM(AJ5:AJ" + lastRow + ")";
                worksheet.Cells["AK" + fila].Formula = "=SUM(AK5:AK" + lastRow + ")";
                worksheet.Cells["AL" + fila].Formula = "=SUM(AL5:AL" + lastRow + ")";
                worksheet.Cells["AM" + fila].Formula = "=SUM(AM5:AM" + lastRow + ")";
                worksheet.Cells["AN" + fila].Formula = "=SUM(AN5:AN" + lastRow + ")";
                worksheet.Calculate();
                worksheet.Cells["A" + fila + ":AW" + fila].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["A" + fila + ":AW" + fila].Style.Fill.BackgroundColor.SetColor(Color.SteelBlue);
                worksheet.Cells["A" + fila + ":AW" + fila].Style.Font.Bold = true;
                package.Save();
            }
            var filePath = Server.MapPath("~/Archivos/Temp/" + nameFile +".xlsx");
            var mimeType = "application/vnd.ms-excel";
            return File(new FileStream(filePath, FileMode.Open), mimeType, nameFile + ".xlsx");
        }

        //Reportes de planilla final
        public FileStreamResult GetPdf_PF(int id)
        {
            int idValido = 0;
            Encabezado_Planilla encabezado = db.Encabezado_Planilla.Find(id);
            if (encabezado != null)
            {
                idValido = encabezado.id_encabezado_planilla;
            }
            string parametros = "&id_encabezado_planilla=" + idValido.ToString();
            string reporte = "rpt_Planilla_Final";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Reporte Planilla Final " + encabezado.Empresa.nombre +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult GetExcel_PF(int id)
        {
            int idValido = 0;
            Encabezado_Planilla encabezado = db.Encabezado_Planilla.Find(id);
            if (encabezado != null)
            {
                idValido = encabezado.id_encabezado_planilla;
            }
            string parametros = "&id_encabezado_planilla=" + idValido.ToString();
            string reporte = "rpt_Planilla_Final";
            PDF_Protal archivo_reporte = new PDF_Protal(parametros, 0, reporte, "excel");
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/ms-excel";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Reporte Planilla Final " + encabezado.Empresa.nombre +
                      ".xls\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, Response.ContentType);
        }

        //Reporte de Boletas de Pago
        public FileStreamResult GenerarBoletaPagoPlanilla(int id)
        {
            Encabezado_Planilla encabezado = db.Encabezado_Planilla.SingleOrDefault(e=>e.id_encabezado_planilla == id && !e.eliminado);
            if(encabezado == null)
            {
                return null;
            }
            string parametros = "&id_encabezado_planilla=" + id.ToString();
            string reporte = "rpt_Boleta_Pago_Planilla";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Boletas de Pago Planilla No. " + id.ToString() +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        //Reporte de Resumen de Planilla
        public FileStreamResult GenerarResumenPlanilla(int id)
        {
            string parametros = "&id_encabezado_planilla=" + id.ToString();
            string reporte = "rpt_Resumen_Planilla";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Resumen de Planilla No. " + id.ToString() +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        //Reportes de pre planilla
        public FileStreamResult GetExcel_Pre_Planilla(int id)
        {
            int idValido = 0;
            Encabezado_Planilla encabezado = db.Encabezado_Planilla.Find(id);
            if (encabezado != null)
            {
                idValido = encabezado.id_encabezado_planilla;
            }
            string parametros = "&id_encabezado_planilla=" + idValido.ToString();
            string reporte = "rpt_Pre_Planilla";
            PDF_Protal archivo_reporte = new PDF_Protal(parametros, 0, reporte, "excel");
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/ms-excel";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Reporte Pre Planilla " + encabezado.Empresa.nombre + " " + encabezado.fecha_apertura.ToString("dd/MM/yyyy") +
                      ".xls\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, Response.ContentType);
        }

        public FileStreamResult GetExcel_PP(int id)
        {
            int idValido = 0;
            Encabezado_Planilla encabezado = db.Encabezado_Planilla.Find(id);
            if (encabezado != null)
            {
                idValido = encabezado.id_encabezado_planilla;
            }
            string parametros = "&id_encabezado_planilla=" + idValido.ToString() + "&id_empresa=" + encabezado.id_empresa;
            string reporte = "rpt_Pre_Planilla";
            PDF_Protal archivo_reporte = new PDF_Protal(parametros, 0, reporte, "excel");
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/ms-excel";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Reporte Pre Planilla " + encabezado.Empresa.nombre +
                      ".xls\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, Response.ContentType);
        }

        #endregion

        #region ReporteIgss

        public ActionResult ReporteIgss()
        {
            ViewBag.id_empresa = new SelectList(db.Empresa.Where(e => !e.eliminado), "id_empresa", "nombre");
            return View();
        }

        [HttpPost]
        public ActionResult GenerarArchivo(int id_empresa, DateTime fecha)
        {
            var planillas = db.Encabezado_Planilla.Where(e => e.id_empresa == id_empresa && !e.eliminado && e.fecha_apertura.Month == fecha.Month && e.fecha_apertura.Year == fecha.Year);

            if (planillas.Where(e => e.activo).Count() > 0) //Existen planillas activas
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Info, "No se puede generar el archivo de texto debido a que existen planillas activas en la empresa.");
                msg.ReturnUrl = Url.Action("ReporteIgss");
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
            else
            {
                int usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                string ruta = "~/Archivos/Temp/reporte_igss_ " + usuario + "_" + DateTime.Now.ToString() + ".txt";
                try
                {
                    return EscribirArchivo(id_empresa, fecha, ruta);
                }
                catch (Exception ex)
                {
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "Error durante la generación del archivo. Error" + ex.Message);
                    msg.ReturnUrl = Url.Action("ReporteIgss");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }
        }

        public FileContentResult EscribirArchivo(int id_empresa, DateTime fecha, string ruta)
        {
            StringWriter file = new StringWriter();
            var empleados = db.rpt_Igss_Empleados(id_empresa, fecha);
            var suspendidos = db.rpt_Igss_Suspendidos(id_empresa, fecha);
            var empresa = db.rpt_IgssEmpresa(id_empresa).First();

            file.WriteLine("2.1.0|" + 
                DateTime.Today.ToString("dd/MM/yyyy") + "|" +
                empresa.numero_patronal + "|" +
                fecha.ToString("MM") + "|" +
                fecha.ToString("yyyy") + "|" +
                empresa.nombre_comercial + "|" +
                empresa.nit + "|" +
                empresa.correo + "|0"
                );

            file.WriteLine("[centros]");
            file.WriteLine("1|" + 
                empresa.nombre + "|" +
                empresa.direccion + "|" +
                empresa.zona + "|" +
                empresa.telefono + "|" +
                empresa.fax + "|" +
                empresa.contacto + "|" +
                "1|1|749200");

            file.WriteLine("[tiposplanilla]");
            file.WriteLine("1|PLANILLA MENSUAL|C|M|1|749200|N|");

            file.WriteLine("[liquidaciones]");
            file.WriteLine("1|1|01/" + fecha.ToString("MM/yyyy") + "|" + DateTime.DaysInMonth(fecha.Year, fecha.Month) + "/" + fecha.ToString("MM/yyyy") + "|O||");

            file.WriteLine("[empleados]");
            foreach (var emp in empleados)
            {
                file.WriteLine(emp.identificacion_liquidacion + "|" +
                    emp.numero_afiliado + "|" +
                    emp.primer_nombre + "|" +
                    emp.segundo_nombre + "|" +
                    emp.primer_apellido + "|" +
                    emp.segundo_apellido + "|" +
                    emp.apellido_casada + "|" +
                    emp.sueldo_devengado + "|" +
                    emp.fecha_inicio.Value.ToString("dd/MM/yyyy") + "|" +
                    ((emp.fecha_fin.HasValue) ? emp.fecha_fin.Value.ToString("dd/MM/yyyy") : "") + "|" +
                    emp.centro_trabajo + "|" +
                    emp.nit + "|" +
                    emp.codigo_ocupacion + "|" +
                    emp.condicion_laboral + "|" +
                    emp.deducciones);
            }

            file.WriteLine("[suspendidos]");
            foreach (var susp in suspendidos)
            {
                file.WriteLine(susp.identificacion_liquidacion + "|" +
                    susp.numero_afiliado + "|" +
                    susp.primer_nombre + "|" +
                    susp.segundo_nombre + "|" +
                    susp.primer_apellido + "|" +
                    susp.segundo_apellido + "|" +
                    susp.apellido_casada + "|" +
                    susp.fecha_inicio.Value.ToString("dd/MM/yyyy") + "|" +
                    susp.fecha_final.Value.ToString("dd/MM/yyyy"));
            }


            String contenido = file.ToString();
            String NombreArchivo = "Reporte_Igss" + DateTime.Now.Date;
            String ExtensionArchivo = "txt";
            file.Close();
            return File(new System.Text.UTF8Encoding().GetBytes(contenido), "text/" + ExtensionArchivo, NombreArchivo + "." + ExtensionArchivo);
        }

        #endregion

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
