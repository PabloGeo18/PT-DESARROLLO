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
using System.Globalization;
using MVC2013.Src.Comun.View;

namespace MVC2013.Areas.rrhh.Controllers
{
    public class PuestoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Puestoes
        public ActionResult Index()
        {
            var puesto = db.Puesto.Where(p => !p.eliminado).OrderBy(p => p.nombre);
            return View(puesto);
        }

        // GET: rrhh/Puestoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Puesto puesto = db.Puesto.SingleOrDefault(p => p.id_puesto == id && p.activo);
            if (puesto == null)
            {
                return HttpNotFound();
            }
            return View(puesto);
        }

        // GET: rrhh/Puestoes/Create
        public ActionResult Create()
        {
            ViewBag.tipo_puesto = new SelectList(db.Tipo_Puesto.Where(t => !t.eliminado), "id_tipo_puesto", "nombre");
            Puesto puesto = new Puesto();
            ViewBag.empresa = new SelectList(db.Empresa.Where(e => !e.eliminado), "id_empresa", "nombre");
            return View(puesto);
        }

        // POST: rrhh/Puestoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(Puesto puesto)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    puesto.nombre = puesto.nombre.ToUpper();
                    Tipo_Puesto tipo_puesto = db.Tipo_Puesto.SingleOrDefault(t => t.activo && !t.eliminado && t.id_tipo_puesto == puesto.id_tipo_puesto);
                    if (tipo_puesto == null)
                    {
                        ModelState.AddModelError("", "Tipo de Puesto seleccionado no valido.");
                        ViewBag.tipo_puesto = new SelectList(db.Tipo_Puesto.Where(t => !t.eliminado), "id_tipo_puesto", "nombre");
                        ViewBag.empresa = new SelectList(db.Empresa.Where(e => !e.eliminado), "id_empresa", "nombre");
                        return View(puesto);
                    }
                    decimal bono_extra = Convert.ToDecimal(!String.IsNullOrEmpty(Request["bono_extra"]) ? Request["bono_extra"] : "0", CultureInfo.InvariantCulture);
                    decimal bono_decreto = Convert.ToDecimal(!String.IsNullOrEmpty(Request["bono_decreto"]) ? Request["bono_decreto"] : "0", CultureInfo.InvariantCulture);
                    decimal sueldo_base = Convert.ToDecimal(Request["sueldo_base"], CultureInfo.InvariantCulture);
                    puesto.bono_decreto = bono_decreto;
                    puesto.bono_extra = bono_extra;
                    puesto.sueldo_base = sueldo_base;
                    decimal sueldo = sueldo_base + bono_extra + bono_decreto;
                    if (sueldo < tipo_puesto.salario_minimo || sueldo > tipo_puesto.salario_maximo)
                    {
                        ModelState.AddModelError("", "El sueldo ingresado para el puesto no entra en el rango establecido.");
                        ViewBag.tipo_puesto = new SelectList(db.Tipo_Puesto.Where(t => !t.eliminado), "id_tipo_puesto", "nombre");
                        ViewBag.empresa = new SelectList(db.Empresa.Where(e => !e.eliminado), "id_empresa", "nombre");
                        return View(puesto);
                    }
                    puesto.fecha_creacion = DateTime.Now;
                    puesto.activo = true;
                    puesto.eliminado = false;
                    puesto.prestaciones = tipo_puesto.Empresa.prestaciones;
                    puesto.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Puesto.Add(puesto);
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    tran.Rollback();
                    ModelState.AddModelError("", "Error durante la operación. Datos no guardados.");
                    ViewBag.empresa = new SelectList(db.Empresa.Where(e => !e.eliminado), "id_empresa", "nombre");
                    ViewBag.id_tipo_puesto = new SelectList(db.Tipo_Puesto.Where(t => !t.eliminado), "id_tipo_puesto", "nombre");
                    return View(puesto);
                }
            }
        }

        public Puesto NuevoPuesto()
        {
            Puesto puesto = new Puesto();
            puesto.activo = true;
            puesto.eliminado = false;
            puesto.fecha_creacion = DateTime.Now;
            puesto.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return puesto;
        }

        // GET: rrhh/Puestoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Puesto puesto = db.Puesto.SingleOrDefault(p => p.id_puesto == id && p.activo);
            if (puesto == null)
            {
                return HttpNotFound();
            }
            ViewBag.tipo_puesto = new SelectList(db.Tipo_Puesto.Where(t => !t.eliminado && t.id_empresa == puesto.Tipo_Puesto.id_empresa), "id_tipo_puesto", "nombre", puesto.id_tipo_puesto);
            ViewBag.id_documento = new SelectList(db.Documentos.Where(e => e.activo && !e.eliminado), "id_documento", "nombre");
            return View(puesto);
        }

        public JsonResult AgregarDocumento(int id_puesto, int id_documento)
        {
            try
            {
                Puesto puesto = db.Puesto.SingleOrDefault(p => p.activo && p.id_puesto == id_puesto);
                Documentos documento = db.Documentos.SingleOrDefault(d => d.activo && d.id_documento == id_documento);
                if (puesto == null)
                {
                    return Json(new { respuesta = false, msg = "No se encontró el puesto respectivo." }, JsonRequestBehavior.AllowGet);
                }
                if (documento == null)
                {
                    return Json(new { respuesta = false, msg = "No se encontró el documento asignado." }, JsonRequestBehavior.AllowGet);
                }
                Documento_Puesto documento_puesto = new Documento_Puesto();
                documento_puesto.id_puesto = id_puesto;
                documento_puesto.id_documento = id_documento;
                documento_puesto.activo = true;
                documento_puesto.eliminado = false;
                documento_puesto.fecha_creacion = DateTime.Now;
                documento_puesto.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Documento_Puesto.Add(documento_puesto);
                db.SaveChanges();
                return Json(new { respuesta = true, nombre = documento.nombre, id = documento_puesto.id_documento_puesto }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { respuesta = false, msg = "No se pudo asignar correctamente el documento al puesto." }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult RemoverDocumento(int id)
        {
            Documento_Puesto documento_puesto = db.Documento_Puesto.SingleOrDefault(d => d.id_documento_puesto == id && d.activo);
            if (documento_puesto == null)
            {
                return Json(new { respuesta = false, msg = "No se encontro el documento asignado al puesto." }, JsonRequestBehavior.AllowGet);
            }
            documento_puesto.activo = false;
            documento_puesto.eliminado = true;
            documento_puesto.fecha_eliminacion = DateTime.Now;
            documento_puesto.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            try
            {
                db.Entry(documento_puesto).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { respuesta = true }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { respuesta = false, msg = "No se pudo eliminar el documento del puesto." }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: rrhh/Puestoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Puesto puesto, string salario_minimo, string salario_maximo)
        {
            ModelState.Clear();
            Puesto editPuesto = db.Puesto.SingleOrDefault(e => e.id_puesto == puesto.id_puesto && e.activo);
            if (editPuesto == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Tipo_Puesto tipo_puesto = db.Tipo_Puesto.SingleOrDefault(t => t.activo && !t.eliminado && t.id_tipo_puesto == puesto.id_tipo_puesto);
                        if (tipo_puesto == null)
                        {
                            ModelState.AddModelError("", "Tipo de Puesto seleccionado no valido.");
                            ViewBag.tipo_puesto = new SelectList(db.Tipo_Puesto.Where(t => !t.eliminado), "id_tipo_puesto", "nombre");
                            ViewBag.id_documento = new SelectList(db.Documentos.Where(e => e.activo && !e.eliminado), "id_documento", "reporte");
                            return View(editPuesto);
                        }
                        decimal bono_extra = Convert.ToDecimal(!String.IsNullOrEmpty(Request["bono_extra"]) ? Request["bono_extra"] : "0", CultureInfo.InvariantCulture);
                        decimal bono_decreto = Convert.ToDecimal(!String.IsNullOrEmpty(Request["bono_decreto"]) ? Request["bono_decreto"] : "0", CultureInfo.InvariantCulture);
                        decimal sueldo_base = Convert.ToDecimal(Request["sueldo_base"], CultureInfo.InvariantCulture);
                        puesto.bono_decreto = bono_decreto;
                        puesto.bono_extra = bono_extra;
                        puesto.sueldo_base = sueldo_base;
                        decimal sueldo = sueldo_base + bono_extra + bono_decreto;
                        if (sueldo < tipo_puesto.salario_minimo || sueldo > tipo_puesto.salario_maximo)
                        {
                            ModelState.AddModelError("", "El sueldo ingresado para el puesto no entra en el rango establecido.");
                            ViewBag.tipo_puesto = new SelectList(db.Tipo_Puesto.Where(t => !t.eliminado), "id_tipo_puesto", "nombre");
                            ViewBag.id_documento = new SelectList(db.Documentos.Where(e => e.activo && !e.eliminado), "id_documento", "reporte");
                            return View(editPuesto);
                        }
                        editPuesto.nombre = puesto.nombre.ToUpper();
                        editPuesto.id_tipo_puesto = puesto.id_tipo_puesto;
                        editPuesto.bono_decreto = puesto.bono_decreto;
                        editPuesto.bono_extra = puesto.bono_extra;
                        editPuesto.sueldo_base = puesto.sueldo_base;
                        editPuesto.prestaciones = tipo_puesto.Empresa.prestaciones;
                        editPuesto.fecha_modificacion = DateTime.Now;
                        editPuesto.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        db.Entry(editPuesto).State = EntityState.Modified;
                        db.CambioSalarioPuesto(editPuesto.sueldo_base, editPuesto.bono_decreto,
                            editPuesto.bono_extra, Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario,
                            editPuesto.id_puesto);
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        tran.Rollback();
                        ModelState.AddModelError("", "Error durante la operación. Datos no guardados.");
                    }
                }
            }
            ViewBag.id_documento = new SelectList(db.Documentos.Where(e => e.activo && !e.eliminado), "id_documento", "nombre");
            ViewBag.tipo_puesto = new SelectList(db.Tipo_Puesto.Where(t => !t.eliminado), "id_tipo_puesto", "nombre", puesto.id_tipo_puesto);
            return View(editPuesto);
        }

        // GET: rrhh/Puestoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Puesto puesto = db.Puesto.SingleOrDefault(p => p.activo && p.id_puesto == id);
            if (puesto == null)
            {
                return HttpNotFound();
            }
            return View(puesto);
        }

        // POST: rrhh/Puestoes/Delete/5
        public ActionResult Eliminar(int id)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Puesto puesto = db.Puesto.SingleOrDefault(p => p.activo && p.id_puesto == id);
                    if (puesto == null)
                    {
                        return HttpNotFound();
                    }
                    puesto.activo = false;
                    puesto.eliminado = true;
                    puesto.fecha_eliminacion = DateTime.Now;
                    puesto.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(puesto).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, "Error durante la conexión con el servidor. Cambios no efectuados.");
                    msg.ReturnUrl = Url.Action("EstadoFuerza");
                    TempData[User.Identity.Name] = msg;
                    return RedirectToAction("Mensaje");
                }
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
