using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Areas.rrhh.Models;
using MVC2013.Src.Comun.Util;
using MVC2013.Src.Seguridad.To;
using System.IO;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.Diagnostics;
using System.Globalization;
using MVC2013.Src.Comun.View;
using MVC2013.Src.Sdc.Reports;
namespace MVC2013.Areas.rrhh.Controllers
{
    public class EmpleadoController : Controller
    {
        private AppEntities db = new AppEntities();

        private string ruta_imagen_default = "~/Archivos/Templates/foto_perfil_default.png";

        #region WebCam

        
        public void TomarFoto()
        {
            string ruta_imagen_webcam = "~/Archivos/Temp/webcam_picture_" + Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario + ".jpg";
            var stream = Request.InputStream;
            string dumb;
            using (var reader = new StreamReader(stream))
            {
                dumb = reader.ReadToEnd();
            }
            var path = Server.MapPath(ruta_imagen_webcam);
            System.IO.File.WriteAllBytes(path, String_To_Bytes2(dumb));
        }

        public ActionResult OtenerImagenCamara()
        {
            try
            {
                string ruta_imagen_webcam = "~/Archivos/Temp/webcam_picture_" + Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario + ".jpg";
                var image_byte = System.IO.File.ReadAllBytes(Server.MapPath(ruta_imagen_webcam));
                return File(image_byte, "image/.jpg");
            }
            catch { return HttpNotFound(); }
        }

        private byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;
            byte[] bytes = new byte[numBytes];
            for (int x = 0; x < numBytes; x++)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }
            return bytes;
        }

        public ActionResult Camara()
        {
            ViewBag.id = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return View();
        }

        public ActionResult WebCam()
        {
            return View();
        }

        #endregion

        #region Liquidacion

        //Liquidacion
        public ActionResult CalcularLiquidacion(int? id_contratacion)
        {
            if (id_contratacion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratacion contrato = db.Contratacion.SingleOrDefault(s=>s.id_contratacion == id_contratacion && !s.eliminado);
            if (contrato == null)
            {
                return HttpNotFound();
            }
            ViewBag.empleado = contrato.Empleado.primer_nombre + " " + 
                contrato.Empleado.segundo_nombre + " " + 
                contrato.Empleado.primer_apellido + " " + 
                contrato.Empleado.segundo_apellido;
            ViewBag.empleado = contrato.Empleado.primer_nombre + " " +
                (!String.IsNullOrEmpty(contrato.Empleado.segundo_nombre) ? contrato.Empleado.segundo_nombre + " " : "") +
                contrato.Empleado.primer_apellido + " " +
                (!String.IsNullOrEmpty(contrato.Empleado.segundo_apellido) ? contrato.Empleado.segundo_apellido : "");

            ViewBag.id_empleado = contrato.id_empleado;
            ViewBag.id_contratacion = contrato.id_contratacion;
            ViewBag.estado_empleado = contrato.id_estado_empleado;
            return View();
        }

        public void documento_liquidacion(int id_empleado, DateTime fecha, int dias)
        {
            fn_Liquidacion_xls_Result liquidacion = db.fn_Liquidacion_xls(fecha, id_empleado, dias).Single();
            int usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            //Generado de XLS en base a template
            //FileInfo templateFile = new FileInfo(@"C:\Users\DESAROLLADOR 3\Desktop\Template_Liquidacion.xlsx");
            FileInfo templateFile = new FileInfo(Server.MapPath("~/Archivos/Templates/Template_Liquidacion.xlsx"));
            // Making a new file 'Sample2.xlsx'
            //FileInfo newFile = new FileInfo(@"C:\Liquidaciones\" + id_empleado + "_liquidacion.xlsx");
            FileInfo newFile = new FileInfo(Server.MapPath("~/Archivos/Temp/" + id_empleado + "_" + usuario + "_liquidacion.xlsx"));
            if (newFile.Exists)
            {
                newFile.Delete();
                //newFile = new FileInfo(@"C:\Liquidaciones\" + id_empleado + "_liquidacion.xlsx");
                newFile = new FileInfo(Server.MapPath("~/Archivos/Temp/" + id_empleado + "_" + usuario + "_liquidacion.xlsx"));
            }
            using (ExcelPackage package = new ExcelPackage(newFile, templateFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                worksheet.Cells["D4"].Value = liquidacion.empleado;
                worksheet.Cells["D5"].Value = liquidacion.puesto;
                worksheet.Cells["D6"].Value = liquidacion.fecha_inicio;
                worksheet.Cells["D7"].Value = liquidacion.sueldo_base;
                worksheet.Cells["D8"].Value = liquidacion.bonificacion;
                worksheet.Cells["D9"].Value = liquidacion.fecha_liquidacion;
                worksheet.Cells["D10"].Value = liquidacion.dias_laborados;

                worksheet.Cells["D13"].Value = worksheet.Cells["D14"].Value
                    = worksheet.Cells["D15"].Value = worksheet.Cells["D16"].Value =
                    worksheet.Cells["D17"].Value = worksheet.Cells["D18"].Value = liquidacion.sueldo_base;

                worksheet.Cells["B22"].Value = liquidacion.periodo_aguinaldo;
                worksheet.Cells["B28"].Value = liquidacion.periodo_bono_14;
                worksheet.Cells["B35"].Value = liquidacion.sueldo_base;
                worksheet.Cells["E35"].Value = liquidacion.years;
                worksheet.Cells["E36"].Value = liquidacion.meses;
                worksheet.Cells["E37"].Value = liquidacion.dias;
                worksheet.Cells["B46"].Value = dias;

                worksheet.Cells["G25"].Value = liquidacion.aguinaldo;
                worksheet.Cells["G31"].Value = liquidacion.bono_14;
                worksheet.Cells["G38"].Value = liquidacion.indeminizacion_total;
                worksheet.Cells["G48"].Value = liquidacion.vacaciones;
                worksheet.Cells["G55"].Value = liquidacion.descuentos;
                worksheet.Cells["G62"].Value = liquidacion.descuentos;
                package.Save();


            }

        }

        public ActionResult Downloads(int id)
        {
            int usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            //var filePath = @"C:\Liquidaciones\" + id + "_liquidacion.xlsx";
            var filePath = Server.MapPath("~/Archivos/Temp/" + id + "_" + usuario + "_liquidacion.xlsx");
            var fileName = id + "_" + usuario + @"_liquidacion.xlsx";
            var mimeType = "application/vnd.ms-excel";
            return File(new FileStream(filePath, FileMode.Open), mimeType, fileName);
        }

        public JsonResult Calculo_Liquidación(int id_empleado, DateTime fecha, int id_contratacion)
        {

            var resultado = new object { };
            try
            {
                fn_Liquidacion_Result liquidacion = db.fn_Liquidacion(fecha, id_empleado, id_contratacion).Single();
                resultado =
                new
                {
                    resultado = 1,
                    id_empleado = liquidacion.id_empleado,
                    fecha_inicio = liquidacion.fecha_inicio.Value.ToString("dd/MM/yyyy"),
                    fecha_ultimo_pago = liquidacion.fecha_ultimo_pago.Value.ToString("dd/MM/yyyy"),
                    vacaciones = FormatUtil.getCurrencyString(liquidacion.vacaciones ?? 0),
                    indeminizacion_total = FormatUtil.getCurrencyString(liquidacion.indeminizacion_total ?? 0),
                    sueldo_pendiente = FormatUtil.getCurrencyString(liquidacion.sueldo_pendiente ?? 0),
                    aguinaldo = FormatUtil.getCurrencyString(liquidacion.aguinaldo ?? 0),
                    bono_14 = FormatUtil.getCurrencyString(liquidacion.bono_14 ?? 0),
                    descuentos = FormatUtil.getCurrencyString(liquidacion.descuentos ?? 0),
                    total = FormatUtil.getCurrencyString(liquidacion.total ?? 0)

                };

            }
            catch
            {
                resultado = new
                {
                    resultado = 0
                };
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CalcularLiquidacion(int id_empleado, DateTime fecha_ultimo_pago, int vacaciones_pendientes)
        {
            var resultado = new object { };
            try
            {
                documento_liquidacion(id_empleado, fecha_ultimo_pago, vacaciones_pendientes);
                //Downloads(id_empleado);

                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                fn_Liquidacion_Result liquidacion = db.fn_Liquidacion(fecha_ultimo_pago, id_empleado, vacaciones_pendientes).Single();
                
                Liquidaciones liquidaciones = new Liquidaciones();
                liquidaciones.id_empleado = Convert.ToInt32(liquidacion.id_empleado);
                liquidaciones.vacaciones_pendientes = Convert.ToDecimal(FormatUtil.getDecimalFromString(FormatUtil.getCurrencyString(liquidacion.vacaciones ?? 0)), CultureInfo.InvariantCulture);
                liquidaciones.fecha_ultimo_pago = Convert.ToDateTime(liquidacion.fecha_ultimo_pago);
                liquidaciones.indeminizacion = Convert.ToDecimal(FormatUtil.getDecimalFromString(FormatUtil.getCurrencyString(liquidacion.indeminizacion_total ?? 0)), CultureInfo.InvariantCulture);
                liquidaciones.sueldo_pendiente = Convert.ToDecimal(FormatUtil.getDecimalFromString(FormatUtil.getCurrencyString(liquidacion.sueldo_pendiente ?? 0)), CultureInfo.InvariantCulture);
                liquidaciones.bono_14_pendiente = Convert.ToDecimal(FormatUtil.getDecimalFromString(FormatUtil.getCurrencyString(liquidacion.bono_14 ?? 0)), CultureInfo.InvariantCulture);
                liquidaciones.aguinaldo_pendiente = Convert.ToDecimal(FormatUtil.getDecimalFromString(FormatUtil.getCurrencyString(liquidacion.aguinaldo ?? 0)), CultureInfo.InvariantCulture);
                liquidaciones.Total = Convert.ToDecimal(FormatUtil.getDecimalFromString(FormatUtil.getCurrencyString(liquidacion.total ?? 0)), CultureInfo.InvariantCulture);
                liquidaciones.activo = true;
                liquidaciones.eliminado = false;
                liquidaciones.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                liquidaciones.fecha_creacion = DateTime.Now;

                db.Liquidaciones.Add(liquidaciones);
                db.SaveChanges();
                //return RedirectToAction("Index");
                /*var filePath = @"C:\Liquidaciones\" + id_empleado + "_liquidacion.xlsx";
                var fileName = id_empleado + @"_liquidacion.xlsx";
                var mimeType = "application/vnd.ms-excel";*/
                var filePath = Server.MapPath("~/Archivos/Temp/" + id_empleado + "_" + usuarioTO.usuario.id_usuario + "_liquidacion.xlsx");
                var fileName = id_empleado + "_" + usuarioTO.usuario.id_usuario + @"_liquidacion.xlsx";
                var mimeType = "application/vnd.ms-excel";
                return File(new FileStream(filePath, FileMode.Open), mimeType, fileName);

            }
            catch
            {
                return View("Index");
            }
        }



        #endregion

        #region CRUD
        // GET: Planilla/Empleado
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AlertaFaltasEmpleados()
        {
            var empleados = db.Alerta_Faltas_Empleado();
            return View(empleados);
        }

        public ActionResult FotosSeleccion()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FotosSeleccion(String empleados)
        {
            List<string> list = new List<string>(
                           empleados.Split(new string[] { "\r\n" },
                           StringSplitOptions.RemoveEmptyEntries));
            List<int> listaEmpleados = new List<int>();

            foreach (string item in list)
            {
                int empleado = 0;
                if (int.TryParse(item, out empleado))
                {
                    listaEmpleados.Add(empleado);
                }
            }

            List<Empleado> empleadosDb = db.Empleado.Where(e => listaEmpleados.Contains(e.id_empleado)).ToList();

            ViewBag.paginacion = false;
            ViewBag.cantidad_paginas = 1;
            return View("Fotos", empleadosDb);

            //return View();
        }

        public ActionResult Fotos(int? id)
        {
            int pagina = 1;
            int buffer = 1000;
            if (id == null || id == 0)
            {
                id = 0;
                pagina = buffer;
            }
            else
            {
                pagina = buffer * (int)id;
            }

            //List<Empleado> empleados = db.Empleado.Where(e => !e.eliminado && e.id_empleado > id).OrderBy(x => x.id_empleado).Take(pagina).ToList().OrderByDescending(x=>x.id_empleado).Take(buffer).ToList();
            List<Empleado> empleados = db.Empleado.Where(e => !e.eliminado).OrderBy(x => x.id_empleado).Take(pagina).ToList().OrderByDescending(x => x.id_empleado).Take(buffer).ToList().OrderBy(x => x.id_empleado).ToList();

            ViewBag.paginacion = true;
            ViewBag.cantidad_paginas = db.Empleado.Where(e => !e.eliminado).Count() / 1000;
            return View(empleados);
        }

        public JsonResult Search(int? id_empleado, string primer_apellido, string primer_nombre, string segundo_apellido, string segundo_nombre, long? dpi)
        {
            List<object> resultados = new List<object>();
            var empleados = db.Empleado.Where(e => !e.eliminado);
            if (id_empleado.HasValue)
            {
                empleados = empleados.Where(e => e.id_empleado == id_empleado.Value && e.activo && !e.eliminado);
            }
            if (!String.IsNullOrEmpty(primer_apellido))
            {
                empleados = empleados.Where(e => e.primer_apellido.Contains(primer_apellido.Trim()));
            }
            if (!String.IsNullOrEmpty(primer_nombre))
            {
                empleados = empleados.Where(e => e.primer_nombre.Contains(primer_nombre.Trim()));
            }
            if (!String.IsNullOrEmpty(segundo_nombre))
            {
                empleados = empleados.Where(e => e.segundo_nombre.Contains(segundo_nombre.Trim()));
            }
            if (!String.IsNullOrEmpty(segundo_apellido))
            {
                empleados = empleados.Where(e => e.segundo_apellido.Contains(segundo_apellido.Trim()));
            }
            if (dpi.HasValue)
            {
                empleados = empleados.Where(e => e.dpi == dpi.Value);
            }
            foreach (var empleado in empleados.Take(100))
            {
                resultados.Add(new { empleado.id_empleado, empleado.primer_nombre, 
                    segundo_nombre = String.IsNullOrEmpty(empleado.segundo_nombre) ? "": empleado.segundo_nombre, 
                    empleado.primer_apellido,
                    segundo_apellido = String.IsNullOrEmpty(empleado.segundo_apellido) ? "" : empleado.segundo_apellido, 
                    empleado.dpi });
            }
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        public String EmptyString(string cadena)
        {
            if (!String.IsNullOrEmpty(cadena))
            {
                return cadena;
            }
            return String.Empty;
        }

        // POST: Planilla/Empleado/Delete/
        public ActionResult BorrarEmpleado(int id_empleado)
        {
            Empleado empleado = db.Empleado.Find(id_empleado);
            empleado.fecha_eliminacion = DateTime.Now;
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            empleado.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            empleado.eliminado = true;
            empleado.activo = false;
            db.Entry(empleado).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult ExisteEmpleado(int id_empleado)
        {
            object resultado = new object();
            Empleado empleado = db.Empleado.SingleOrDefault(e => e.activo && e.id_empleado == id_empleado);
            if (empleado == null)
            {
                resultado = new { error = true };
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            resultado = new { error = false, id_empleado, nombre = 
               empleado.primer_nombre + " " +
                (!String.IsNullOrEmpty(empleado.segundo_nombre) ? empleado.segundo_nombre + " " : "") +
                empleado.primer_apellido + " " +
                (!String.IsNullOrEmpty(empleado.segundo_apellido) ? empleado.segundo_apellido : "")
            };
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarEmpleado(int id_empleado)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Empleado empleado = db.Empleado.SingleOrDefault(e=>e.id_empleado == id_empleado && e.activo && !e.eliminado);
                    if (empleado == null)
                    {
                        tran.Rollback();
                        return Json(new { msg = "Empleado no encontrado.", response = false });
                    }
                    empleado.fecha_eliminacion = DateTime.Now;
                    UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                    empleado.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                    empleado.eliminado = true;
                    empleado.activo = false;
                    db.Entry(empleado).State = EntityState.Modified;
                    //Terminar contrato y vacaciones pendientes
                    TerminarContrato(id_empleado);
                    db.SaveChanges();
                    return Json(new { msg = "", response = true });
                }
                catch
                {
                    tran.Rollback();
                    return Json(new { msg = "Cambios no guardados.", response = false });
                }
            }
        }

        public void TerminarContrato(int id_empleado)
        {
            Contratacion contratacion = db.Contratacion.SingleOrDefault(c => c.activo && !c.eliminado && c.id_empleado == id_empleado);
            if (contratacion != null)
            {
                contratacion.activo = false;
                contratacion.eliminado = true;
                contratacion.fecha_eliminacion = DateTime.Now;
                contratacion.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Entry(contratacion).State = EntityState.Modified;
                foreach (var item in contratacion.Vacacion_Contrato.Where(e=>e.activo && !e.eliminado))
                {
                    item.activo = false;
                    item.eliminado = true;
                    item.fecha_eliminacion = DateTime.Now;
                    item.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(item).State = EntityState.Modified;
                }
            }
        }
        
        // GET: Planilla/Empleado/Details/5
        public ActionResult Details(int? id, string error)
        {
            ModelState.Clear();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado empleado = db.Empleado.SingleOrDefault(e => e.id_empleado == id && e.activo && !e.eliminado);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            if (!String.IsNullOrEmpty(error))
            {
                ModelState.AddModelError("", error);
            }
            //Implementar envio de la foto de perfil
            List<Archivo_Empleado> archivo_empleado = db.Archivo_Empleado.Where(e => !e.eliminado).Where(e => e.id_empleado == id).ToList();
            ViewBag.archivos = archivo_empleado;
            ViewBag.id_tipo_telefono = new SelectList(db.Tipo_Telefono, "id_tipo_telefono", "nombre", "Seleccionar");
            ViewBag.id_tipo_direccion = new SelectList(db.Tipo_Direccion, "id_tipo_direccion", "nombre");
            ViewBag.id_banco_cuenta = new SelectList(db.Banco, "id_banco", "nombre", empleado.id_banco_cuenta);
            ViewBag.id_estado_civil = new SelectList(db.Estado_Civil, "id_estado_civil", "nombre", empleado.id_estado_civil);
            ViewBag.id_municipio_dpi = new SelectList(db.Municipios.Where(m => !m.eliminado).OrderBy(m => m.nombre).Select(e => new { e.id_municipio, nombre = e.nombre + " - " + e.Departamentos.nombre }), "id_municipio", "nombre", empleado.id_municipio_dpi);
            ViewBag.id_municipio = new SelectList(db.Municipios.Where(m => !m.eliminado).OrderBy(m => m.nombre).Select(e => new { e.id_municipio, nombre = e.nombre + " - " + e.Departamentos.nombre }), "id_municipio", "nombre");
            //ViewBag.id_departamento = new SelectList(db.Departamentos, "id_departamento", "nombre");
            ViewBag.id_pais_nacionalidad = new SelectList(db.Paises.Where(e => !e.eliminado), "id_pais", "nombre", empleado.id_pais_nacionalidad);
            //ViewBag.id_pais = new SelectList(db.Paises.Where(e => !e.eliminado), "id_pais", "nombre");
            ViewBag.id_profesion = new SelectList(db.Profesion, "id_profesion", "nombre", empleado.id_profesion);
            ViewBag.id_tipo_cuenta = new SelectList(db.Tipo_Cuenta, "id_tipo_cuenta", "nombre", empleado.id_tipo_cuenta);
            ViewBag.id_tipo_licencia = new SelectList(db.Tipo_Licencia, "id_tipo_licencia", "nombre", empleado.id_tipo_licencia);
            ViewBag.id_grado_estudio = new SelectList(db.Grado_Estudio, "id_grado_estudio", "nombre", empleado.id_grado_estudio);
            ViewBag.fecha_nacimiento = empleado.fecha_nacimiento.ToString("dd/MM/yyyy");
            ViewBag.id_genero = new SelectList(db.Genero.Where(e => !e.eliminado), "id_genero", "nombre", empleado.id_genero == null ? 0 : empleado.id_genero);
            ViewBag.tipo_pago = new SelectList(db.Tipo_Pago.Where(e => !e.eliminado), "id_tipo_pago", "nombre", empleado.id_tipo_pago);
            ViewBag.gyt = (int)Catalogos.Banco.GyT;
            if (empleado.fecha_vencimiento_licencia.HasValue)
            {
                ViewBag.fecha_vencimiento_licencia = empleado.fecha_vencimiento_licencia.Value.ToString("dd/MM/yyyy");
            }
            ViewBag.id_tipo_archivo = new SelectList(db.Tipo_Archivo, "id_tipo_archivo", "nombre");
            //Elimanar la imagen de la camara web temporal que exista en el servidor
            try
            {
                System.IO.File.Delete(Server.MapPath("~/Archivos/Temp/webcam_picture_" + Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario + ".jpg"));
            }
            catch
            { }

            //Enviar empleado y su contrato a la vista
            EmpleadoContrato ec = new EmpleadoContrato();
            ec.empleado = empleado;
            ec.contrato = db.Contratacion.SingleOrDefault(c => c.id_empleado == id && c.activo && !c.eliminado);
            ViewBag.motivo_baja = new SelectList(db.Motivo_Baja.Where(m => !m.eliminado), "id_motivo_baja", "descripcion");
            return View(ec);
        }

        // GET: Planilla/Empleado/Edit/5
        [HttpPost]
        public ActionResult EditEmpleado(EmpleadoContrato ec, HttpPostedFileBase imagen_cedula, int tipo_foto)
        {
            ModelState.Clear();
            Empleado empleadoEdit = db.Empleado.SingleOrDefault(e => e.id_empleado == ec.empleado.id_empleado && e.activo);
            if (empleadoEdit == null)
            {
                return HttpNotFound();
            }
            //Validar que todos los datos respecto a la cuenta bancaria este completos o vacios
            if (ec.empleado.id_banco_cuenta != null || ec.empleado.id_tipo_cuenta != null || !String.IsNullOrWhiteSpace(ec.empleado.numero_cuenta))
            {
                if (ec.empleado.id_banco_cuenta == null || ec.empleado.id_tipo_cuenta == null || String.IsNullOrWhiteSpace(ec.empleado.numero_cuenta))
                {
                    return RedirectToAction("Details", new { id = ec.empleado.id_empleado, error = "La información de la cuenta bancaria no puede estar incompleta." });
                }
            }
            if (ec.empleado.id_tipo_licencia != null || ec.empleado.fecha_vencimiento_licencia != null || ec.empleado.numero_licencia != null)
            {
                if (ec.empleado.id_tipo_licencia == null || ec.empleado.fecha_vencimiento_licencia == null || ec.empleado.numero_licencia == null)
                {
                    return RedirectToAction("Details", new { id = ec.empleado.id_empleado, error = "La información de la licencia no puede estar incompleta." });
                }
            }
            if (ModelState.IsValid)
            {
                int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        empleadoEdit.fecha_modificacion = DateTime.Now;
                        empleadoEdit.digeesp = ec.empleado.digeesp;
                        empleadoEdit.id_usuario_modificacion = id_usuario;
                        empleadoEdit.primer_nombre = ec.empleado.primer_nombre.ToUpper();
                        empleadoEdit.primer_apellido = ec.empleado.primer_apellido.ToUpper();
                        empleadoEdit.segundo_nombre = !String.IsNullOrEmpty(ec.empleado.segundo_nombre) ? ec.empleado.segundo_nombre.ToUpper() : "";
                        empleadoEdit.segundo_apellido = !String.IsNullOrEmpty(ec.empleado.segundo_apellido) ? ec.empleado.segundo_apellido.ToUpper() : "";
                        empleadoEdit.apellido_casada = !String.IsNullOrEmpty(ec.empleado.apellido_casada) ? ec.empleado.apellido_casada.ToUpper() : "";
                        empleadoEdit.correo = !String.IsNullOrEmpty(ec.empleado.correo) ? ec.empleado.correo.ToUpper() : "";
                        empleadoEdit.pasaporte = !String.IsNullOrEmpty(ec.empleado.pasaporte) ? ec.empleado.pasaporte.ToUpper() : ""; 
                        empleadoEdit.id_genero = ec.empleado.id_genero;
                        empleadoEdit.comentario = !(String.IsNullOrEmpty(ec.empleado.comentario)) ? ec.empleado.comentario.ToUpper() : "";
                        empleadoEdit.servicio_militar = ec.empleado.servicio_militar;
                        empleadoEdit.fecha_nacimiento = ec.empleado.fecha_nacimiento; empleadoEdit.sabe_leer = ec.empleado.sabe_leer; empleadoEdit.sabe_escribir = ec.empleado.sabe_escribir;
                        empleadoEdit.estatura = ec.empleado.estatura; empleadoEdit.peso = ec.empleado.peso; empleadoEdit.id_estado_civil = ec.empleado.id_estado_civil;
                        empleadoEdit.id_profesion = ec.empleado.id_profesion; empleadoEdit.id_grado_estudio = ec.empleado.id_grado_estudio; empleadoEdit.dpi = ec.empleado.dpi;
                        empleadoEdit.id_municipio_dpi = ec.empleado.id_municipio_dpi; empleadoEdit.id_tipo_licencia = ec.empleado.id_tipo_licencia; empleadoEdit.numero_licencia = ec.empleado.numero_licencia;
                        empleadoEdit.fecha_vencimiento_licencia = ec.empleado.fecha_vencimiento_licencia; empleadoEdit.nit = ec.empleado.nit; empleadoEdit.afiliacion_igss = ec.empleado.afiliacion_igss;
                        empleadoEdit.afiliacion_irtra = ec.empleado.afiliacion_irtra; empleadoEdit.id_pais_nacionalidad = ec.empleado.id_pais_nacionalidad;
                        empleadoEdit.id_tipo_cuenta = ec.empleado.id_tipo_cuenta; empleadoEdit.numero_cuenta = ec.empleado.numero_cuenta;
                        empleadoEdit.id_tipo_pago = ec.empleado.id_tipo_pago;
                        empleadoEdit.id_banco_cuenta = ec.empleado.id_banco_cuenta;
                        empleadoEdit.acreditamiento = ec.empleado.acreditamiento;
                        db.Entry(empleadoEdit).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        ContextMessage msg = new ContextMessage(ContextMessage.Error, "Ocurrio un error inesperado guardando la información del empleado. Cambios no efectuados. Error: " + ex.Message);
                        msg.ReturnUrl = Url.Action("Details", new { id = ec.empleado.id_empleado });
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                }
                //Modificar la imagen de perfil
                if (tipo_foto > 0)
                {
                    using (DbContextTransaction tran = db.Database.BeginTransaction())
                    {
                        try
                        {
                            string ruta_imagen_webcam = "~/Archivos/Temp/webcam_picture_" + id_usuario + ".jpg";
                            var archivo_list = db.Archivo_Empleado.Where(e => !e.eliminado && e.id_empleado == ec.empleado.id_empleado && e.Archivo.id_tipo_archivo == (int)Catalogos.Tipo_Archivo.Foto_Perfil).Select(e => e.Archivo);
                            if (archivo_list.Count() == 0) //El empleado no tenia una imagen de perfil previamente
                            {
                                if (tipo_foto == 1)  //nueva imagen de perfil desde el input file
                                {
                                    Archivo imagenPerfil = new Archivo();
                                    string name = imagen_cedula.FileName;
                                    string extension = name.Substring(name.LastIndexOf('.'));
                                    name = "foto_" + ec.empleado.id_empleado;
                                    imagenPerfil.id_usuario_creacion = id_usuario;
                                    imagenPerfil.fecha_creacion = DateTime.Now;
                                    imagenPerfil.activo = true;
                                    imagenPerfil.eliminado = false;
                                    imagenPerfil.nombre = name + extension;
                                    imagenPerfil.id_tipo_archivo = (int)Catalogos.Tipo_Archivo.Foto_Perfil; 
                                    imagenPerfil.ubicacion = "~/Archivos/Foto Perfil/" + name + extension;

                                    db.Archivo.Add(imagenPerfil);
                                    db.SaveChanges();

                                    imagen_cedula.SaveAs(Server.MapPath("~/Archivos/Foto Perfil/" + name + extension));

                                    //Relacionar la imagen con el usuario
                                    Archivo_Empleado nuevo_archivo_empleado = new Archivo_Empleado();
                                    nuevo_archivo_empleado.id_archivo = imagenPerfil.id_archivo;
                                    nuevo_archivo_empleado.id_empleado = ec.empleado.id_empleado;
                                    nuevo_archivo_empleado.id_usuario_creacion = id_usuario;
                                    nuevo_archivo_empleado.fecha_creacion = DateTime.Now;
                                    nuevo_archivo_empleado.activo = true;
                                    nuevo_archivo_empleado.eliminado = false;
                                    db.Archivo_Empleado.Add(nuevo_archivo_empleado);
                                    db.SaveChanges();
                                }
                                else if (tipo_foto == 2) //nueva imagen desde la camara web
                                {
                                    Archivo imagenPerfil = new Archivo();
                                    string name = "foto_" + ec.empleado.id_empleado;
                                    string extension = ".jpg";
                                    imagenPerfil.id_usuario_creacion = id_usuario;
                                    imagenPerfil.fecha_creacion = DateTime.Now;
                                    imagenPerfil.activo = true;
                                    imagenPerfil.eliminado = false;
                                    imagenPerfil.nombre = name + extension;
                                    imagenPerfil.id_tipo_archivo = (int)Catalogos.Tipo_Archivo.Foto_Perfil;
                                    imagenPerfil.ubicacion = "~/Archivos/Foto Perfil/" + name + extension;

                                    db.Archivo.Add(imagenPerfil);
                                    db.SaveChanges();

                                    System.IO.File.Copy(Server.MapPath(ruta_imagen_webcam), Server.MapPath(imagenPerfil.ubicacion), true);

                                    //Relacionar la imagen con el usuario
                                    Archivo_Empleado nuevo_archivo_empleado = new Archivo_Empleado();
                                    nuevo_archivo_empleado.id_archivo = imagenPerfil.id_archivo;
                                    nuevo_archivo_empleado.id_empleado = ec.empleado.id_empleado;
                                    nuevo_archivo_empleado.id_usuario_creacion = id_usuario;
                                    nuevo_archivo_empleado.fecha_creacion = DateTime.Now;
                                    nuevo_archivo_empleado.activo = true;
                                    nuevo_archivo_empleado.eliminado = false;
                                    db.Archivo_Empleado.Add(nuevo_archivo_empleado);
                                    db.SaveChanges();
                                }
                            }
                            else //El empleado tiene una imagen de perfil
                            {
                                var archivo = archivo_list.First();
                                if (tipo_foto == 1) //modificar de perfil desde el input file
                                {
                                    string name = imagen_cedula.FileName;
                                    string extension = name.Substring(name.LastIndexOf('.'));
                                    name = "foto_" + ec.empleado.id_empleado;
                                    System.IO.File.Delete(Server.MapPath(archivo.ubicacion));
                                    imagen_cedula.SaveAs(Server.MapPath("~/Archivos/Foto Perfil/" + name + extension));
                                    archivo.nombre = name + extension;
                                    archivo.ubicacion = "~/Archivos/Foto Perfil/" + name + extension;
                                    archivo.fecha_modificacion = DateTime.Now;
                                    archivo.id_usuario_modificacion = id_usuario;
                                    db.Entry(archivo).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else if (tipo_foto == 2) //nueva imagen desde la camara web
                                {
                                    archivo.nombre = "foto_" + ec.empleado.id_empleado + ".jpg";
                                    archivo.ubicacion = "~/Archivos/Foto_Perfil/" + archivo.nombre;
                                    System.IO.File.Copy(Server.MapPath(ruta_imagen_webcam), Server.MapPath(archivo.ubicacion), true);
                                    archivo.fecha_modificacion = DateTime.Now;
                                    archivo.id_usuario_modificacion = id_usuario;
                                    db.Entry(archivo).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            ContextMessage msg = new ContextMessage(ContextMessage.Error, "No se pudo modificar correctamente la imagen del empleado.");
                            msg.ReturnUrl = Url.Action("Details", new { id = ec.empleado.id_empleado });
                            TempData[User.Identity.Name] = msg;
                            return RedirectToAction("Mensaje");
                            //return RedirectToAction("Details", new { id = ec.empleado.id_empleado, error = "No se pudo modificar correctamente la imagen del empleado." });
                        }
                    }
                }
            }
            return RedirectToAction("Details", new { id = ec.empleado.id_empleado, error = "" });
        }


        // GET: Planilla/Empleado/Create
        public ActionResult Create()
        {
            ViewBag.banco_cuenta = new SelectList(db.Banco.Where(b => !b.eliminado).OrderBy(b => b.nombre), "id_banco", "nombre");
            ViewBag.estado_civil = new SelectList(db.Estado_Civil.Where(e => !e.eliminado).OrderBy(e => e.nombre), "id_estado_civil", "nombre");
            ViewBag.municipio = new SelectList(db.Municipios.Where(m => !m.eliminado).OrderBy(m => m.nombre).Select(e => new { e.id_municipio, nombre = e.nombre + " - " + e.Departamentos.nombre }), "id_municipio", "nombre");
            ViewBag.departamento = new SelectList(db.Departamentos.Where(d => !d.eliminado).OrderBy(d => d.nombre), "id_departamento", "nombre");
            ViewBag.pais = new SelectList(db.Paises.Where(p => !p.eliminado).OrderBy(p => p.nombre), "id_pais", "nombre");
            ViewBag.profesion = new SelectList(db.Profesion.Where(p => !p.eliminado).OrderBy(p => p.nombre), "id_profesion", "nombre");
            ViewBag.tipo_cuenta = new SelectList(db.Tipo_Cuenta.Where(t => !t.eliminado).OrderBy(t => t.nombre), "id_tipo_cuenta", "nombre");
            ViewBag.tipo_licencia = new SelectList(db.Tipo_Licencia.Where(t => !t.eliminado).OrderBy(t => t.nombre), "id_tipo_licencia", "nombre");
            ViewBag.estado_empleado = new SelectList(db.Estado_Empleado.Where(e => !e.eliminado).OrderBy(e => e.nombre), "id_estado_empleado", "nombre");
            ViewBag.grado_estudio = new SelectList(db.Grado_Estudio.Where(g => !g.eliminado).OrderBy(g => g.nombre), "id_grado_estudio", "nombre");
            ViewBag.tipo_direccion = new SelectList(db.Tipo_Direccion.Where(t => t.activo).OrderBy(t => t.nombre), "id_tipo_direccion", "nombre");
            ViewBag.tipo_telefono = new SelectList(db.Tipo_Telefono.Where(t => t.activo).OrderBy(t => t.nombre), "id_tipo_telefono", "nombre");
            ViewBag.municipios = new SelectList(db.Municipios.Where(m => m.activo).OrderBy(m => m.nombre).Select(e => new { e.id_municipio, nombre = e.nombre + " - " + e.Departamentos.nombre }), "id_municipio", "nombre");
            ViewBag.genero = new SelectList(db.Genero.Where(e => !e.eliminado), "id_genero", "nombre");
            ViewBag.tipo_pago = new SelectList(db.Tipo_Pago.Where(e => !e.eliminado), "id_tipo_pago", "nombre");
            ViewBag.gyt = (int)Catalogos.Banco.GyT;
            //Eliminar la imagen temporal de la camara web que exista en el servidor
            string ruta_imagen_webcam = "~/Archivos/Temp/webcam_picture_" + Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario + ".jpg";
            try
            {
                System.IO.File.Delete(Server.MapPath(ruta_imagen_webcam));
            }
            catch
            { }
            return View();
        }

        // POST: Planilla/Empleado/Create
        [HttpPost]
        public ActionResult Create(Empleado empleado, HttpPostedFileBase imagen_cedula, int tipo_foto, DateTime fecha_nacimiento, DateTime? fecha_vencimiento_licencia)
        {
            ModelState.Clear();
            //Validar que todos los datos respecto a la cuenta bancaria este completos o vacios
            if (empleado.id_banco_cuenta != null || empleado.id_tipo_cuenta != null || !String.IsNullOrWhiteSpace(empleado.numero_cuenta))
            {
                if (empleado.id_banco_cuenta == null || empleado.id_tipo_cuenta == null || String.IsNullOrWhiteSpace(empleado.numero_cuenta))
                {
                    ModelState.AddModelError("", "La información de la cuenta bancaria no puede estar incompleta.");
                }
            }
            if (empleado.id_tipo_licencia != null || fecha_vencimiento_licencia != null || empleado.numero_licencia != null)
            {
                if (empleado.id_tipo_licencia == null || fecha_vencimiento_licencia == null || empleado.numero_licencia == null)
                {
                    ModelState.AddModelError("", "La información de la licencia no puede estar incompleta.");
                }
            }
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //Empleado
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        empleado.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        empleado.fecha_creacion = DateTime.Now;
                        empleado.activo = true;
                        empleado.fecha_nacimiento = fecha_nacimiento;
                        empleado.fecha_vencimiento_licencia = fecha_vencimiento_licencia;
                        empleado.eliminado = false;
                        empleado.primer_apellido = empleado.primer_apellido.ToUpper();
                        empleado.primer_nombre = empleado.primer_nombre.ToUpper();
                        if (!String.IsNullOrEmpty(empleado.segundo_apellido))
                        {
                            empleado.segundo_apellido = empleado.segundo_apellido.ToUpper();
                        }
                        if (!String.IsNullOrEmpty(empleado.segundo_nombre))
                        {
                            empleado.segundo_nombre = empleado.segundo_nombre.ToUpper();
                        }
                        if (!String.IsNullOrEmpty(empleado.apellido_casada))
                        {
                            empleado.apellido_casada = empleado.apellido_casada.ToUpper();
                        }
                        if (!String.IsNullOrEmpty(empleado.correo))
                        {
                            empleado.correo = empleado.correo.ToUpper();
                        }
                        if (!String.IsNullOrEmpty(empleado.pasaporte))
                        {
                            empleado.pasaporte = empleado.pasaporte.ToUpper();
                        }
                        if (!String.IsNullOrEmpty(empleado.comentario))
                        {
                            empleado.comentario = empleado.comentario.ToUpper();
                        }
                        empleado.id_estado_empleado = (int)Catalogos.Estado_Empleado.Alta;
                        db.Empleado.Add(empleado);
                        db.SaveChanges();

                        //Obtener telefonos
                        foreach (var identifier in Request.Form.AllKeys.Where(k => k.Contains("extension_")).Select(k => k.Replace("extension_", "")))
                        {
                            Empleado_Telefono empleado_telefono = NuevoTelefono();
                            empleado_telefono.telefono = Convert.ToInt32(Request["telefono_" + identifier]);
                            if (!String.IsNullOrEmpty(Request["extension_" + identifier]))
                            {
                                empleado_telefono.extension = Convert.ToInt32(Request["extension_" + identifier]);
                            }
                            if (!String.IsNullOrEmpty(Request["comentario_" + identifier]))
                            {
                                empleado_telefono.comentario = Request["comentario_" + identifier].ToUpper();
                            }
                            empleado_telefono.id_tipo_telefono = Convert.ToInt32(Request["tipo_telefono_" + identifier]);
                            empleado_telefono.id_empleado = empleado.id_empleado;
                            db.Empleado_Telefono.Add(empleado_telefono);
                        }
                        //Obtener direcciones
                        foreach (var identifier in Request.Form.AllKeys.Where(k => k.Contains("tipo_direccion_")).Select(k => k.Replace("tipo_direccion_", "")))
                        {
                            Empleado_Direcciones empleado_direccion = NuevaDireccion();
                            if (!String.IsNullOrEmpty(Request["direccion_" + identifier]))
                            {
                                empleado_direccion.direccion = Request["direccion_" + identifier];
                            }
                            empleado_direccion.id_tipo_direccion = Convert.ToInt32(Request["tipo_direccion_" + identifier]);
                            empleado_direccion.id_municipio = Convert.ToInt32(Request["municipio_" + identifier]);
                            empleado_direccion.id_empleado = empleado.id_empleado;
                            db.Empleado_Direcciones.Add(empleado_direccion);
                        }
                        //Obtener experiencias laborales
                        foreach (var identifier in Request.Form.AllKeys.Where(k => k.Contains("el_empresa_")).Select(k => k.Replace("el_empresa_", "")))
                        {
                            Empleado_Experiencia_Laboral empleado_experiencia = NuevaExperienciaLaboral();

                            empleado_experiencia.id_empleado = empleado.id_empleado;
                            string empresa = Request["el_empresa_" + identifier];
                            string direccion = Request["el_direccion_" + identifier];
                            string puesto = Request["el_puesto_" + identifier];
                            string comentario = Request["el_comentario_" + identifier];
                            string telefono = Request["el_telefono_" + identifier];
                            string jefe = Request["el_jefe_" + identifier];
                            int? tel = null;
                            empleado_experiencia.empresa = String.IsNullOrEmpty(empresa) ? "" : empresa.ToUpper();
                            empleado_experiencia.direccion = String.IsNullOrEmpty(direccion) ? "" : direccion.ToUpper();
                            empleado_experiencia.puesto = String.IsNullOrEmpty(puesto) ? "" : puesto.ToUpper();
                            empleado_experiencia.telefono = !String.IsNullOrEmpty(telefono) ? Convert.ToInt32(telefono) : tel;
                            empleado_experiencia.jefe_inmediato = String.IsNullOrEmpty(jefe) ? "" : jefe.ToUpper();
                            empleado_experiencia.fecha_inicio = Convert.ToDateTime(Request["el_fecha_inicio_" + identifier]);
                            empleado_experiencia.fecha_fin = Convert.ToDateTime(Request["el_fecha_fin_" + identifier]);
                            empleado_experiencia.comentario = String.IsNullOrEmpty(comentario) ? "" : comentario.ToUpper();
                            empleado_experiencia.id_empleado = empleado.id_empleado;
                            db.Empleado_Experiencia_Laboral.Add(empleado_experiencia);
                        }
                        //Subir la imagen de perfil desde el input file
                        if (imagen_cedula != null && tipo_foto == 1)
                        {
                            Archivo imagenPerfil = new Archivo();
                            string name = imagen_cedula.FileName;
                            string extension = name.Substring(name.LastIndexOf('.'));
                            name = "foto_" + empleado.id_empleado;
                            imagenPerfil.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                            imagenPerfil.fecha_creacion = DateTime.Now;
                            imagenPerfil.activo = true;
                            imagenPerfil.eliminado = false;
                            imagenPerfil.nombre = name + extension;
                            imagenPerfil.id_tipo_archivo = (int)Catalogos.Tipo_Archivo.Foto_Perfil;
                            imagenPerfil.ubicacion = "~/Archivos/Foto Perfil/" + name + extension;

                            db.Archivo.Add(imagenPerfil);
                            db.SaveChanges();

                            imagen_cedula.SaveAs(Server.MapPath("~/Archivos/Foto Perfil/" + name + extension));

                            //Relacionar la imagen con el usuario
                            Archivo_Empleado nuevo_archivo_empleado = new Archivo_Empleado();
                            nuevo_archivo_empleado.id_archivo = imagenPerfil.id_archivo;
                            nuevo_archivo_empleado.id_empleado = empleado.id_empleado;
                            nuevo_archivo_empleado.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                            nuevo_archivo_empleado.fecha_creacion = DateTime.Now;
                            nuevo_archivo_empleado.activo = true;
                            nuevo_archivo_empleado.eliminado = false;
                            db.Archivo_Empleado.Add(nuevo_archivo_empleado);
                            db.SaveChanges();

                        }
                        //Subir la imagen de perfil desde la camara web
                        if (tipo_foto == 2)
                        {
                            Archivo imagenPerfil = new Archivo();
                            string name = "foto_" + empleado.id_empleado;
                            string extension = ".jpg";
                            imagenPerfil.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                            imagenPerfil.fecha_creacion = DateTime.Now;
                            imagenPerfil.activo = true;
                            imagenPerfil.eliminado = false;
                            imagenPerfil.nombre = name + extension;
                            imagenPerfil.id_tipo_archivo = (int)Catalogos.Tipo_Archivo.Foto_Perfil;
                            imagenPerfil.ubicacion = "~/Archivos/Foto Perfil/" + name + extension;

                            db.Archivo.Add(imagenPerfil);
                            db.SaveChanges();

                            string ruta_imagen_webcam = "~/Archivos/Temp/webcam_picture_" + Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario + ".jpg";

                            System.IO.File.Copy(Server.MapPath(ruta_imagen_webcam), Server.MapPath(imagenPerfil.ubicacion), true);

                            //Relacionar la imagen con el usuario
                            Archivo_Empleado nuevo_archivo_empleado = new Archivo_Empleado();
                            nuevo_archivo_empleado.id_archivo = imagenPerfil.id_archivo;
                            nuevo_archivo_empleado.id_empleado = empleado.id_empleado;
                            nuevo_archivo_empleado.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                            nuevo_archivo_empleado.fecha_creacion = DateTime.Now;
                            nuevo_archivo_empleado.activo = true;
                            nuevo_archivo_empleado.eliminado = false;
                            db.Archivo_Empleado.Add(nuevo_archivo_empleado);
                            db.SaveChanges();
                        }
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Details", new { id = empleado.id_empleado });
                    }
                    catch
                    {
                        tran.Rollback();
                        ModelState.AddModelError("", "Ocurrio un error inesperado. Datos no guardados.");
                    }
                }
            }
            ViewBag.banco_cuenta = new SelectList(db.Banco, "id_banco", "nombre");
            ViewBag.estado_civil = new SelectList(db.Estado_Civil, "id_estado_civil", "nombre");
            ViewBag.municipio = new SelectList(db.Municipios.Where(d => !d.eliminado).OrderBy(m => m.nombre).Select(e => new { e.id_municipio, nombre = e.nombre + " - " + e.Departamentos.nombre }), "id_municipio", "nombre");
            ViewBag.departamento = new SelectList(db.Departamentos.Where(d => !d.eliminado), "id_departamento", "nombre");
            ViewBag.pais = new SelectList(db.Paises.Where(e => !e.eliminado), "id_pais", "nombre");
            ViewBag.profesion = new SelectList(db.Profesion, "id_profesion", "nombre");
            ViewBag.tipo_cuenta = new SelectList(db.Tipo_Cuenta, "id_tipo_cuenta", "nombre");
            ViewBag.tipo_licencia = new SelectList(db.Tipo_Licencia, "id_tipo_licencia", "nombre");
            ViewBag.grado_estudio = new SelectList(db.Grado_Estudio, "id_grado_estudio", "nombre");
            ViewBag.estado_empleado = new SelectList(db.Estado_Empleado, "id_estado_empleado", "nombre");
            ViewBag.fecha_nacimiento = fecha_nacimiento.ToString("dd/MM/yyyy");
            ViewBag.tipo_direccion = new SelectList(db.Tipo_Direccion.Where(t => t.activo).OrderBy(t => t.nombre), "id_tipo_direccion", "nombre");
            ViewBag.tipo_telefono = new SelectList(db.Tipo_Telefono.Where(t => t.activo).OrderBy(t => t.nombre), "id_tipo_telefono", "nombre");
            ViewBag.municipios = new SelectList(db.Municipios.Where(m => m.activo).OrderBy(m => m.nombre).Select(e => new { e.id_municipio, nombre = e.nombre + " - " + e.Departamentos.nombre }), "id_municipio", "nombre");
            ViewBag.tipo_pago = new SelectList(db.Tipo_Pago.Where(e => !e.eliminado), "id_tipo_pago", "nombre");
            ViewBag.gyt = (int)Catalogos.Banco.GyT;
            ViewBag.genero = new SelectList(db.Genero.Where(e => !e.eliminado), "id_genero", "nombre");
            if (fecha_vencimiento_licencia != null)
            {
                ViewBag.fecha_vencimiento_licencia = fecha_vencimiento_licencia.Value.ToString("dd/MM/yyyy");
            }
            return View(empleado);
        }

        [HttpPost]
        public ActionResult NuevoCodigoEmpleado(int id)
        {
            Empleado empleado = db.Empleado.SingleOrDefault(e => e.id_empleado == id && !e.eliminado);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            if (empleado.Contratacion.Where(c => c.activo).Count() > 0)
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Info, "No se pudo crear el empleado a partir de la información existente. El empleado seleccionado todavía esta de Alta en la empresa.");
                msg.ReturnUrl = null;
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
            int id_usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Empleado nuevoEmpleado = CopiarEmpleado(empleado);
                    db.Empleado.Add(nuevoEmpleado);
                    db.SaveChanges();
                    foreach (var telefono in empleado.Empleado_Telefono.Where(e => e.activo && !e.eliminado))
                    {
                        Empleado_Telefono nuevoTelefono = new Empleado_Telefono();
                        nuevoTelefono.telefono = telefono.telefono;
                        nuevoTelefono.extension = telefono.extension;
                        nuevoTelefono.id_tipo_telefono = telefono.id_tipo_telefono;
                        nuevoTelefono.comentario = telefono.comentario;
                        nuevoTelefono.activo = true;
                        nuevoTelefono.eliminado = false;
                        nuevoTelefono.fecha_creacion = DateTime.Now;
                        nuevoTelefono.id_usuario_creacion = id_usuario;
                        nuevoTelefono.id_empleado = nuevoEmpleado.id_empleado;
                        db.Empleado_Telefono.Add(nuevoTelefono);
                    }
                    foreach (var direccion in empleado.Empleado_Direcciones.Where(d => d.activo && !d.eliminado))
                    {
                        Empleado_Direcciones nuevaDireccion = CopiarDireccion(direccion);
                        nuevaDireccion.id_empleado = nuevoEmpleado.id_empleado;
                        db.Empleado_Direcciones.Add(nuevaDireccion);
                    }
                    //Copiar experiencia laboral
                    foreach (var experiencia in empleado.Empleado_Experiencia_Laboral.Where(e => e.activo && !e.eliminado))
                    {
                        Empleado_Experiencia_Laboral experiencia_laboral = new Empleado_Experiencia_Laboral();
                        experiencia_laboral.id_empleado = experiencia.id_empleado;
                        experiencia_laboral.empresa = experiencia.empresa;
                        experiencia_laboral.direccion = experiencia.direccion;
                        experiencia_laboral.comentario = experiencia.comentario;
                        experiencia_laboral.puesto = experiencia.puesto;
                        experiencia_laboral.fecha_inicio = experiencia.fecha_inicio;
                        experiencia_laboral.fecha_fin = experiencia.fecha_fin;
                        experiencia_laboral.telefono = experiencia.telefono;
                        experiencia_laboral.jefe_inmediato = experiencia.jefe_inmediato;
                        experiencia_laboral.activo = true;
                        experiencia_laboral.eliminado = false;
                        experiencia_laboral.fecha_creacion = DateTime.Now;
                        experiencia_laboral.id_usuario_creacion = id_usuario;
                        db.Empleado_Experiencia_Laboral.Add(experiencia_laboral);
                    }
                    foreach (var archivo in empleado.Archivo_Empleado.Where(e => e.activo && !e.eliminado && e.Archivo.id_tipo_archivo == (int)Catalogos.Tipo_Archivo.Foto_Perfil))
                    {

                        Archivo imagen = new Archivo();
                        string name = "foto_" + nuevoEmpleado.id_empleado;
                        string extension = ".jpg";
                        string ruta_vieja = archivo.Archivo.ubicacion;
                        imagen.id_tipo_archivo = (int)Catalogos.Tipo_Archivo.Foto_Perfil;
                        imagen.id_usuario_creacion = id_usuario;
                        imagen.fecha_creacion = DateTime.Now;
                        imagen.activo = true; imagen.eliminado = false;
                        imagen.nombre = name + extension;
                        imagen.ubicacion = "~/Archivos/Foto Perfil/" + name + extension;

                        db.Archivo.Add(imagen);
                        db.SaveChanges();

                        System.IO.File.Copy(Server.MapPath(ruta_vieja), Server.MapPath(imagen.ubicacion), true);

                        //Relacionar la imagen con el usuario
                        Archivo_Empleado nuevo_archivo_empleado = new Archivo_Empleado();
                        nuevo_archivo_empleado.id_archivo = imagen.id_archivo;
                        nuevo_archivo_empleado.id_empleado = nuevoEmpleado.id_empleado;
                        nuevo_archivo_empleado.id_usuario_creacion = id_usuario;
                        nuevo_archivo_empleado.fecha_creacion = DateTime.Now;
                        nuevo_archivo_empleado.activo = true;
                        nuevo_archivo_empleado.eliminado = false;
                        db.Archivo_Empleado.Add(nuevo_archivo_empleado);
                    }
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Details", new { id = nuevoEmpleado.id_empleado });
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "Ocurrio un error innesperado. " + ex.Message);
                    msg.ReturnUrl = null;
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }
        }

        public Empleado CopiarEmpleado(Empleado empleado)
        {
            Empleado nuevoEmpleado = new Empleado();
            nuevoEmpleado.id_tipo_pago = empleado.id_tipo_pago;
            nuevoEmpleado.acreditamiento = empleado.acreditamiento;
            nuevoEmpleado.afiliacion_igss = empleado.afiliacion_igss;
            nuevoEmpleado.afiliacion_irtra = empleado.afiliacion_irtra;
            nuevoEmpleado.apellido_casada = empleado.apellido_casada;
            nuevoEmpleado.cedula = empleado.cedula;
            nuevoEmpleado.comentario = empleado.comentario;
            nuevoEmpleado.correo = empleado.correo;
            nuevoEmpleado.dpi = empleado.dpi;
            nuevoEmpleado.digeesp = empleado.digeesp;
            nuevoEmpleado.estatura = empleado.estatura;
            nuevoEmpleado.fecha_nacimiento = empleado.fecha_nacimiento;
            nuevoEmpleado.fecha_vencimiento_licencia = empleado.fecha_vencimiento_licencia;
            nuevoEmpleado.id_banco_cuenta = empleado.id_banco_cuenta;
            nuevoEmpleado.id_estado_civil = empleado.id_estado_civil;
            nuevoEmpleado.id_genero = empleado.id_genero;
            nuevoEmpleado.id_grado_estudio = empleado.id_grado_estudio;
            nuevoEmpleado.id_municipio_cedula = empleado.id_municipio_cedula;
            nuevoEmpleado.id_municipio_dpi = empleado.id_municipio_dpi;
            nuevoEmpleado.id_pais_nacionalidad = empleado.id_pais_nacionalidad;
            nuevoEmpleado.id_profesion = empleado.id_profesion;
            nuevoEmpleado.id_tipo_cuenta = empleado.id_tipo_cuenta;
            nuevoEmpleado.id_tipo_licencia = empleado.id_tipo_licencia;
            nuevoEmpleado.nit = empleado.nit;
            nuevoEmpleado.numero_cuenta = empleado.numero_cuenta;
            nuevoEmpleado.numero_licencia = empleado.numero_licencia;
            nuevoEmpleado.pasaporte = empleado.pasaporte;
            nuevoEmpleado.peso = empleado.peso;
            nuevoEmpleado.primer_apellido = empleado.primer_apellido;
            nuevoEmpleado.primer_nombre = empleado.primer_nombre;
            nuevoEmpleado.sabe_escribir = empleado.sabe_escribir;
            nuevoEmpleado.sabe_leer = empleado.sabe_leer;
            nuevoEmpleado.segundo_apellido = empleado.segundo_apellido;
            nuevoEmpleado.segundo_nombre = empleado.segundo_nombre;
            nuevoEmpleado.servicio_militar = empleado.servicio_militar;
            nuevoEmpleado.fecha_creacion = DateTime.Now;
            nuevoEmpleado.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            nuevoEmpleado.activo = true;
            nuevoEmpleado.eliminado = false;
            nuevoEmpleado.id_estado_empleado = (int)Catalogos.Estado_Empleado.Alta;
            return nuevoEmpleado;
        }

        public Empleado_Direcciones CopiarDireccion(Empleado_Direcciones direccion)
        {
            Empleado_Direcciones nuevaDireccion = new Empleado_Direcciones();
            nuevaDireccion.activo = true;
            nuevaDireccion.eliminado = false;
            nuevaDireccion.avenida = direccion.avenida;
            nuevaDireccion.calle = direccion.calle;
            nuevaDireccion.colonia_cc_edificio = direccion.colonia_cc_edificio;
            nuevaDireccion.comentario = direccion.comentario;
            nuevaDireccion.id_municipio = direccion.id_municipio;
            nuevaDireccion.id_tipo_direccion = direccion.id_tipo_direccion;
            nuevaDireccion.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            nuevaDireccion.fecha_creacion = DateTime.Now;
            nuevaDireccion.kilometro = direccion.kilometro;
            nuevaDireccion.local = direccion.local;
            nuevaDireccion.no_casa = direccion.no_casa;
            nuevaDireccion.zona = direccion.zona;
            nuevaDireccion.direccion = direccion.direccion;
            return nuevaDireccion;
        }


        public JsonResult GetRangoSalario(int id_tipo_puesto)
        {
            Tipo_Puesto tipo_puesto = db.Tipo_Puesto.SingleOrDefault(p => p.id_tipo_puesto == id_tipo_puesto);
            if (tipo_puesto == null)
            {
                return Json("Salario no determinado.", JsonRequestBehavior.AllowGet);
            }
            return Json(new { rango = tipo_puesto.salario_minimo.ToString("C", CultureInfo.GetCultureInfo("es-GT")) + " - " + tipo_puesto.salario_maximo.ToString("C", CultureInfo.GetCultureInfo("es-GT")) },
                JsonRequestBehavior.AllowGet);
        }
       
        
        #endregion
       
        #region Archivos de Empleado
        [HttpPost]
        public ActionResult UploadFiles(List<int> id_tipo_archivo, List<HttpPostedFileBase> lista_archivos, int id_empleado)
        {
            int i = 0;
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var nuevo_archivo in lista_archivos)
                    {
                        if (nuevo_archivo != null)
                        {
                            //Subio un archivo pero no selecciono una categoria
                            if (id_tipo_archivo[i] == 0)
                            {
                                ModelState.AddModelError("id_tipo_archivo", "No indicó el tipo de archivo.");
                            }
                            else
                            {
                                //Subir la imagen de perfil
                                Archivo archivo = new Archivo();
                                archivo.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                archivo.fecha_creacion = DateTime.Now;
                                archivo.activo = true;
                                archivo.eliminado = false;
                                archivo.Tipo_Archivo = db.Tipo_Archivo.Find(id_tipo_archivo[i]);
                                archivo.nombre = archivo.fecha_creacion.ToLongDateString() + "_" + nuevo_archivo.FileName;
                                archivo.ubicacion = "~/Archivos/" + archivo.Tipo_Archivo.nombre + "/" + archivo.fecha_creacion.ToLongDateString() + "_" + nuevo_archivo.FileName;
                                db.Archivo.Add(archivo);
                                db.SaveChanges();
                                nuevo_archivo.SaveAs(Server.MapPath(archivo.ubicacion));
                                //Relacionar la imagen con el usuario
                                Archivo_Empleado nuevo_archivo_empleado = new Archivo_Empleado();
                                nuevo_archivo_empleado.id_archivo = archivo.id_archivo;
                                nuevo_archivo_empleado.id_empleado = id_empleado;
                                nuevo_archivo_empleado.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                                nuevo_archivo_empleado.fecha_creacion = DateTime.Now;
                                nuevo_archivo_empleado.activo = true;
                                nuevo_archivo_empleado.eliminado = false;
                                db.Archivo_Empleado.Add(nuevo_archivo_empleado);
                                db.SaveChanges();
                                
                            }
                        }
                        i++;
                    }
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    ModelState.AddModelError("", "Error subiendo los archivos, proceso cancelado.");
                    //Error inesperado
                }
            }
            return RedirectToAction("Details", new { @id=id_empleado });
        }

        [ActionName("EliminarArchivo")]
        public ActionResult EliminarArchivo(int id_archivo, int id_archivo_empleado, int id_empleado)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                    Archivo archivo = db.Archivo.Find(id_archivo);
                    archivo.eliminado = true;
                    archivo.activo = false;
                    archivo.fecha_eliminacion = DateTime.Now;
                    archivo.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                    db.Entry(archivo).State = EntityState.Modified;

                    Archivo_Empleado archivo_empleado = db.Archivo_Empleado.Find(id_archivo_empleado);
                    archivo_empleado.activo = false;
                    archivo_empleado.eliminado = true;
                    archivo_empleado.fecha_eliminacion = DateTime.Now;
                    archivo_empleado.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                    db.Entry(archivo_empleado).State = EntityState.Modified;

                    db.SaveChanges();
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
            return RedirectToAction("Details", new { @id = id_empleado });
        }

        public ActionResult GetImage(int id)
        {
            var archivo_empleado = db.Archivo_Empleado.Where(e => !e.eliminado && e.id_empleado == id && e.Archivo.id_tipo_archivo == (int)Catalogos.Tipo_Archivo.Foto_Perfil);
            foreach (var file in archivo_empleado)
            {
                string nombre = file.Archivo.nombre;
                nombre = nombre.Substring(0, nombre.LastIndexOf('.'));
                string[] imagen = nombre.Split('_');
                if (imagen.Length == 2)
                {
                    int id_imagen;
                    bool conversion = int.TryParse(imagen[1], out id_imagen);
                    if (conversion)
                    {
                        if (id_imagen == id)
                        {
                            var image_byte = System.IO.File.ReadAllBytes(Server.MapPath(file.Archivo.ubicacion));
                            string extension = file.Archivo.ubicacion.Substring(file.Archivo.ubicacion.LastIndexOf('.') + 1);
                            
                            return File(image_byte, "image/" + extension);
                        }
                    }
                }
            }
            //Retornar imagen default si el empleado no tiene ninguna en el sistema
            var image_default = System.IO.File.ReadAllBytes(Server.MapPath(ruta_imagen_default));
            string ext = ruta_imagen_default.Substring(ruta_imagen_default.LastIndexOf('.') + 1);
            return File(image_default, "image/" + ext);
        }
        #endregion

        #region Telefonos

        [HttpPost]
        public ActionResult NuevoTelefono(int id_empleado, int numero, int? extension, int id_tipo_telefono, string comentario)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Empleado_Telefono telefono = new Empleado_Telefono();
                        telefono.telefono = numero;
                        telefono.comentario = ReplaceEmpty(comentario);
                        telefono.extension = extension;
                        telefono.id_tipo_telefono = id_tipo_telefono;
                        telefono.id_empleado = id_empleado;
                        telefono.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        telefono.fecha_creacion = DateTime.Now;
                        telefono.activo = true;
                        telefono.eliminado = false;
                        db.Empleado_Telefono.Add(telefono);
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        ContextMessage msg = new ContextMessage(ContextMessage.Error, "El nuevo teléfono no pudo se añadido.");
                        msg.ReturnUrl = Url.Action("Details", new { id = id_empleado });
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                }
            }
            return RedirectToAction("Details", new { @id = id_empleado });
        }

        [HttpPost]
        public ActionResult EditarTelefono(int id_empleado_telefono, int id_tipo_telefono, int ed_numero, int? ed_extension, string ed_comentario)
        {
            int id_empleado = 0;
            if (ModelState.IsValid)
            {
                using(DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        Empleado_Telefono telefono = db.Empleado_Telefono.SingleOrDefault(e => e.id_empleado_telefono == id_empleado_telefono && e.activo);
                        if (telefono == null)
                        {
                            return HttpNotFound();
                        }
                        id_empleado = telefono.id_empleado;
                        telefono.id_tipo_telefono = id_tipo_telefono;
                        telefono.telefono = ed_numero;
                        telefono.extension = ed_extension;
                        telefono.comentario = ReplaceEmpty(ed_comentario);
                        telefono.fecha_modificacion = DateTime.Now;
                        telefono.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                        db.Entry(telefono).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        ContextMessage msg = new ContextMessage(ContextMessage.Error, "El teléfono no pudo ser modificado.");
                        msg.ReturnUrl = Url.Action("Details", new { id = id_empleado });
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                }
            }
            return RedirectToAction("Details", new { @id = id_empleado });
        }

        [ActionName("BorrarTelefono")]
        public ActionResult BorrarTelefono(int id_empleado_telefono)
        {
            int id_empleado = 0;
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Empleado_Telefono telefono = db.Empleado_Telefono.SingleOrDefault(e => e.id_empleado_telefono == id_empleado_telefono && e.activo);
                    if (telefono == null)
                    {
                        return HttpNotFound();
                    }
                    UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                    telefono.fecha_eliminacion = DateTime.Now;
                    telefono.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                    telefono.eliminado = true;
                    telefono.activo = false;
                    id_empleado = telefono.id_empleado;
                    db.Entry(telefono).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "El teléfono no pudo ser eliminado");
                    msg.ReturnUrl = Url.Action("Details", new { id = id_empleado });
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
            }
            return RedirectToAction("Details", new { @id = id_empleado });
        }

        #endregion

        #region Direccion

        [HttpPost]
        public ActionResult NuevaDireccion(int id_empleado, string direccion_input, int id_tipo_direccion, int id_municipio)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Empleado_Direcciones direccion = new Empleado_Direcciones();
                        //Direcciones
                        direccion.direccion = String.IsNullOrEmpty(direccion_input) ? "" : direccion_input.ToUpper();
                        direccion.id_empleado = id_empleado;
                        direccion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                        direccion.fecha_creacion = DateTime.Now;
                        direccion.activo = true;
                        direccion.eliminado = false;
                        direccion.id_municipio = id_municipio;
                        direccion.id_tipo_direccion = id_tipo_direccion;
                        db.Empleado_Direcciones.Add(direccion);
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        ContextMessage msg = new ContextMessage(ContextMessage.Error, "La nueva dirección no pudo ser creada.");
                        msg.ReturnUrl = Url.Action("Details", new { id = id_empleado });
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                }
            }
            return RedirectToAction("Details", new { @id = id_empleado });
        }

        [HttpPost]
        public ActionResult EditarDireccion(int id_empleado_direcciones, string direccion_input, int id_tipo_direccion, int id_municipio)
        {
            int id_empleado = 0;
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Empleado_Direcciones direccion = db.Empleado_Direcciones.SingleOrDefault(e => e.activo && e.id_empleado_direcciones == id_empleado_direcciones);
                        if (direccion == null)
                        {
                            return HttpNotFound();
                        }
                        id_empleado = direccion.id_empleado;
                        direccion.fecha_modificacion = DateTime.Now;
                        direccion.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        direccion.direccion = String.IsNullOrEmpty(direccion_input) ? "" : direccion_input.ToUpper();
                        direccion.id_tipo_direccion = id_tipo_direccion;
                        direccion.id_municipio = id_municipio;
                        db.Entry(direccion).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        ContextMessage msg = new ContextMessage(ContextMessage.Error, "La dirección no pudo ser modificada.");
                        msg.ReturnUrl = Url.Action("Details", new { id = id_empleado });
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                }
            }
            return RedirectToAction("Details", new { id = id_empleado });
        }

        [ActionName("EliminarDireccion")]
        public ActionResult EliminarDireccion(int id_empleado_direcciones)
        {
            int id_empleado = 0;
            if(ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Empleado_Direcciones direccion = db.Empleado_Direcciones.SingleOrDefault(e => e.activo && e.id_empleado_direcciones == id_empleado_direcciones);
                        if (direccion == null)
                        {
                            return HttpNotFound();
                        }
                        UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        id_empleado = direccion.id_empleado;
                        direccion.fecha_eliminacion = DateTime.Now;
                        direccion.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                        direccion.eliminado = true;
                        direccion.activo = false;
                        db.Entry(direccion).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        ContextMessage msg = new ContextMessage(ContextMessage.Error, "La dirección no pudo ser eliminada.");
                        msg.ReturnUrl = Url.Action("Details", new { id = id_empleado });
                        TempData[User.Identity.Name] = msg;
                        return RedirectToAction("Mensaje");
                    }
                }
            }
            return RedirectToAction("Details", new { @id = id_empleado });
        }

        #endregion

        #region Experiencia Laboral
        [HttpPost]
        public ActionResult NuevaExperienciaLaboral(int id_empleado, string jefe, string empresa, int? telefono, string direccion, DateTime fecha_inicio, DateTime fecha_fin, string comentario, string puesto)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            try
            {
                Empleado_Experiencia_Laboral experiencia_laboral = new Empleado_Experiencia_Laboral();
                experiencia_laboral.empresa = String.IsNullOrEmpty(empresa) ? "" : empresa.ToUpper();
                experiencia_laboral.puesto = String.IsNullOrEmpty(puesto) ? "" : puesto.ToUpper();
                experiencia_laboral.direccion = String.IsNullOrEmpty(direccion) ? "" : direccion.ToUpper();
                experiencia_laboral.comentario = String.IsNullOrEmpty(comentario) ? "" : comentario.ToUpper();
                experiencia_laboral.id_empleado = id_empleado;
                experiencia_laboral.fecha_inicio = fecha_inicio;
                experiencia_laboral.fecha_fin = fecha_fin;
                experiencia_laboral.telefono = telefono;
                experiencia_laboral.jefe_inmediato = String.IsNullOrEmpty(jefe) ? "" : jefe.ToUpper();
                experiencia_laboral.activo = true;
                experiencia_laboral.eliminado = false;
                experiencia_laboral.fecha_creacion = DateTime.Now;
                experiencia_laboral.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                db.Empleado_Experiencia_Laboral.Add(experiencia_laboral);
                db.SaveChanges();
                return RedirectToAction("Details", new { @id = id_empleado });
            }
            catch
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Error, "La experiencia laboral no pudo se añadida.");
                msg.ReturnUrl = Url.Action("Details", new { id = id_empleado });
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
        }

        [HttpPost]
        public ActionResult EditarExperienciaLaboral(int id, string empresa, string jefe, int? telefono, string direccion, DateTime fecha_inicio, DateTime fecha_fin, string comentario, string puesto)
        {
            int id_empleado = 0;
            try
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                Empleado_Experiencia_Laboral experiencia_laboral = db.Empleado_Experiencia_Laboral.SingleOrDefault(e => e.id_empleado_experiencia_laboral == id && e.activo);
                if (experiencia_laboral == null)
                {
                    return HttpNotFound();
                }
                id_empleado = experiencia_laboral.id_empleado;
                experiencia_laboral.empresa = String.IsNullOrEmpty(empresa) ? "" : empresa.ToUpper();
                experiencia_laboral.puesto = String.IsNullOrEmpty(puesto) ? "" : puesto.ToUpper();
                experiencia_laboral.direccion = String.IsNullOrEmpty(direccion) ? "" : direccion.ToUpper();
                experiencia_laboral.comentario = String.IsNullOrEmpty(comentario) ? "" : comentario.ToUpper();
                experiencia_laboral.jefe_inmediato = String.IsNullOrEmpty(jefe) ? "" : jefe.ToUpper();
                experiencia_laboral.fecha_inicio = fecha_inicio;
                experiencia_laboral.fecha_fin = fecha_fin;
                experiencia_laboral.telefono = telefono;
                experiencia_laboral.fecha_modificacion = DateTime.Now;
                experiencia_laboral.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                db.Entry(experiencia_laboral).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { @id = id_empleado });

            }
            catch
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Error, "La experiencia laboral no pudo ser modificada.");
                msg.ReturnUrl = Url.Action("Details", new { id = id_empleado });
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
        }

        [HttpPost]
        public ActionResult BorrarExperienciaLaboral(int id)
        {
            int id_empleado = 0;
            try
            {
                Empleado_Experiencia_Laboral experiencia_laboral = db.Empleado_Experiencia_Laboral.SingleOrDefault(e => e.id_empleado_experiencia_laboral == id && e.activo);
                if (experiencia_laboral == null)
                {
                    return HttpNotFound();
                }
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                experiencia_laboral.fecha_eliminacion = DateTime.Now;
                experiencia_laboral.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
                experiencia_laboral.eliminado = true;
                experiencia_laboral.activo = false;
                id_empleado = experiencia_laboral.id_empleado;
                db.Entry(experiencia_laboral).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = id_empleado });
            }
            catch
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Error, "La experiencia laboral no pudo ser eliminada.");
                msg.ReturnUrl = Url.Action("Details", new { id = id_empleado });
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
        }

        #endregion

        #region Contrato

        public ActionResult Contrato(int id)
        {
            var Contrato = db.Contratacion.Where(c => c.id_empleado == id && c.activo && !c.eliminado).FirstOrDefault();
            if (Contrato == null)
            {
                return RedirectToAction("Create", "Contratacion", new { id_empleado = id });
            }
            else
            {
                return RedirectToAction("Details", "Contratacion", new { id = Contrato.id_contratacion });
            }
        }

        #endregion

        #region Reportes de Contrato

        public FileStreamResult Get_Hoja_Informacion(int id)
        {
            Empleado empleado = db.Empleado.SingleOrDefault(c => c.id_empleado == id);
            if (empleado == null)
            {
                return null;
            }
            string parametros = "&id_empleado=" + empleado.id_empleado;
            string reporte = "rpt_Hoja_Informacion_Empleado";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Hoja de Información - Empleado - " + empleado.id_empleado +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult Get_Hoja_Vida(int id)
        {
            Empleado empleado = db.Empleado.SingleOrDefault(c => c.id_empleado == id);
            if (empleado == null)
            {
                return null;
            }
            string parametros = "&id_empleado=" + empleado.id_empleado;
            string reporte = "rpt_Hoja_Vida";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Hoja de Vida - Empleado - " + empleado.id_empleado +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult Get_Carta_Renuncia(int id)
        {
            int idValido = 0;
            Contratacion contrato = db.Contratacion.SingleOrDefault(c => c.id_contratacion == id && c.activo && !c.eliminado);
            if (contrato != null)
            {
                idValido = contrato.id_contratacion;
            }
            string parametros = "&id_contratacion=" + idValido.ToString();
            string reporte = "rpt_Carta_Renuncia";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Carta de Renuncia - Empleado - " + contrato.id_empleado +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult Get_Declaracion_Jurada(int id)
        {
            int idValido = 0;
            Contratacion contrato = db.Contratacion.SingleOrDefault(c => c.id_contratacion == id && c.activo && !c.eliminado);
            if (contrato != null)
            {
                idValido = contrato.id_contratacion;
            }
            string parametros = "&id_contratacion=" + idValido.ToString();
            string reporte = "rpt_Declaracion_Jurada";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Declaracion Jurada - Empleado - " + contrato.id_empleado +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult Get_Pagare(int id)
        {
            int idValido = 0;
            Contratacion contrato = db.Contratacion.SingleOrDefault(c => c.id_contratacion == id && c.activo && !c.eliminado);
            if (contrato != null)
            {
                idValido = contrato.id_contratacion;
            }
            string parametros = "&id_contratacion=" + idValido.ToString();
            string reporte = "rpt_Pagare";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Pagare - Empleado - " + contrato.id_empleado +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult Get_Registro_Firmas(int id)
        {
            int idValido = 0;
            Contratacion contrato = db.Contratacion.SingleOrDefault(c => c.id_contratacion == id && c.activo && !c.eliminado);
            if (contrato != null)
            {
                idValido = contrato.id_contratacion;
            }
            string parametros = "&id_contratacion=" + idValido.ToString();
            string reporte = "rpt_Registro_Firmas";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Registro de Firmas - Empleado - " + contrato.id_empleado +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult Get_Registro_Huellas(int id)
        {
            int idValido = 0;
            Contratacion contrato = db.Contratacion.SingleOrDefault(c => c.id_contratacion == id && c.activo && !c.eliminado);
            if (contrato != null)
            {
                idValido = contrato.id_contratacion;
            }
            string parametros = "&id_contratacion=" + idValido.ToString();
            string reporte = "rpt_Registro_Huellas_Digitales";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Registro de Huellas Digitales - Empleado - " + contrato.id_empleado +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult Get_Contrato_Prestaciones(int id)
        {
            int idValido = 0;
            Contratacion contrato = db.Contratacion.SingleOrDefault(c => c.id_contratacion == id && c.activo && !c.eliminado);
            if (contrato != null)
            {
                idValido = contrato.id_contratacion;
            }
            string parametros = "&id_contratacion=" + idValido.ToString();
            string reporte = "rpt_Contrato_Prestaciones";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Contrato Prestaciones - Empleado - " + contrato.id_empleado +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult Get_Contrato_SSV(int id)
        {
            int idValido = 0;
            Contratacion contrato = db.Contratacion.SingleOrDefault(c => c.id_contratacion == id && c.activo && !c.eliminado);
            if (contrato != null)
            {
                idValido = contrato.id_contratacion;
            }
            string parametros = "&id_contratacion=" + idValido.ToString();
            string reporte = "rpt_Contrato_Servicio_Seguridad_Vigilancia";
            PDF_Protal archivo_reporte = new PDF_Protal(reporte, parametros);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"Contrato por Servicios de Seguridad y Vigilancia - Empleado - " + contrato.id_empleado +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        public FileStreamResult Get_Reporte(int id_contrato, int id_reporte)
        {
            int idValido = 0;
            Contratacion contrato = db.Contratacion.SingleOrDefault(c => c.id_contratacion == id_contrato && c.activo && !c.eliminado);
            Documentos documento = db.Documentos.SingleOrDefault(d => d.id_documento == id_reporte);
            if (contrato == null || documento == null)
            {
                return null;
            }
            idValido = contrato.id_contratacion;
            string parametro = "&id_contratacion=" + idValido.ToString();
            PDF_Protal archivo_reporte = new PDF_Protal(documento.reporte, parametro);
            byte[] fileBytes = archivo_reporte.obtener_reporte();
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader(
                "content-disposition",
                "attachment; filename=\"" + documento.nombre + " - Empleado - " + contrato.id_empleado +
                      ".pdf\"");
            Response.BinaryWrite(fileBytes);
            Response.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }

        #endregion

        #region Metodos

        public ViewResult Mensaje()
        {
            ContextMessage msg = (ContextMessage)TempData[User.Identity.Name];
            return View(ContextMessage.ViewLocation(this), msg);
        }

        public string ReplaceEmpty(string cadena)
        {
            if (string.IsNullOrWhiteSpace(cadena))
            {
                return null;
            }
            return cadena.ToUpper();
        }

        public Empleado_Direcciones NuevaDireccion()
        {
            Empleado_Direcciones empleado_direccion = new Empleado_Direcciones();
            empleado_direccion.activo = true;
            empleado_direccion.eliminado = false;
            empleado_direccion.fecha_creacion = DateTime.Now;
            empleado_direccion.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return empleado_direccion;
        }

        public Empleado_Telefono NuevoTelefono()
        {
            Empleado_Telefono empleado_telefono = new Empleado_Telefono();
            empleado_telefono.activo = true;
            empleado_telefono.eliminado = false;
            empleado_telefono.fecha_creacion = DateTime.Now;
            empleado_telefono.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return empleado_telefono;
        }

        public Empleado_Experiencia_Laboral NuevaExperienciaLaboral()
        {
            Empleado_Experiencia_Laboral empleado_experiencia = new Empleado_Experiencia_Laboral();
            empleado_experiencia.activo = true;
            empleado_experiencia.eliminado = false;
            empleado_experiencia.fecha_creacion = DateTime.Now;
            empleado_experiencia.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return empleado_experiencia;
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

        public static string RenderViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            var context = controllerContext;
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            var viewData = new ViewDataDictionary(model);
            
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, viewData, new TempDataDictionary(), sw);
                
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult PrintTestContrato(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Contratacion contratacion = db.Contratacion.Find(id);
                if (contratacion == null)
                {
                    return HttpNotFound();
                }

                rpt_Contrato_Result contrato = db.rpt_Contrato(id).FirstOrDefault();
                string htmlToConvert = RenderViewToString(this.ControllerContext, "TestContrato", contrato);


                // the base URL to resolve relative images and css
                //String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
                //String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "Home/ConvertAboutPageToPdf".Length);


                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                //SelectPdf.PdfDocument doc = converter.ConvertUrl("http://selectpdf.com");
                /*
                 * A1
                 * A2
                 * A3
                 * A4
                 * A5
                 * Letter
                 * HalfLetter
                 * Ledger
                 * Legal
                 */
                string pdf_page_size = "Legal";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                /*
                 * Portrait
                 * Landscape
                 */
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlToConvert);
                //doc.Save("test.pdf");
                //doc.Close();
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();

                // return resulted pdf document
                FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                fileResult.FileDownloadName = "Contrato_" + contratacion.id_empleado + ".pdf";
                return fileResult;


            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            return View();
        }
        [AllowAnonymous]
        public ActionResult TestContrato(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratacion contratacion = db.Contratacion.Find(id);
            if (contratacion == null)
            {
                return HttpNotFound();
            }
            rpt_Contrato_Result contrato = db.rpt_Contrato(id).FirstOrDefault();
            return View(contrato);
        }

        [AllowAnonymous]
        public ActionResult RptDeclaracionJurada(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratacion contratacion = db.Contratacion.Find(id);
            if (contratacion == null)
            {
                return HttpNotFound();
            }
            rpt_Contrato_Result contrato = db.rpt_Contrato(id).FirstOrDefault();
            ViewBag.empleado = contratacion.Empleado;
            return View(contrato);
        }
        public ActionResult PrintDeclaracionJurada(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Contratacion contratacion = db.Contratacion.Find(id);
                if (contratacion == null)
                {
                    return HttpNotFound();
                }

                rpt_Contrato_Result contrato = db.rpt_Contrato(id).FirstOrDefault();
                string htmlToConvert = RenderViewToString(this.ControllerContext, "RptDeclaracionJurada", contrato);


                // the base URL to resolve relative images and css
                //String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
                //String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "Home/ConvertAboutPageToPdf".Length);


                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                //SelectPdf.PdfDocument doc = converter.ConvertUrl("http://selectpdf.com");
                /*
                 * A1
                 * A2
                 * A3
                 * A4
                 * A5
                 * Letter
                 * HalfLetter
                 * Ledger
                 * Legal
                 */
                string pdf_page_size = "Legal";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                /*
                 * Portrait
                 * Landscape
                 */
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlToConvert);
                //doc.Save("test.pdf");
                //doc.Close();
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();

                // return resulted pdf document
                FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                fileResult.FileDownloadName = "DeclaracionJurada_" + contratacion.id_empleado + ".pdf";
                return fileResult;


            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            return View();
        }
        public ActionResult RptPagare(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratacion contratacion = db.Contratacion.Find(id);
            if (contratacion == null)
            {
                return HttpNotFound();
            }
            rpt_Contrato_Result contrato = db.rpt_Contrato(id).FirstOrDefault();
            ViewBag.empleado = contratacion.Empleado;
            return View(contrato);
        }
        public ActionResult PrintPagare(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Contratacion contratacion = db.Contratacion.Find(id);
                if (contratacion == null)
                {
                    return HttpNotFound();
                }

                rpt_Contrato_Result contrato = db.rpt_Contrato(id).FirstOrDefault();
                string htmlToConvert = RenderViewToString(this.ControllerContext, "RptPagare", contrato);


                // the base URL to resolve relative images and css
                //String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
                //String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "Home/ConvertAboutPageToPdf".Length);


                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                //SelectPdf.PdfDocument doc = converter.ConvertUrl("http://selectpdf.com");
                /*
                 * A1
                 * A2
                 * A3
                 * A4
                 * A5
                 * Letter
                 * HalfLetter
                 * Ledger
                 * Legal
                 */
                string pdf_page_size = "Legal";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                /*
                 * Portrait
                 * Landscape
                 */
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlToConvert);
                //doc.Save("test.pdf");
                //doc.Close();
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();

                // return resulted pdf document
                FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                fileResult.FileDownloadName = "Pagare_" + contratacion.id_empleado + ".pdf";
                return fileResult;


            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            return View();
        }
        public ActionResult RptCartaRenuncia(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratacion contratacion = db.Contratacion.Find(id);
            if (contratacion == null)
            {
                return HttpNotFound();
            }
            rpt_Contrato_Result contrato = db.rpt_Contrato(id).FirstOrDefault();
            ViewBag.empleado = contratacion.Empleado;
            return View(contrato);
        }
        public ActionResult PrintCartaRenuncia(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Contratacion contratacion = db.Contratacion.Find(id);
                if (contratacion == null)
                {
                    return HttpNotFound();
                }

                rpt_Contrato_Result contrato = db.rpt_Contrato(id).FirstOrDefault();
                string htmlToConvert = RenderViewToString(this.ControllerContext, "RptCartaRenuncia", contrato);


                // the base URL to resolve relative images and css
                //String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
                //String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "Home/ConvertAboutPageToPdf".Length);


                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                //SelectPdf.PdfDocument doc = converter.ConvertUrl("http://selectpdf.com");
                /*
                 * A1
                 * A2
                 * A3
                 * A4
                 * A5
                 * Letter
                 * HalfLetter
                 * Ledger
                 * Legal
                 */
                string pdf_page_size = "Legal";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                /*
                 * Portrait
                 * Landscape
                 */
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlToConvert);
                //doc.Save("test.pdf");
                //doc.Close();
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();

                // return resulted pdf document
                FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                fileResult.FileDownloadName = "CartaRenuncia_" + contratacion.id_empleado + ".pdf";
                return fileResult;


            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            return View();
        }
        public ActionResult RptContratoAprendiz(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratacion contratacion = db.Contratacion.Find(id);
            if (contratacion == null)
            {
                return HttpNotFound();
            }
            rpt_Contrato_Result contrato = db.rpt_Contrato(id).FirstOrDefault();
            ViewBag.empleado = contratacion.Empleado;
            return View(contrato);
        }
        public ActionResult PrintContratoAprendiz(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Contratacion contratacion = db.Contratacion.Find(id);
                if (contratacion == null)
                {
                    return HttpNotFound();
                }

                rpt_Contrato_Result contrato = db.rpt_Contrato(id).FirstOrDefault();
                string htmlToConvert = RenderViewToString(this.ControllerContext, "RptContratoAprendiz", contrato);


                // the base URL to resolve relative images and css
                //String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
                //String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "Home/ConvertAboutPageToPdf".Length);


                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                //SelectPdf.PdfDocument doc = converter.ConvertUrl("http://selectpdf.com");
                /*
                 * A1
                 * A2
                 * A3
                 * A4
                 * A5
                 * Letter
                 * HalfLetter
                 * Ledger
                 * Legal
                 */
                string pdf_page_size = "Legal";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                /*
                 * Portrait
                 * Landscape
                 */
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlToConvert);
                //doc.Save("test.pdf");
                //doc.Close();
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();

                // return resulted pdf document
                FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                fileResult.FileDownloadName = "ContratoAprendiz_" + contratacion.id_empleado + ".pdf";
                return fileResult;


            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            return View();
        }

        public ActionResult PrintRegistroFirmas(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Contratacion contratacion = db.Contratacion.Find(id);
                if (contratacion == null)
                {
                    return HttpNotFound();
                }

                rpt_Contrato_Result contrato = db.rpt_Contrato(id).FirstOrDefault();
                string htmlToConvert = RenderViewToString(this.ControllerContext, "RptRegistroFirmas", contrato);


                // the base URL to resolve relative images and css
                //String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
                //String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "Home/ConvertAboutPageToPdf".Length);


                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                //SelectPdf.PdfDocument doc = converter.ConvertUrl("http://selectpdf.com");
                /*
                 * A1
                 * A2
                 * A3
                 * A4
                 * A5
                 * Letter
                 * HalfLetter
                 * Ledger
                 * Legal
                 */
                string pdf_page_size = "Legal";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                /*
                 * Portrait
                 * Landscape
                 */
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlToConvert);
                //doc.Save("test.pdf");
                //doc.Close();
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();

                // return resulted pdf document
                FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                fileResult.FileDownloadName = "ContratoAprendiz_" + contratacion.id_empleado + ".pdf";
                return fileResult;


            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            return View();
        }

        public ActionResult PrintRegistroHuellas(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Contratacion contratacion = db.Contratacion.Find(id);
                if (contratacion == null)
                {
                    return HttpNotFound();
                }

                rpt_Contrato_Result contrato = db.rpt_Contrato(id).FirstOrDefault();
                string htmlToConvert = RenderViewToString(this.ControllerContext, "RptRegistroHuellas", contrato);


                // the base URL to resolve relative images and css
                //String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
                //String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "Home/ConvertAboutPageToPdf".Length);


                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                //SelectPdf.PdfDocument doc = converter.ConvertUrl("http://selectpdf.com");
                /*
                 * A1
                 * A2
                 * A3
                 * A4
                 * A5
                 * Letter
                 * HalfLetter
                 * Ledger
                 * Legal
                 */
                string pdf_page_size = "Legal";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                /*
                 * Portrait
                 * Landscape
                 */
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlToConvert);
                //doc.Save("test.pdf");
                //doc.Close();
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();

                // return resulted pdf document
                FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                fileResult.FileDownloadName = "ContratoAprendiz_" + contratacion.id_empleado + ".pdf";
                return fileResult;


            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            return View();
        }

        [HttpPost]
        public ActionResult PrintFiniquito(int? id_contratacion, decimal? vacaciones, decimal? bono_14, decimal? sueldos_pendientes,
            decimal? indemnizacion, decimal? aguinaldo, decimal? deducciones)
        {
            try
            {
                if (id_contratacion == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Contratacion contratacion = db.Contratacion.Find(id_contratacion);
                if (contratacion == null)
                {
                    return HttpNotFound();
                }
                Finiquito finiquito = new Finiquito();
                finiquito.finiquito = db.rpt_Finiquito_Doc(id_contratacion).FirstOrDefault();
                finiquito.aguinaldo = aguinaldo.HasValue ? aguinaldo.Value : 0;
                finiquito.bono_14 = bono_14.HasValue ? bono_14.Value : 0;
                finiquito.deducciones = deducciones.HasValue ? deducciones.Value : 0;
                finiquito.indemnizacion = indemnizacion.HasValue ? indemnizacion.Value : 0;
                finiquito.sueldos_pendientes = sueldos_pendientes.HasValue ? sueldos_pendientes.Value : 0;
                finiquito.vacaciones = vacaciones.HasValue ? vacaciones.Value : 0;
                finiquito.total = finiquito.aguinaldo + finiquito.bono_14 - finiquito.deducciones + finiquito.indemnizacion +
                    finiquito.sueldos_pendientes + finiquito.vacaciones;
                string htmlToConvert = RenderViewToString(this.ControllerContext, "Finiquito", finiquito);


                // the base URL to resolve relative images and css
                //String thisPageUrl = this.ControllerContext.HttpContext.Request.Url.AbsoluteUri;
                //String baseUrl = thisPageUrl.Substring(0, thisPageUrl.Length - "Home/ConvertAboutPageToPdf".Length);


                SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                //SelectPdf.PdfDocument doc = converter.ConvertUrl("http://selectpdf.com");
                /*
                 * A1
                 * A2
                 * A3
                 * A4
                 * A5
                 * Letter
                 * HalfLetter
                 * Ledger
                 * Legal
                 */
                string pdf_page_size = "Letter";
                SelectPdf.PdfPageSize pageSize =
                    (SelectPdf.PdfPageSize)Enum.Parse(typeof(SelectPdf.PdfPageSize), pdf_page_size, true);

                /*
                 * Portrait
                 * Landscape
                 */
                string pdf_orientation = "Portrait";
                SelectPdf.PdfPageOrientation pdfOrientation =
                    (SelectPdf.PdfPageOrientation)Enum.Parse(typeof(SelectPdf.PdfPageOrientation),
                    pdf_orientation, true);

                int webPageWidth = 1024;
                int webPageHeight = 0;

                // set converter options
                converter.Options.PdfPageSize = pageSize;
                converter.Options.PdfPageOrientation = pdfOrientation;
                converter.Options.WebPageWidth = webPageWidth;
                converter.Options.WebPageHeight = webPageHeight;

                SelectPdf.PdfDocument doc = converter.ConvertHtmlString(htmlToConvert);
                //doc.Save("test.pdf");
                //doc.Close();
                byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();

                // return resulted pdf document
                FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                fileResult.FileDownloadName = "Finiquito  - " + contratacion.id_empleado + ".pdf";
                return fileResult;


            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult Finiquito(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratacion contratacion = db.Contratacion.Find(id);
            if (contratacion == null)
            {
                return HttpNotFound();
            }
            Finiquito finiquito = new Finiquito();
            finiquito.finiquito = db.rpt_Finiquito_Doc(id).FirstOrDefault();
            finiquito.aguinaldo = 0;
            finiquito.bono_14 = 0;
            finiquito.deducciones = 0;
            finiquito.indemnizacion = 0;
            finiquito.sueldos_pendientes = 0;
            finiquito.vacaciones = 0;
            finiquito.total = finiquito.aguinaldo + finiquito.bono_14 + finiquito.deducciones + finiquito.indemnizacion +
                finiquito.sueldos_pendientes + finiquito.vacaciones;
            return View(finiquito);
        }
    }
}
