using MVC2013.Models;
using MVC2013.Src.Comun.Exception;
using MVC2013.Src.Comun.View;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MVC2013.Areas.Administracion.Controllers
{
    public class PermisosController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Permisos
        public ActionResult Index()
        {
            ViewBag.aplicaciones = new SelectList(db.Aplicaciones, "id_aplicacion", "nombre");
            return View();
        }

        [HttpPost]
        public ActionResult Index(int? id_aplicacion) 
        {
            if (id_aplicacion == null) 
            {
                ModelState.AddModelError(String.Empty, "Debes seleccionar una aplicacion");
            }

            if (ModelState.IsValid) 
            {
                return RedirectToAction("Edit", new { idAplicacion = (int)id_aplicacion });
            }

            ViewBag.aplicaciones = new SelectList(db.Aplicaciones, "id_aplicacion", "nombre", id_aplicacion);
            return View();
        }

        public ActionResult Edit(int? idAplicacion, int? idRol, int? idControlador) 
        {
            if (idAplicacion == null) 
            {
                return RedirectToAction("Index");
            }

            Aplicaciones aplicacion = db.Aplicaciones.Find(idAplicacion);

            if (aplicacion == null) 
            {
                return HttpNotFound();
            }

            ViewBag.roles = new SelectList(aplicacion.Roles, "id_rol", "nombre", idRol);
            ViewBag.controladores = new SelectList(aplicacion.Controladores, "id_controlador", "nombre", idControlador);
            ViewBag.aplicacion = aplicacion;
            return View();
        }

        [HttpGet]
        public JsonResult GetAcciones(int idControlador, int idRol) 
        {
            List<object> resultado = new List<object>();
            Controladores controlador = db.Controladores.Find(idControlador);
            Roles rol = db.Roles.Find(idRol);

            if (rol == null || controlador == null) 
            {
                goto Result;
            }

            List<int> accesos = new List<int>();
            rol.Rol_Acciones.Where(ra => ra.id_controlador == controlador.id_controlador).ToList().ForEach(ra => accesos.Add(ra.id_accion));

            //Agregamos las acciones a la lista del resultado y verificamos si tiene permiso
            db.Acciones.Where(a =>
                a.id_controlador == controlador.id_controlador
            ).ToList()
            .ForEach(a => resultado.Add(new { id = a.id_accion, label = a.nombre, tiene_acceso = accesos.Contains(a.id_accion) ? 1 : 0 }));

        Result:
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SwitchAccess(int idControlador, int idRol, int idAccion) 
        {
            Controladores controlador = db.Controladores.Find(idControlador);
            Roles rol = db.Roles.Find(idRol);
            Acciones accion = db.Acciones.Find(idAccion);

            if (controlador == null || rol == null || accion == null) 
            {
                return Json(new { resultado = 0 }, JsonRequestBehavior.AllowGet);    
            }

            try
            {
                Rol_Acciones permiso = db.Rol_Acciones.Where(ra => ra.id_accion == accion.id_accion && ra.id_controlador == controlador.id_controlador && ra.id_rol == rol.id_rol).DefaultIfEmpty(null).FirstOrDefault();

                if (permiso == null)
                {
                    Rol_Acciones nuevoPermiso = new Rol_Acciones();
                    nuevoPermiso.id_rol = rol.id_rol;
                    nuevoPermiso.id_controlador = controlador.id_controlador;
                    nuevoPermiso.id_accion = accion.id_accion;
                    db.Rol_Acciones.Add(nuevoPermiso);
                }
                else
                {
                    db.Rol_Acciones.Remove(permiso);
                }
                db.SaveChanges();

                return Json(new { resultado = 1 }, JsonRequestBehavior.AllowGet);    
            }
            catch 
            {
                return Json(new { resultado = 0 }, JsonRequestBehavior.AllowGet);    
            }
        }

        public ActionResult AddAllByController(int idControlador, int id_rol)
        {
            Controladores controlador = db.Controladores.Find(idControlador);
            Roles rol = db.Roles.Find(id_rol);

            if (rol == null || controlador == null)
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Warning, "El rol o controlador no son validos");
                msg.ReturnUrl = Url.Action("Index");
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (Acciones accion in controlador.Acciones)
                    {
                        Rol_Acciones permiso = db.Rol_Acciones.Where(ra => ra.id_accion == accion.id_accion && ra.id_rol == rol.id_rol && ra.id_controlador == controlador.id_controlador).DefaultIfEmpty(null).FirstOrDefault();
                        if (permiso == null)
                        {
                            Rol_Acciones rolAccion = new Rol_Acciones();
                            rolAccion.id_accion = accion.id_accion;
                            rolAccion.id_rol = rol.id_rol;
                            rolAccion.id_controlador = controlador.id_controlador;
                            db.Rol_Acciones.Add(rolAccion);
                        }
                    }
                    db.SaveChanges();

                    tran.Commit();
                    return RedirectToAction("Edit", new { idAplicacion = controlador.id_aplicacion, idRol = rol.id_rol, idControlador = controlador.id_controlador });
                }
                catch
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, App_GlobalResources.Resources.error_inesperado);
                    msg.ReturnUrl = Url.Action("Edit", new { idAplicacion = controlador.id_aplicacion });
                    return RedirectToAction("Mensaje");
                }
            }
        }

        public ActionResult RemoveAllByController(int idControlador, int id_rol)
        {
            Controladores controlador = db.Controladores.Find(idControlador);
            Roles rol = db.Roles.Find(id_rol);

            if (rol == null || controlador == null)
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Warning, "El rol o controlador no son validos");
                msg.ReturnUrl = Url.Action("Index");
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }

            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    db.Rol_Acciones.RemoveRange(controlador.Rol_Acciones.Where(ra => ra.id_rol == rol.id_rol));
                    db.SaveChanges();

                    tran.Commit();
                    return RedirectToAction("Edit", new { idAplicacion = controlador.id_aplicacion, idRol = rol.id_rol, idControlador = controlador.id_controlador});
                }
                catch
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, App_GlobalResources.Resources.error_inesperado);
                    msg.ReturnUrl = Url.Action("Edit", new { idAplicacion = controlador.id_aplicacion });
                    return RedirectToAction("Mensaje");
                }
            }
        }

        public ActionResult AddAllByApp(int idAplicacion, int id_rol) 
        { 
            Aplicaciones aplicacion = db.Aplicaciones.Find(idAplicacion);
            Roles rol = db.Roles.Find(id_rol);

            if(rol == null || aplicacion == null)
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Warning, "El rol o la aplicacion no son validos");
                msg.ReturnUrl = Url.Action("Index");
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }

            List<Controladores> controladores = aplicacion.Controladores.ToList();

            using (DbContextTransaction tran = db.Database.BeginTransaction()) 
            {
                try
                {
                    foreach (Controladores controlador in controladores)
                    {
                        foreach (Acciones accion in controlador.Acciones)
                        {
                            Rol_Acciones permiso = db.Rol_Acciones.Where(ra => ra.id_accion == accion.id_accion && ra.id_rol == rol.id_rol && ra.id_controlador == controlador.id_controlador).DefaultIfEmpty(null).FirstOrDefault();
                            if (permiso == null)
                            {
                                Rol_Acciones rolAccion = new Rol_Acciones();
                                rolAccion.id_accion = accion.id_accion;
                                rolAccion.id_rol = rol.id_rol;
                                rolAccion.id_controlador = controlador.id_controlador;
                                db.Rol_Acciones.Add(rolAccion);
                            }
                        }
                    }
                    db.SaveChanges();
                    
                    ContextMessage msg = new ContextMessage(ContextMessage.Success, "Permisos agregados con éxito");
                    msg.ReturnUrl = Url.Action("Edit", new { idAplicacion = aplicacion.id_aplicacion, idRol = rol.id_rol });
                    TempData[User.Identity.Name] = msg;

                    tran.Commit();
                    return RedirectToAction("Mensaje");
                }
                catch 
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, App_GlobalResources.Resources.error_inesperado);
                    msg.ReturnUrl = Url.Action("Edit", new { idAplicacion = aplicacion.id_aplicacion });
                    return RedirectToAction("Mensaje");
                }
            }
        }

        public ActionResult RemoveAllByApp(int idAplicacion, int id_rol)
        {
            Aplicaciones aplicacion = db.Aplicaciones.Find(idAplicacion);
            Roles rol = db.Roles.Find(id_rol);

            if (rol == null || aplicacion == null)
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Warning, "El rol o la aplicacion no son validos");
                msg.ReturnUrl = Url.Action("Index");
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }

            List<Controladores> controladores = aplicacion.Controladores.ToList();

            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    aplicacion.Controladores
                        .ToList()
                        .ForEach(c => 
                            db.Rol_Acciones.RemoveRange(c.Rol_Acciones.Where(ra => ra.id_rol == rol.id_rol))
                        ); 
   
                    db.SaveChanges();

                    ContextMessage msg = new ContextMessage(ContextMessage.Success, "Permisos bloqueados con éxito");
                    msg.ReturnUrl = Url.Action("Edit", new { idAplicacion = aplicacion.id_aplicacion, idRol = rol.id_rol });
                    TempData[User.Identity.Name] = msg;

                    tran.Commit();
                    return RedirectToAction("Mensaje");
                }
                catch
                {
                    tran.Rollback();
                    ContextMessage msg = new ContextMessage(ContextMessage.Error, App_GlobalResources.Resources.error_inesperado);
                    msg.ReturnUrl = Url.Action("Edit", new { idAplicacion = aplicacion.id_aplicacion });
                    return RedirectToAction("Mensaje");
                }
            }
        }

        public ViewResult Mensaje() 
        {
            ContextMessage msg = TempData.ContainsKey(User.Identity.Name) && TempData[User.Identity.Name].GetType() == typeof(ContextMessage) ? (ContextMessage)TempData[User.Identity.Name] : new ContextMessage();
            return View(ContextMessage.ViewLocation(this), msg);
        }
    }
}