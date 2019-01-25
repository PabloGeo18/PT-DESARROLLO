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

namespace MVC2013.Areas.Administracion.Controllers
{
    public class RolesController : Controller
    {
        private AppEntities db = new AppEntities(Constantes.getDataSource());

        // GET: Administracion/Roles
        public ActionResult Index()
        {
            var roles = db.Roles.Include(r => r.Aplicaciones);
            return View(roles.ToList());
        }

        // GET: Administracion/Roles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.Roles.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }
            return View(roles);
        }

        // GET: Administracion/Roles/Create
        public ActionResult Create()
        {
            ViewBag.id_aplicacion = new SelectList(db.Aplicaciones, "id_aplicacion", "nombre");
            return View();
        }

        // POST: Administracion/Roles/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_rol,nombre,id_aplicacion")] Roles roles)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(roles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_aplicacion = new SelectList(db.Aplicaciones, "id_aplicacion", "nombre", roles.id_aplicacion);
            return View(roles);
        }

        // GET: Administracion/Roles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.Roles.Find(id);
            var controladoresPorAplicacion = roles.Aplicaciones.Controladores;

            if (roles == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_aplicacion = new SelectList(db.Aplicaciones, "id_aplicacion", "nombre", roles.id_aplicacion);
            ViewBag.id_controlador = new SelectList(controladoresPorAplicacion, "id_controlador", "nombre");
            return View(roles);
        }

        // POST: Administracion/Roles/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_rol,nombre,id_aplicacion")] Roles roles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_aplicacion = new SelectList(db.Aplicaciones, "id_aplicacion", "nombre", roles.id_aplicacion);
            return View(roles);
        }

        // GET: Administracion/Roles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles roles = db.Roles.Find(id);
            if (roles == null)
            {
                return HttpNotFound();
            }
            return View(roles);
        }

        // POST: Administracion/Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Roles roles = db.Roles.Find(id);
            db.Roles.Remove(roles);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        ///////////////////////

        public ActionResult GetAccionesPorControlador(int? id)
        {
            List<object> listReturn = new List<object>();
            if(id == null)
                return Json(listReturn);

            Controladores controlador = db.Controladores.Find(id);
            foreach (Acciones accion in controlador.Acciones)
            {
                listReturn.Add(new { nombre = accion.nombre, id = accion.id_accion });
            }

            return Json(listReturn);
        }


        public ActionResult CreateRoleAccion(int idRol, int idAccion)
        {
            Acciones accion = db.Acciones.Find(idAccion);

            Rol_Acciones rol_AccionVerification =
                db.Rol_Acciones.Where(x =>
                    x.id_accion == idAccion &&
                    x.id_controlador == accion.Controladores.id_controlador &&
                    x.id_rol == idRol
                ).DefaultIfEmpty(null).Single();

            if (rol_AccionVerification == null) { 
                Roles rol = db.Roles.Find(idRol);            
                Rol_Acciones rolAccion = new Rol_Acciones();
                rolAccion.Roles = rol;
                rolAccion.id_rol = idRol;
                rolAccion.Acciones = accion;
                rolAccion.id_accion = idAccion;
                rolAccion.Controladores = accion.Controladores;
                rol.Rol_Acciones.Add(rolAccion);
                db.SaveChanges();
            }

            return new EmptyResult();
        }

        public ActionResult RemoveRoleAccion(int idRol, int idAccion, int idControlador)
        {

            Rol_Acciones rol_Accion = 
                db.Rol_Acciones.Where(x =>          
                    x.id_accion == idAccion &&
                    x.id_controlador == idControlador &&
                    x.id_rol == idRol
                ).DefaultIfEmpty(null).Single();

            if (rol_Accion != null) { 
                db.Rol_Acciones.Remove(rol_Accion);
                db.SaveChanges();
            }

            return RedirectToAction("Edit", new { id = idRol }); ;
        }










    }
}
