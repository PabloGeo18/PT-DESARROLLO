using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Src.Comun.Util;
using MVC2013.Src.MvcPaging;
using MVC2013.Src.Seguridad.To;
using LinqToExcel;
using Remotion.FunctionalProgramming;
//using MVC2013.Src.Tdv.To;
using MVC2013.Src.EstadoFuerza;
using System.Data.Entity.Core.Objects;
using Newtonsoft.Json;
using System.IO;

namespace MVC2013.Areas.EstadoFuerza.Controllers
{
    public class CargaMasivaController : Controller
    {

        private Protal_webEntities db = new Protal_webEntities();

        // GET: EstadoFuerza/CargaMasiva
        public ActionResult Index()
        {

            pt_estadofuerza_encabezado encabezado = new pt_estadofuerza_encabezado();
            encabezado = db.pt_estadofuerza_encabezado.FirstOrDefault(x => x.fecha_fin == null && x.estado == 1);

            if (encabezado != null)
            {
                ViewBag.fecha_estado_fuerza = encabezado.fecha_inicio.Year.ToString("0000") + "-" + encabezado.fecha_inicio.Month.ToString("00") + "-" + encabezado.fecha_inicio.Day.ToString("00");
                return View();
            }
            else {
                return RedirectToAction(
                                  "Mensaje", // Action name
                                  "CargaMasiva", // Controller name
                                  new { msg = "No existe ningun Estado de Fuerza Iniciado." }); // Route values
            }
            
        }
        public ActionResult ConsultaArchivos()
        {

            DirectoryInfo salesFTPDirectory = null;
            FileInfo[] files = null;

            //IEnumerable<FileInfo> files = new IEnumerable<FileInfo>();

            try
            {
                //string salesFTPPath = "E:/ftproot/sales";
                string salesFTPPath = Server.MapPath("~/files/CargaEstadoFuerza");
                salesFTPDirectory = new DirectoryInfo(salesFTPPath);
                files = salesFTPDirectory.GetFiles();
            }
            catch (DirectoryNotFoundException exp)
            {
                throw new Exception("No se puede abrir el directorio", exp);
            }
            catch (IOException exp)
            {
                throw new Exception("Error al acceder al directorio", exp);
            }

            files = files.OrderByDescending(f => f.CreationTime).ToArray();

            //var salesFiles = files.Where(f => f.Extension == ".xls" || f.Extension == ".xml");

            return View(files);

        }

        public FileResult Download(string filePath)
        {
            return File(filePath, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(filePath));
        }
        public ActionResult Mensaje(string msg)
        {
            ViewBag.Mensaje = msg;
            return View();
        }

        // GET: EstadoFuerza/CargaMasiva/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EstadoFuerza/CargaMasiva/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EstadoFuerza/CargaMasiva/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EstadoFuerza/CargaMasiva/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EstadoFuerza/CargaMasiva/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: EstadoFuerza/CargaMasiva/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EstadoFuerza/CargaMasiva/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /* PROCESANDO CARGA MASIVA DE SOLICITUDES */
        // GET: TransporteValores/Solicitudes/CargaMasiva
        public ActionResult SubeArchivo(HttpPostedFileBase file)
        {
            using (var context = new Protal_webEntities())
            {

                pt_estadofuerza_encabezado encabezado_dia;
                encabezado_dia = context.pt_estadofuerza_encabezado.FirstOrDefault(x => x.fecha_fin == null && x.estado == 1);
                List<int?> empleados_dia = new List<int?>();
                List<int?> empleados = new List<int?>();
                List<int?> ubicaciones = new List<int?>();
                List<int?> tipo_ubicacion = new List<int?>();
                List<int?> estado = new List<int?>();

                
                if (encabezado_dia != null) {
                    foreach (pt_estadofuerza item in context.pt_estadofuerza.Where(x => x.fecha == encabezado_dia.fecha_inicio )){
                        empleados_dia.Add(item.ptempleadoid);
                    }
                    foreach (SelectListItem item in context.pt_empleado.Where(x =>  x.pt_contratacion.Count() > 0 && x.pt_contratacion.FirstOrDefault().ESTADO.ToUpper() != "BAJA").Select(s => new SelectListItem { Value = s.PTEMPLEADOID.ToString() }))
                    {
                        empleados.Add(int.Parse(item.Value));
                    }

                    foreach (pt_ubicacion item in context.pt_ubicacion.Where(x => x.ESTADO.ToUpper() !="BAJA"))
                    {
                        ubicaciones.Add(item.PTUBICACIONID);
                    }

                    foreach (pt_tipo_ubicacion item in context.pt_tipo_ubicacion )
                    {
                        tipo_ubicacion.Add(item.PTTIPOID);
                    }

                    foreach (pt_situacion item in context.pt_situacion)
                    {
                        estado.Add(item.id_situacion);
                    }

                }
                /* TRANSACCION DE OPERACIONES */
                    try
                    {
                        if (Request.Files["FileUpload1"].ContentLength > 0)
                        {
                            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                            DateTime now = DateTime.Now;
                            string fileName = Request.Files["FileUpload1"].FileName;
                            string extension = System.IO.Path.GetExtension(fileName);
                            string path = string.Format("{0}/{1}", System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + "_" + fileName);

                            //string pa = string.Format("{0}/{1}", System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + "_" + fileName);

                            /* ELIMINA EL ARCHIVO SI EXISTE */
                            if (System.IO.File.Exists(path)) { System.IO.File.Delete(path); }

                            /* GUARDANDO EL ARCHIVO TEMPORALMENTE */
                            Request.Files["FileUpload1"].SaveAs(path);

                            /*LEYENDO ARCHIVO EXCEL*/
                            //using linq to excel
                            var excel = new ExcelQueryFactory(path);
                            var worksheetNames = excel.GetWorksheetNames();

                            /* OBTENIENDO EL PRIMER WORKSHEET */
                            var firstWorksheet = worksheetNames.First();
                            var worksheet = from a in excel.Worksheet(firstWorksheet) select a;


                            int contador = 0;

                            pt_estadofuerza_encabezado encabezado;
                            //List<MVC2013.Src.EstadoFuerza.EstadoFuerzaMasivo> lista_errores = new List<MVC2013.Src.EstadoFuerza.EstadoFuerzaMasivo> ();
                            Lista_EstadoFuerzaMasivo lista_errores = new Lista_EstadoFuerzaMasivo();
                            Lista_EstadoFuerzaMasivo lista_correctos = new Lista_EstadoFuerzaMasivo();



                            encabezado = context.pt_estadofuerza_encabezado.FirstOrDefault(x => x.fecha_fin == null && x.estado == 1);
                            DateTime fecha_ingreso_masivo = DateTime.Now;
                            if (encabezado != null) {
                                /* GUARDANDO EL ARCHIVO EN SERVIDOR */
                                string path_server = string.Format("{0}/{1}", Server.MapPath("~/files/CargaEstadoFuerza"), "(" + encabezado.fecha_inicio.Year.ToString("0000") + "-" + encabezado.fecha_inicio.Month.ToString("00") + "-" + encabezado.fecha_inicio.Day.ToString("00") + ")_(" + fecha_ingreso_masivo.Year.ToString("0000") + "-" + fecha_ingreso_masivo.Month.ToString("00") + "-" + fecha_ingreso_masivo.Day.ToString("00") + " " + fecha_ingreso_masivo.Hour.ToString("00") + ";" + fecha_ingreso_masivo.Minute.ToString("00") + ";" + fecha_ingreso_masivo.Second.ToString("00") + ")_" + usuarioTO.usuario.usuario + "_" + fileName);
                                if (System.IO.File.Exists(path_server)) { System.IO.File.Delete(path_server); }
                                Request.Files["FileUpload1"].SaveAs(path_server);


                                foreach (var a in worksheet)
                                {

                                    EstadoFuerzaMasivo loteDetalle = new EstadoFuerzaMasivo();
                                    loteDetalle.id_encabezado = encabezado.id;
                                    
                                    try
                                    {
                                        int id_empleado = int.Parse(a["id_empleado"]);
                                        int id_ubicacion = int.Parse(a["id_ubicacion"]);
                                        int id_tipo_ubicacion = int.Parse(a["id_tipo_ubicacion"]);
                                        int id_estado = int.Parse(a["id_estado"]);
                                        string comentario = a["comentario"];


                                        loteDetalle.id_empleado = id_empleado;
                                        loteDetalle.id_ubicacion = id_ubicacion;
                                        loteDetalle.id_tipo_ubicacion = id_tipo_ubicacion;
                                        loteDetalle.id_estado = id_estado;
                                        loteDetalle.comentario = comentario;
                                        loteDetalle.fecha = encabezado.fecha_inicio;

                                        if (!empleados.Contains(loteDetalle.id_empleado))
                                        {
                                            loteDetalle.error = true;
                                            loteDetalle.error_msg += "Empleado Incorrecto o de baja. ";
                                        }

                                        if (!ubicaciones.Contains(loteDetalle.id_ubicacion ))
                                        {
                                            loteDetalle.error = true;
                                            loteDetalle.error_msg += "Ubicacion Incorrecta o de baja. ";
                                        }

                                        if (!tipo_ubicacion.Contains(loteDetalle.id_tipo_ubicacion) )
                                        {
                                            loteDetalle.error = true;
                                            loteDetalle.error_msg += "Tipo de Ubicacion Incorrecto. ";
                                        }
                                        if (!estado.Contains(loteDetalle.id_estado))
                                        {
                                            loteDetalle.error = true;
                                            loteDetalle.error_msg += "Estado Incorrecto. ";
                                        }
                                        if (empleados_dia.Contains(loteDetalle.id_empleado)) {
                                            loteDetalle.error = true;
                                            loteDetalle.error_msg += "El empleado ya existe para este dia";
                                        }
                                        if (lista_correctos.lista_carga.Where(x=> x.id_empleado == loteDetalle.id_empleado).Count() > 0)
                                        {
                                            loteDetalle.error = true;
                                            loteDetalle.error_msg += "El empleado ya existe en el archivo cargado";
                                        }
                                        if (loteDetalle.valida_errores())
                                        {
                                            contador++;
                                            loteDetalle.error = true;
                                            lista_errores.lista_carga.Add(loteDetalle);
                                        }
                                        else { 

                                            contador++;
                                            loteDetalle.error = false;

                                            //pt_estadofuerza estado_fuerza_masivo = new pt_estadofuerza();

                                            //estado_fuerza_masivo.ptempleadoid = loteDetalle.id_empleado;
                                            //estado_fuerza_masivo.ptubicacionid = loteDetalle.id_ubicacion;
                                            //estado_fuerza_masivo.pttipoid = loteDetalle.id_tipo_ubicacion;
                                            //estado_fuerza_masivo.estado = loteDetalle.id_estado;
                                            //estado_fuerza_masivo.fecha = encabezado.fecha_inicio;
                                            //estado_fuerza_masivo.id_encabezado = encabezado.id;
                                            //estado_fuerza_masivo.observaciones = loteDetalle.comentario;
                                            //estado_fuerza_masivo.modificado_el = fecha_ingreso_masivo;
                                            //estado_fuerza_masivo.usuario_creacion = usuarioTO.usuario.usuario;

                                            lista_correctos.lista_carga.Add(loteDetalle);
                                            //context.pt_estadofuerza.Add(estado_fuerza_masivo);
                                            //context.SaveChanges();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        contador++;
                                        loteDetalle.error=true;
                                        if (loteDetalle.id_empleado != 0) {
                                            lista_errores.lista_carga.Add(loteDetalle);
                                        }
                                       
                                        //loteDetalle.fila = contador;
                                        //loteDetalle.error_msg += e.Message;
                                        //context.Solicitudes_Lote_Detalle.Add(loteDetalle);
                                        //context.SaveChanges();
                                    }

                                }
                            }

                            //if (lista_correctos.lista_carga.Count() == 0)
                            {
                                //throw new Exception("Error al procesar Archivo.");
                            //}                             else {
                                //using (var dbContextTransaction = context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                                {

                                    try
                                    {
                                        foreach (EstadoFuerzaMasivo loteDetalle in lista_correctos.lista_carga)
                                        {
                                            pt_estadofuerza estado_fuerza_masivo = new pt_estadofuerza();
                                            estado_fuerza_masivo.ptempleadoid = loteDetalle.id_empleado;
                                            estado_fuerza_masivo.ptubicacionid = loteDetalle.id_ubicacion;
                                            estado_fuerza_masivo.pttipoid = loteDetalle.id_tipo_ubicacion;
                                            estado_fuerza_masivo.estado = loteDetalle.id_estado;
                                            estado_fuerza_masivo.fecha = encabezado.fecha_inicio;
                                            estado_fuerza_masivo.id_encabezado = encabezado.id;
                                            estado_fuerza_masivo.observaciones = loteDetalle.comentario;
                                            estado_fuerza_masivo.modificado_el = fecha_ingreso_masivo;
                                            estado_fuerza_masivo.usuario_creacion = usuarioTO.usuario.usuario;
                                            
                                            context.pt_estadofuerza.Add(estado_fuerza_masivo);
                                            

                                        }
                                        context.SaveChanges();

                                        //dbContextTransaction.Commit();
                                        TempData["listado_errores"] = lista_errores;
                                        return RedirectToAction(
                                              "DetalleCarga", // Action name
                                              "CargaMasiva", // Controller name
                                              new { total_cargado = lista_correctos.lista_carga.Count() }); // Route values
                                    }
                                    catch (Exception)
                                    {

                                        //dbContextTransaction.UnderlyingTransaction.Rollback();
                                    }
                                    

                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //
                        //ModelState.AddModelError("", App_GlobalResources.Resources.import_masivo_error);
                        // ViewBag.ErrorMessage = App_GlobalResources.Resources.import_masivo_error;
                        TempData["ErrorMessage"] = App_GlobalResources.Resources.import_masivo_error;
                    }

            }

        return RedirectToAction(
                "Mensaje", // Action name
                "CargaMasiva", new { msg = "Error al procesar el archivo" }); // Controller name            
        }

        public ActionResult DetalleCarga(int total_cargado)
        {
            //var listado_errObj = JsonConvert.DeserializeObject<MVC2013.Src.EstadoFuerza.Lista_EstadoFuerzaMasivo>(listado_errores);
            //var total_carga = total_cargado;
            ViewBag.listado_errores = TempData["listado_errores"];
            ViewBag.listado_cargado = total_cargado;
            return View();
        }
    }
}
