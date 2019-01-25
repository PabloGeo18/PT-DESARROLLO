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
using MVC2013.Areas.rrhh.Models;
using System.Globalization;
using MVC2013.Src.Comun.View;

namespace MVC2013.Areas.rrhh.Controllers
{
    public class ContratacionController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Contratacions
        public ActionResult Index()
        {
            var contratacion = db.Contratacion.Where(c=>c.activo && !c.eliminado);
            return View(contratacion.ToList());
        }

        // GET: rrhh/Contratacions/Details/5
        public ActionResult Details(int? id)
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
            return View(contratacion);
        }

        // GET: rrhh/Contratacions/Create
        public ActionResult Create(int id_empleado)
        {
            Empleado empleado = db.Empleado.SingleOrDefault(e => e.id_empleado == id_empleado && e.activo);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            ViewBag.empresa = new SelectList(db.Empresa.Where(e => !e.eliminado), "id_empresa", "nombre");
            ViewBag.nombre_empleado = empleado.primer_nombre + " " + (!String.IsNullOrEmpty(empleado.segundo_nombre) ? empleado.segundo_nombre + " " : "") +
                empleado.primer_apellido + " " + (!String.IsNullOrEmpty(empleado.segundo_apellido) ? empleado.segundo_apellido : "");
            Contratacion contrato = new Contratacion();
            contrato.id_empleado = empleado.id_empleado;
            return View(contrato);
        }

        [HttpGet]
        public JsonResult GetPuestos(int id_tipo_puesto)
        {
            var resultados = new List<object>();
            Tipo_Puesto tipo_puesto = db.Tipo_Puesto.SingleOrDefault(e => e.id_tipo_puesto == id_tipo_puesto && e.activo);
            if (tipo_puesto == null)
            {
                return Json(resultados, JsonRequestBehavior.AllowGet);
            }
            foreach (var puesto in tipo_puesto.Puesto.Where(t => t.activo && !t.eliminado))
            {
                resultados.Add(new { nombre = puesto.nombre, valor = puesto.id_puesto });
            }
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTiposPuestos(int id_empresa)
        {
            var resultados = new List<object>();
            Empresa empresa = db.Empresa.SingleOrDefault(e => e.id_empresa == id_empresa && e.activo);
            if (empresa == null)
            {
                return Json(resultados, JsonRequestBehavior.AllowGet);
            }
            foreach (var tipo_puesto in empresa.Tipo_Puesto.Where(t => t.activo && !t.eliminado))
            {
                resultados.Add(new { prestaciones = empresa.prestaciones, nombre = tipo_puesto.nombre, valor = tipo_puesto.id_tipo_puesto });
            }
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Modificar_Baja(int id, string razon_baja, DateTime fecha_baja, int motivo_baja)
        {
            Contratacion contrato = db.Contratacion.SingleOrDefault(c => c.id_contratacion == id);
            if (contrato == null)
            {
                return HttpNotFound();
            }
            try
            {
                contrato.fecha_fin = fecha_baja;
                contrato.id_motivo_baja = motivo_baja;
                contrato.razon_baja = razon_baja;
                contrato.fecha_modificacion = DateTime.Now;
                contrato.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Entry(contrato).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Error, "No se pudo modificar el contrato seleccionado.");
                msg.ReturnUrl = Url.Action("Historico", new { id_empleado = contrato.id_empleado });
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
            return RedirectToAction("Historico", new { id_empleado = contrato.id_empleado });
        }

        public ViewResult Mensaje()
        {
            ContextMessage msg = (ContextMessage)TempData[User.Identity.Name];
            return View(ContextMessage.ViewLocation(this), msg);
        }

        public ActionResult Historico(int id_empleado)
        {
            Empleado empleado = db.Empleado.SingleOrDefault(e => e.id_empleado == id_empleado && e.activo);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            ViewBag.motivo_baja = new SelectList(db.Motivo_Baja.Where(m => !m.eliminado), "id_motivo_baja", "descripcion");
            return View(empleado);
        }

        public ActionResult ActivarContrato(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratacion contrato = db.Contratacion.SingleOrDefault(c => c.id_contratacion == id);
            
            if (contrato == null)
            {
                return HttpNotFound();
            }
            var contratos = db.Contratacion.Where(c => c.activo && c.id_empleado == contrato.id_empleado);
            if(contratos.Count() > 0)
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Error, "El empleado ya tiene un contrato activo.");
                msg.ReturnUrl = Url.Action("Details", "Empleado", new { id = contrato.id_empleado });
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
            contrato.fecha_fin = null;
            contrato.activo = true;
            contrato.eliminado = false;
            contrato.id_estado_empleado = 1;
            contrato.id_motivo_baja = null;
            contrato.razon_baja = "";
            contrato.fecha_modificacion = DateTime.Now;
            contrato.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            db.Entry(contrato).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Details","Empleado", new { id = contrato.id_empleado});

        }
        // POST: rrhh/Contratacions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Contratacion contrato)
        {
            ModelState.Clear();
            if (db.Contratacion.Where(c => c.activo && !c.eliminado && c.id_empleado == contrato.id_empleado).Count() > 0)
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Error, "El empleado ya tiene un contrato activo en una empresa.");
                msg.ReturnUrl = Url.Action("Details", "Empleado", new { id = contrato.id_empleado });
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Puesto puesto = db.Puesto.SingleOrDefault(p => p.id_puesto == contrato.id_puesto);
                        contrato.id_ubicacion = (int)Catalogos.Ubicacion.Central;
                        Salario salario = new Salario();
                        salario.bono_decreto = puesto.bono_decreto;
                        salario.bono_extra = puesto.bono_extra;
                        salario.sueldo_base = puesto.sueldo_base;
                        salario.prestaciones = puesto.prestaciones;
                        salario.activo = contrato.activo = true;
                        salario.id_empleado = contrato.id_empleado;
                        salario.eliminado = contrato.eliminado = false;
                        salario.fecha_creacion = contrato.fecha_creacion = DateTime.Now;
                        salario.id_usuario_creacion = contrato.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;

                        contrato.id_estado_empleado = (int)Catalogos.Estado_Empleado.Alta;
                        db.Salario.Add(salario);
                        db.SaveChanges();

                        contrato.id_salario = salario.id_salario;
                        db.Contratacion.Add(contrato);

                        db.SaveChanges();

                        //Crear periodo de vacaciones para el nuevo contrato
                        Vacacion_Contrato vacacion_contrato = NuevoVacacion_Contrato();
                        vacacion_contrato.id_contratacion = contrato.id_contratacion;
                        vacacion_contrato.dias_total = 15;
                        vacacion_contrato.id_empleado = contrato.id_empleado;
                        vacacion_contrato.id_periodo = db.Periodo.Where(p => p.activo && !p.eliminado).FirstOrDefault().id_periodo;
                        db.Vacacion_Contrato.Add(vacacion_contrato);
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Details", "Empleado", new { id = contrato.id_empleado, error = "" });
                    }
                    catch
                    {
                        tran.Rollback();
                        ModelState.AddModelError("", "Fallo en la creación del contrato. Datos no guardados.");
                    }
                }
            }
            Empleado empleado = db.Empleado.SingleOrDefault(e => e.id_empleado == contrato.id_empleado && e.activo);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            ViewBag.empresa = new SelectList(db.Empresa.Where(p => !p.eliminado), "id_empresa", "nombre", contrato.id_empresa);
            ViewBag.fecha_inicio_contrato = contrato.fecha_inicio.ToString("dd/MM/yyyy");
            ViewBag.nombre_empleado =
                empleado.primer_nombre + " " +
                (!String.IsNullOrEmpty(empleado.segundo_nombre) ? empleado.segundo_nombre + " " : "") +
                empleado.primer_apellido + " " +
                (!String.IsNullOrEmpty(empleado.segundo_apellido) ? empleado.segundo_apellido : "");
            contrato = new Contratacion();
            contrato.id_empleado = empleado.id_empleado;
            return View(contrato);
        }

        public Vacacion_Contrato NuevoVacacion_Contrato()
        {
            Vacacion_Contrato vacacion_contrato = new Vacacion_Contrato();
            vacacion_contrato.fecha_creacion = DateTime.Now;
            vacacion_contrato.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            vacacion_contrato.activo = true;
            vacacion_contrato.eliminado = false;
            return vacacion_contrato;
        }

        // GET: rrhh/Contratacions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratacion contrato = db.Contratacion.SingleOrDefault(c => c.id_contratacion == id && c.activo);
            if (contrato == null)
            {
                return HttpNotFound();
            }
           
            ViewBag.estado_empleado = new SelectList(db.Estado_Empleado.Where(t => !t.eliminado).OrderBy(t => t.nombre), "id_estado_empleado", "nombre", contrato.id_estado_empleado);
            ViewBag.nombre_empleado =
                contrato.Empleado.primer_nombre + " " +
                (!String.IsNullOrEmpty(contrato.Empleado.segundo_nombre) ? contrato.Empleado.segundo_nombre + " " : "") +
                contrato.Empleado.primer_apellido + " " +
                (!String.IsNullOrEmpty(contrato.Empleado.segundo_apellido) ? contrato.Empleado.segundo_apellido : "");
            ViewBag.fecha_inicio_contrato = contrato.fecha_inicio.ToString("dd/MM/yyyy");
            return View(contrato);
        }

        // GET: rrhh/Contratacions/Edit/5
        public ActionResult EditPuesto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contratacion contrato = db.Contratacion.SingleOrDefault(c => c.id_contratacion == id && c.activo);
            if (contrato == null)
            {
                return HttpNotFound();
            }

            ViewBag.tipo_puesto = new SelectList(db.Tipo_Puesto.Where(p => !p.eliminado && p.id_empresa == contrato.Puesto.Tipo_Puesto.Empresa.id_empresa).OrderBy(p => p.nombre), "id_tipo_puesto", "nombre", contrato.Puesto.id_tipo_puesto);
            ViewBag.puesto = new SelectList(db.Puesto.Where(p => !p.eliminado && p.id_tipo_puesto == contrato.Puesto.id_tipo_puesto).OrderBy(p => p.nombre), "id_puesto", "nombre", contrato.id_puesto);

            ViewBag.nombre_empleado =
                contrato.Empleado.primer_nombre + " " +
                (!String.IsNullOrEmpty(contrato.Empleado.segundo_nombre) ? contrato.Empleado.segundo_nombre + " " : "") +
                contrato.Empleado.primer_apellido + " " +
                (!String.IsNullOrEmpty(contrato.Empleado.segundo_apellido) ? contrato.Empleado.segundo_apellido : "");
            ViewBag.fecha_inicio_contrato = contrato.fecha_inicio.ToString("dd/MM/yyyy");
            return View(contrato);
        }

        [HttpPost]
        public ActionResult Editar(Contratacion contrato)
        {
            ModelState.Clear();
            Contratacion editContrato = db.Contratacion.SingleOrDefault(c => c.activo && !c.eliminado && c.id_contratacion == contrato.id_contratacion);
            if (editContrato == null)
            {
                return RedirectToAction("Details", "Empleado", new { id = editContrato.id_empleado, error = "El contrato no se encuentra activo." });
            }
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (editContrato.id_estado_empleado != contrato.id_estado_empleado)
                        {
                            if (contrato.id_estado_empleado == (int)Catalogos.Estado_Empleado.Baja)
                            {
                                contrato.fecha_fin = DateTime.Now;
                            }
                            else
                            {
                                contrato.fecha_fin = null;
                                contrato.id_motivo_baja = null;
                                contrato.razon_baja = "";
                            }
                        }
                        editContrato.fecha_inicio = contrato.fecha_inicio;
                        editContrato.referido_por = contrato.referido_por;
                        editContrato.fecha_modificacion = DateTime.Now;
                        editContrato.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        db.Entry(editContrato).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Details", "Empleado", new { id = editContrato.id_empleado, error = "" });
                    }
                    catch
                    {
                        tran.Rollback();
                        ModelState.AddModelError("", "Error durante la operación. Datos no guardados.");
                    }
                }
            }
            ViewBag.fecha_inicio_contrato = editContrato.fecha_inicio.ToString("dd/MM/yyyy");
            ViewBag.empresa = new SelectList(db.Empresa.Where(e => !e.eliminado).OrderBy(e => e.nombre), "id_empresa", "nombre", editContrato.id_empresa);
            ViewBag.puesto = new SelectList(db.Puesto.Where(p => !p.eliminado).OrderBy(p => p.nombre), "id_puesto", "nombre", editContrato.id_puesto);
            ViewBag.estado_empleado = new SelectList(db.Estado_Empleado.OrderBy(e => e.nombre), "id_estado_empleado", "nombre", editContrato.id_estado_empleado);
            ViewBag.nombre_empleado =
                editContrato.Empleado.primer_nombre + " " +
                (!String.IsNullOrEmpty(editContrato.Empleado.segundo_nombre) ? editContrato.Empleado.segundo_nombre + " " : "") +
                editContrato.Empleado.primer_apellido + " " +
                (!String.IsNullOrEmpty(editContrato.Empleado.segundo_apellido) ? editContrato.Empleado.segundo_apellido : "");

            return View("Edit", editContrato);
        }

        [HttpPost]
        public ActionResult EditarPuesto(Contratacion contrato)
        {
            ModelState.Clear();
            Contratacion editContrato = db.Contratacion.SingleOrDefault(c => c.activo && !c.eliminado && c.id_contratacion == contrato.id_contratacion);
            if (editContrato == null)
            {
                return RedirectToAction("Details", "Empleado", new { id = editContrato.id_empleado, error = "El contrato no se encuentra activo." });
            }
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (editContrato.id_puesto != contrato.id_puesto)
                        {
                            Puesto puesto = db.Puesto.SingleOrDefault(p => p.id_puesto == contrato.id_puesto);
                            editContrato.id_puesto = contrato.id_puesto;
                            editContrato.Salario.bono_decreto = puesto.bono_decreto;
                            editContrato.Salario.bono_extra = puesto.bono_extra;
                            editContrato.Salario.prestaciones = puesto.prestaciones;
                            editContrato.Salario.sueldo_base = puesto.sueldo_base;
                            editContrato.Salario.fecha_modificacion = DateTime.Now;
                            editContrato.Salario.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                            db.Entry(editContrato.Salario).State = EntityState.Modified;
                            editContrato.fecha_modificacion = DateTime.Now;
                            editContrato.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                            db.Entry(editContrato).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Details", "Empleado", new { id = editContrato.id_empleado, error = "" });
                    }
                    catch
                    {
                        tran.Rollback();
                        ModelState.AddModelError("", "Error durante la operación. Datos no guardados.");
                    }
                }
            }
            ViewBag.fecha_inicio_contrato = editContrato.fecha_inicio.ToString("dd/MM/yyyy");
            ViewBag.empresa = new SelectList(db.Empresa.Where(e => !e.eliminado).OrderBy(e => e.nombre), "id_empresa", "nombre", editContrato.id_empresa);
            ViewBag.puesto = new SelectList(db.Puesto.Where(p => !p.eliminado).OrderBy(p => p.nombre), "id_puesto", "nombre", editContrato.id_puesto);
            ViewBag.estado_empleado = new SelectList(db.Estado_Empleado.OrderBy(e => e.nombre), "id_estado_empleado", "nombre", editContrato.id_estado_empleado);
            ViewBag.nombre_empleado =
                editContrato.Empleado.primer_nombre + " " +
                (!String.IsNullOrEmpty(editContrato.Empleado.segundo_nombre) ? editContrato.Empleado.segundo_nombre + " " : "") +
                editContrato.Empleado.primer_apellido + " " +
                (!String.IsNullOrEmpty(editContrato.Empleado.segundo_apellido) ? editContrato.Empleado.segundo_apellido : "");

            return View("EditarPuesto", editContrato);
        }
        
        [HttpPost]
        public ActionResult Terminar_Contrato(int id, DateTime fecha_contrato, int motivo_baja, string razon_baja)
        {
            Contratacion contratacion = db.Contratacion.SingleOrDefault(c => c.id_contratacion == id && c.activo);
            if (contratacion == null)
            {
                return HttpNotFound();
            }
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    contratacion.Salario.fecha_modificacion = DateTime.Now;
                    contratacion.fecha_fin = fecha_contrato;
                    contratacion.fecha_modificacion = DateTime.Now;
                    contratacion.id_motivo_baja = motivo_baja;
                    contratacion.razon_baja = razon_baja;
                    contratacion.id_estado_empleado = (int)Catalogos.Estado_Empleado.Baja;
                    contratacion.id_usuario_modificacion = contratacion.Salario.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    contratacion.activo = false;
                    contratacion.Salario.activo = false;
                    db.Entry(contratacion.Salario).State = EntityState.Modified;
                    db.Entry(contratacion).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Details", "Empleado", new { id = contratacion.id_empleado, error = "" });
                }
                catch
                {
                    tran.Rollback();
                    return View("Details", "Empleado", new { id = contratacion.id_empleado, error = "Error. No se guardaron los cambios efectuados." });
                }
            }
        }

        #region Asignar nuevo codigo a empleado existente

        public ActionResult NuevoCodigo()
        {
            ViewBag.empresa = new SelectList(db.Empresa.Where(e => !e.eliminado), "id_empresa", "nombre");
            Contratacion contrato = new Contratacion();
            return View(contrato);
        }

        [HttpPost]
        public ActionResult NuevoCodigoEmpleado(Contratacion contrato)
        {
            Empleado empleado = db.Empleado.SingleOrDefault(e => e.id_empleado == contrato.id_empleado && !e.eliminado);
            if (empleado == null)
            {
                return HttpNotFound();
            }
            if (empleado.Contratacion.Where(c => c.activo).Count() > 0)
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Info, "No se pudo crear el empleado a partir de la información existente. El empleado seleccionado todavía esta de Alta..");
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
                    //Copiar Telefonos
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
                    //Copiar direcciones
                    foreach (var direccion in empleado.Empleado_Direcciones.Where(d => d.activo && !d.eliminado))
                    {
                        Empleado_Direcciones nuevaDireccion = CopiarDireccion(direccion);
                        nuevaDireccion.id_empleado = nuevoEmpleado.id_empleado;
                        db.Empleado_Direcciones.Add(nuevaDireccion);
                    }
                    //Copiar Archivos
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
                    //Asignar Salario
                    Salario salario = new Salario();
                    Puesto puesto = db.Puesto.SingleOrDefault(p => p.id_puesto == contrato.id_puesto);
                    salario.bono_decreto = puesto.bono_decreto;
                    salario.bono_extra = puesto.bono_extra;
                    salario.sueldo_base = puesto.sueldo_base;
                    salario.prestaciones = puesto.prestaciones;
                    salario.activo = true;
                    salario.id_empleado = contrato.id_empleado;
                    salario.eliminado = false;
                    salario.fecha_creacion = contrato.fecha_creacion = DateTime.Now;
                    salario.id_usuario_creacion = id_usuario;

                    db.Salario.Add(salario);
                    db.SaveChanges();

                    //Crear nuevo contrato
                    contrato.id_salario = salario.id_salario;
                    contrato.id_empleado = nuevoEmpleado.id_empleado;
                    contrato.id_ubicacion = contrato.id_ubicacion == 0 ? (int)Catalogos.Ubicacion.Central : contrato.id_ubicacion;
                    contrato.fecha_creacion = DateTime.Now;
                    contrato.id_usuario_creacion = id_usuario;
                    contrato.id_estado_empleado = (int)Catalogos.Estado_Empleado.Alta;
                    contrato.activo = true;
                    contrato.eliminado = false;
                    db.Contratacion.Add(contrato);
                    db.SaveChanges();

                    //Crear periodo de vacaciones para el nuevo contrato
                    Vacacion_Contrato vacacion_contrato = NuevoVacacion_Contrato();
                    vacacion_contrato.id_contratacion = contrato.id_contratacion;
                    vacacion_contrato.dias_total = 15;
                    vacacion_contrato.id_empleado = contrato.id_empleado;
                    vacacion_contrato.id_periodo = db.Periodo.Where(p => p.activo && !p.eliminado).FirstOrDefault().id_periodo;
                    db.Vacacion_Contrato.Add(vacacion_contrato);
                    db.SaveChanges();

                    tran.Commit();
                    return RedirectToAction("Details", "Empleado", new { id = nuevoEmpleado.id_empleado });
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

        public JsonResult ExisteEmpleado(int id_empleado)
        {
            object resultado = new object();
            Empleado empleado = db.Empleado.SingleOrDefault(e => e.activo && e.id_empleado == id_empleado);
            if (empleado == null)
            {
                resultado = new { error = true };
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            if (empleado.Contratacion.Where(e => e.activo).Count() > 0)
            {
                resultado = new { error = true, msg = "El empleado tiene un contrato activo." };
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            var contrato = empleado.Contratacion.Where(e => !e.eliminado).OrderByDescending(c => c.fecha_fin).First();
            string ubicacion = "No se ha encontrado una ubicación previa.";
            int id_ubicacion = 0;
            if (contrato != null)
            {
                if (contrato.id_ubicacion.HasValue)
                {
                    id_ubicacion = contrato.id_ubicacion.Value;
                    ubicacion = contrato.Ubicaciones.direccion;
                }
            }
            resultado = new
            {
                error = false,
                id_empleado,
                nombre =
                    empleado.primer_nombre + " " +
                    (!String.IsNullOrEmpty(empleado.segundo_nombre) ? empleado.segundo_nombre + " " : "") +
                    empleado.primer_apellido + " " +
                    (!String.IsNullOrEmpty(empleado.segundo_apellido) ? empleado.segundo_apellido : ""),
                ubicacion,
                id_ubicacion
            };
            return Json(resultado, JsonRequestBehavior.AllowGet);
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
