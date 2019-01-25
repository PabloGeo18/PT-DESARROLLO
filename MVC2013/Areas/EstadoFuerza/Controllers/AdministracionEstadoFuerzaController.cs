using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using System.Data.Entity;
using LinqToExcel;
using MVC2013.Src.Comun.Util;
using MVC2013.Src.Comun.View;
using MVC2013.Areas.EstadoFuerza.Models;

namespace MVC2013.Areas.EstadoFuerza.Controllers
{
    public class AdministracionEstadoFuerzaController : Controller
    {
        private AppEntities db = new AppEntities();
        // GET: EstadoFuerza/AdministracionEstadoFuerza
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EstadoFuerza()
        {
            Estado_Fuerza estado_fuerza = db.Estado_Fuerza.SingleOrDefault(e => e.activo && !e.eliminado && e.fecha_finalizacion == null);
            int estados = 0;
            if (estado_fuerza != null) {
                estados = estado_fuerza.Estado_Fuerza_Detalle.Count(x => x.activo && !x.eliminado);
            }
            
            int contratos = db.Contratacion.Count(x => x.activo && !x.eliminado);
            ViewBag.estados = estados;
            ViewBag.contratos = contratos;
            return View(estado_fuerza);
        }

        public ActionResult CrearEstadoFuerza(Estado_Fuerza estado_fuerza)
        {
            if (db.Estado_Fuerza.Where(f => (f.fecha == estado_fuerza.fecha && !f.eliminado) || f.activo).Count() > 0)
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Error, "El día seleccionado ya esta almacenado en el Estado de Fuerza.");
                msg.ReturnUrl = Url.Action("EstadoFuerza");
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
            estado_fuerza.activo = true;
            estado_fuerza.eliminado = false;
            estado_fuerza.fecha_creacion = DateTime.Now;
            estado_fuerza.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            try
            {
                db.Estado_Fuerza.Add(estado_fuerza);
                db.SaveChanges();
                return RedirectToAction("EstadoFuerza");
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

        public ActionResult FinalizarEstadoFuerza(int id_estado_fuerza)
        {
            Estado_Fuerza estadofuerza = db.Estado_Fuerza.SingleOrDefault(e => e.id_estado_fuerza == id_estado_fuerza && e.activo && !e.eliminado);
            if(estadofuerza == null)
            {
                return HttpNotFound();
            }
            estadofuerza.activo = false;
            estadofuerza.fecha_finalizacion = estadofuerza.fecha_modificacion = DateTime.Now;
            estadofuerza.id_usuario_finalizacion = estadofuerza.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            //using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    
                    db.Entry(estadofuerza).State = EntityState.Modified;
                    db.SaveChanges();
                    db.Finalizar_Dia(estadofuerza.id_estado_fuerza);
                    //tran.Commit();
                    return RedirectToAction("EstadoFuerza");
                }
                catch
                {
                    //tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "Error durante la conexión con el servidor. Cambios no efectuados.");
                    msg.ReturnUrl = Url.Action("EstadoFuerza");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }
        }

        public void ActualizarEmpleados(Estado_Fuerza estadofuerza)
        {
            var estadofuerzadetalle = estadofuerza.Estado_Fuerza_Detalle.Where(e => e.activo && !e.eliminado);
            foreach (var item in estadofuerzadetalle)
            {
                //Actualizar ubicacion del empleado en el contrato
                var contrato = db.Contratacion.Where(c => c.activo && !c.eliminado && c.id_empleado == item.id_empleado);
                if (contrato.Count() > 1)
                {
                    contrato.First().id_ubicacion = item.id_ubicacion;
                    //Dar de baja al empleado 
                    if (item.id_situacion == (int)Catalogos.Situacion.Ausente)
                    {
                        var estado_fuerza = db.Estado_Fuerza_Detalle.Where(e => e.activo && !e.eliminado && e.id_empleado == item.id_empleado && e.id_situacion == (int)Catalogos.Situacion.Ausente);
                        if (estado_fuerza.Count() >= 2)
                        {
                            contrato.First().id_estado_empleado = (int)Catalogos.Estado_Empleado.Baja;
                            contrato.First().id_motivo_baja = (int)Catalogos.Motivo_Baja.Ausencias;
                            contrato.First().razon_baja = "Baja automática.";
                        }
                    }
                    db.Entry(contrato.First()).State = EntityState.Modified;
                }
            }
        }


        public ActionResult CargarArchivo()
        {
            Estado_Fuerza estado_fuerza = db.Estado_Fuerza.SingleOrDefault(e => e.activo && !e.eliminado && e.fecha_finalizacion == null);
            if (estado_fuerza == null)
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Info,"No existe un Estado de Fuerza activo en el sistema.");
                msg.ReturnUrl = Url.Action("Index", "Home");
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
            return View(estado_fuerza);
        }

        public Carga_Estado_Fuerza NuevaCargaEstadoFuerza()
        {
            Carga_Estado_Fuerza carga = new Carga_Estado_Fuerza();
            carga.fecha_creacion = DateTime.Now;
            carga.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return carga;
        }

        [HttpPost]
        public ActionResult Leer_Archivo(HttpPostedFileBase file, int id)
        {
            var filepath = Server.MapPath("~/Archivos/Temp/" + file.FileName);
            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
            file.SaveAs(filepath);
            Estado_Fuerza estado_fuerza = db.Estado_Fuerza.SingleOrDefault(e => e.activo && !e.eliminado && e.id_estado_fuerza == id);
            if (estado_fuerza == null)
            {
                return HttpNotFound();
            }
            var excel = new ExcelQueryFactory(filepath);
            var worksheetNames = excel.GetWorksheetNames();
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                Carga_Estado_Fuerza carga_encabezado = NuevaCargaEstadoFuerza();
                carga_encabezado.id_estado_fuerza = id;
                carga_encabezado.nombre_archivo = file.FileName;
                try
                {
                    db.Carga_Estado_Fuerza.Add(carga_encabezado);
                    db.SaveChanges();
                }
                catch
                {
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "Error durante la conexión con el servidor. Carga de archivo no efectuada.");
                    msg.ReturnUrl = Url.Action("");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
                var hoja = from a in excel.Worksheet(worksheetNames.First()) select a;
                List<ResultadoCargaEF> resultado = new List<ResultadoCargaEF>();
                foreach (var a in hoja)
                {
                    bool correcto = true;
                    string error = "";
                    try
                    {
                        string empleado = a["id_empleado"];
                        string ubicacion = a["id_ubicacion"];
                        string tipo_servicio = a["id_tipo_ubicacion"]; //Tipo Servicio
                        string situacion = a["id_estado"]; //Situacion
                        string observacion = a["comentario"]; //Observacion
                        
                        int id_empleado, id_ubicacion, id_tipo_servicio, id_situacion;
                        //Código de Empleado
                        if (int.TryParse(empleado, out id_empleado))
                        {
                            var Empleado = db.Contratacion.SingleOrDefault(e => e.id_empleado == id_empleado && e.activo && !e.eliminado);
                            if (Empleado != null)
                            {
                                if (Empleado.id_estado_empleado != (int)Catalogos.Estado_Empleado.Alta)
                                {
                                    error = "El empleado no está de Alta.";
                                    correcto = false;
                                }
                            }
                            else
                            {
                                error = "El empleado no existe.\n";
                                correcto = false;
                            }
                        }
                        else
                        {
                            error = "El formato del código de empleado es incorrecto.\n";
                            correcto = false;
                        }
                        //Situacion
                        if (int.TryParse(situacion, out id_situacion))
                        {
                            if (db.Situacion.SingleOrDefault(s => s.activo && !s.eliminado && s.id_situacion == id_situacion) == null)
                            {
                                error += "Estado ingresado no reconocido.\n";
                                correcto = false;
                            }
                        }
                        else
                        {
                            error += "El formato del estado es incorrecto.\n";
                            correcto = false;
                        }
                        //Ubicacion
                        if (int.TryParse(ubicacion, out id_ubicacion))
                        {
                            if (db.Ubicaciones.SingleOrDefault(u => u.activo && !u.eliminado && u.id_ubicacion == id_ubicacion) == null)
                            {
                                error += "Ubicación ingresada no reconocida.\n";
                                correcto = false;
                            }
                        }
                        else
                        {
                            error += "El formato del código de ubicación es incorrecto.\n";
                            correcto = false;
                        }
                        //Tipo de Servicio
                        if (int.TryParse(tipo_servicio, out id_tipo_servicio))
                        {
                            if (db.Cat_Tipos_Agente.SingleOrDefault(t => t.activo && !t.eliminado && t.id_cat_tipo_agente == id_tipo_servicio) == null)
                            {
                                error += "Tipo de ubicación no reconocido.\n";
                                correcto = false;
                            }
                        }
                        else
                        {
                            error += "El formato del tipo de ubicación es incorrecto.\n";
                            correcto = false;
                        }

                        var detalle_ef = estado_fuerza.Estado_Fuerza_Detalle.SingleOrDefault(e => e.activo && !e.eliminado && e.id_empleado == id_empleado);
                        if (detalle_ef != null)
                        {
                            error += "El empleado ya esta registrado en el estado de fuerza.\n";
                            correcto = false;
                        }
                        //Validar que no se ingrese dos veces en el mismo archivo
                        var semejantes = resultado.Where(r => r.id_empleado == empleado);
                        if(semejantes.Count() > 0)
                        {
                            error += "Registro duplicado en la carga del archivo.\n";
                            correcto = false;
                        }
                        if(!String.IsNullOrEmpty(error))
                        {
                            error = error.Substring(0, error.LastIndexOf('.'));
                        }
                        Carga_Estado_Fuerza_Detalle carga_detalle = new Carga_Estado_Fuerza_Detalle();
                        carga_detalle.id_empleado = empleado;
                        carga_detalle.id_ubicacion = ubicacion;
                        carga_detalle.id_cat_tipo_agente = tipo_servicio;
                        carga_detalle.observacion = observacion;
                        carga_detalle.id_situacion = situacion;
                        carga_detalle.id_carga_estado_fuerza = carga_encabezado.id_carga_estado_fuerza;
                        db.Carga_Estado_Fuerza_Detalle.Add(carga_detalle);

                        //Resultados de la lectura del archivo
                        ResultadoCargaEF nuevo_resultado = new ResultadoCargaEF();
                        nuevo_resultado.correcto = correcto;
                        nuevo_resultado.error = error;
                        nuevo_resultado.id_empleado = empleado;
                        nuevo_resultado.id_cat_tipo_agente = tipo_servicio;
                        nuevo_resultado.id_situacion = situacion;
                        nuevo_resultado.observacion = observacion;
                        nuevo_resultado.id_ubicacion = ubicacion;
                        resultado.Add(nuevo_resultado);
                    }
                    catch
                    {
                        error += "Ocurrio un error innesperado en la lectura de este registro.";
                        correcto = false;
                    }
                }
                db.SaveChanges();
                tran.Commit();
                ViewBag.id_estado_fuerza = estado_fuerza.id_estado_fuerza;
                return View("ResultadoCargaMasiva", resultado.OrderBy(x=>x.correcto).ToList());
            }
        }

        public ActionResult Leer_Archivo()
        {
            return RedirectToAction("CargarArchivo");
        }

        [HttpPost]
        public ActionResult GuardarResultadoCargaMasiva(List<ResultadoCargaEF> resultadocargaef, int operacion) 
        {
            switch (operacion)
            {
                    //Guardar resultados correctos
                case 1:
                    using (DbContextTransaction tran = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var nuevo_resultado = VerificarDuplicados(resultadocargaef);
                            GuardarRegistros(nuevo_resultado.Where(r => r.correcto));
                            tran.Commit();
                            return RedirectToAction("Index", "Home");
                        }
                        catch
                        {
                            ContextMessage msg = new ContextMessage(ContextMessage.Error, "Los registros no pudieron ser guardados correctamente.");
                            msg.ReturnUrl = Url.Action("CargarArchivo");
                            TempData[User.Identity.Name] = msg;
                            tran.Rollback();
                            return RedirectToAction("Mensaje");
                        }
                    }
                    //Validar resultados
                case 2:
                    var nuevos_resultados = ValidarRegistros(resultadocargaef);
                    return View("ResultadoCargaMasiva", nuevos_resultados);
                default:
                    return RedirectToAction("CargarArchivo");
            }
        }

        public List<ResultadoCargaEF> VerificarDuplicados(List<ResultadoCargaEF> estado_fuerza_detalle)
        {
            List<ResultadoCargaEF> nuevo_resultado = new List<ResultadoCargaEF>();
            Estado_Fuerza ef = db.Estado_Fuerza.SingleOrDefault(e => e.activo && !e.eliminado && e.fecha_finalizacion == null);
            foreach (var registro in estado_fuerza_detalle.Where(r => r.correcto))
            {
                int id_empleado = int.Parse(registro.id_empleado);
                var detalle_ef = ef.Estado_Fuerza_Detalle.SingleOrDefault(e => e.activo && !e.eliminado && e.id_empleado == id_empleado);
                if (detalle_ef == null)
                {
                    nuevo_resultado.Add(registro);
                }
            }
            return nuevo_resultado;
        }

        public List<ResultadoCargaEF> ValidarRegistros(List<ResultadoCargaEF> estado_fuerza_detalle)
        {
            
            int id_empleado, id_ubicacion, id_tipo_agente, id_situacion;
            Estado_Fuerza ef = db.Estado_Fuerza.SingleOrDefault(e => e.activo && !e.eliminado && e.fecha_finalizacion == null);
            foreach (var item in estado_fuerza_detalle)
            {
                if(!item.correcto)
                {
                    bool correcto = true;
                    string error = "";
                    //Codigo del Empleado
                    if (int.TryParse(item.id_empleado, out id_empleado))
                    {
                        var Empleado = db.Contratacion.SingleOrDefault(e => e.id_empleado == id_empleado && e.activo && !e.eliminado);
                        if (Empleado != null)
                        {
                            if (Empleado.id_estado_empleado != (int)Catalogos.Estado_Empleado.Alta)
                            {
                                error = "El empleado no está de Alta.";
                                correcto = false;
                            }
                        }
                        else
                        {
                            error = "El empleado no existe.\n";
                            correcto = false;
                        }
                    }
                    else
                    {
                        error = "El formato del código de empleado es incorrecto.\n";
                        correcto = false;
                    }
                    //Situacion
                    if (int.TryParse(item.id_situacion, out id_situacion))
                    {
                        if (db.Situacion.SingleOrDefault(s => s.activo && !s.eliminado && s.id_situacion == id_situacion) == null)
                        {
                            error += "Estado ingresado no reconocido.\n";
                            correcto = false;
                        }
                    }
                    else
                    {
                        error += "El formato del estado es incorrecto.\n";
                        correcto = false;
                    }
                    //Ubicacion
                    if (int.TryParse(item.id_ubicacion, out id_ubicacion))
                    {
                        if (db.Ubicaciones.SingleOrDefault(u => u.activo && !u.eliminado && u.id_ubicacion == id_ubicacion) == null)
                        {
                            error += "Ubicación ingresada no reconocida.\n";
                            correcto = false;
                        }
                    }
                    else
                    {
                        error += "El formato del código de ubicación es incorrecto.\n";
                        correcto = false;
                    }
                    //Tipo de Servicio
                    if (int.TryParse(item.id_cat_tipo_agente, out id_tipo_agente))
                    {
                        if (db.Tipo_Servicio.SingleOrDefault(t => t.activo && !t.eliminado && t.id_tipo_servicio == id_tipo_agente) == null)
                        {
                            error += "Tipo de Agente no reconocido.\n";
                            correcto = false;
                        }
                    }
                    else
                    {
                        error += "El formato del tipo de agente es incorrecto.\n";
                        correcto = false;
                    }
                    
                    var detalle_ef = ef.Estado_Fuerza_Detalle.SingleOrDefault(e => e.activo && !e.eliminado && e.id_empleado == id_empleado);
                    if (detalle_ef != null)
                    {
                        error += "Ya fue almacenado este registro.";
                        correcto = false;
                    }
                    if (!String.IsNullOrEmpty(error))
                    {
                        error = error.Substring(0, error.LastIndexOf('.'));
                    }
                    //Registro analizado
                    item.error = error;
                    item.correcto = correcto;
                }
            }
            return estado_fuerza_detalle;
        }

        public void GuardarRegistros(IEnumerable<ResultadoCargaEF> estado_fuerza_detalle)
        {
            Estado_Fuerza ef = db.Estado_Fuerza.SingleOrDefault(e => e.activo && !e.eliminado && e.fecha_finalizacion == null);
            int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            foreach (var item in estado_fuerza_detalle)
            {
                Estado_Fuerza_Detalle efd = new Estado_Fuerza_Detalle();
                efd.activo = true; efd.eliminado = false;
                efd.fecha_creacion = DateTime.Now;
                efd.fecha = ef.fecha;
                efd.id_usuario_creacion = id_usuario;
                efd.id_empleado = Convert.ToInt32(item.id_empleado);
                efd.id_ubicacion = Convert.ToInt32(item.id_ubicacion);
                efd.id_situacion = Convert.ToInt32(item.id_situacion);
                efd.id_cat_tipo_agente = Convert.ToInt32(item.id_cat_tipo_agente);
                efd.observacion = item.observacion;
                efd.id_estado_fuerza = ef.id_estado_fuerza;
                db.Estado_Fuerza_Detalle.Add(efd);
            }
            db.SaveChanges();
        }


    }
}