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
    public class Rol_AccionesController : Controller
    {
        private AppEntities db = new AppEntities(Constantes.getDataSource());

        // GET: Administracion/Rol_Acciones
        public ActionResult Index()
        {
            db = new AppEntities();
            var rol_Acciones = db.Rol_Acciones.Include(r => r.Acciones).Include(r => r.Controladores).Include(r => r.Roles);
            return View(rol_Acciones.ToList());
        }

        // GET: Administracion/Rol_Acciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rol_Acciones rol_Acciones = db.Rol_Acciones.Find(id);
            if (rol_Acciones == null)
            {
                return HttpNotFound();
            }
            return View(rol_Acciones);
        }

        // GET: Administracion/Rol_Acciones/Create
        public ActionResult Create()
        {
            ViewBag.id_accion = new SelectList(db.Acciones, "id_accion", "nombre");
            ViewBag.id_controlador = new SelectList(db.Controladores, "id_controlador", "nombre");
            ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "nombre");
            return View();
        }

        // POST: Administracion/Rol_Acciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_rol,id_controlador,id_accion")] Rol_Acciones rol_Acciones)
        {
            if (ModelState.IsValid)
            {
                db.Rol_Acciones.Add(rol_Acciones);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_accion = new SelectList(db.Acciones, "id_accion", "nombre", rol_Acciones.id_accion);
            ViewBag.id_controlador = new SelectList(db.Controladores, "id_controlador", "nombre", rol_Acciones.id_controlador);
            ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "nombre", rol_Acciones.id_rol);
            return View(rol_Acciones);
        }

        // GET: Administracion/Rol_Acciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rol_Acciones rol_Acciones = db.Rol_Acciones.Find(id);
            if (rol_Acciones == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_accion = new SelectList(db.Acciones, "id_accion", "nombre", rol_Acciones.id_accion);
            ViewBag.id_controlador = new SelectList(db.Controladores, "id_controlador", "nombre", rol_Acciones.id_controlador);
            ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "nombre", rol_Acciones.id_rol);
            return View(rol_Acciones);
        }

        // POST: Administracion/Rol_Acciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_rol,id_controlador,id_accion")] Rol_Acciones rol_Acciones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rol_Acciones).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_accion = new SelectList(db.Acciones, "id_accion", "nombre", rol_Acciones.id_accion);
            ViewBag.id_controlador = new SelectList(db.Controladores, "id_controlador", "nombre", rol_Acciones.id_controlador);
            ViewBag.id_rol = new SelectList(db.Roles, "id_rol", "nombre", rol_Acciones.id_rol);
            return View(rol_Acciones);
        }

        // GET: Administracion/Rol_Acciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rol_Acciones rol_Acciones = db.Rol_Acciones.Find(id);
            if (rol_Acciones == null)
            {
                return HttpNotFound();
            }
            return View(rol_Acciones);
        }

        // POST: Administracion/Rol_Acciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rol_Acciones rol_Acciones = db.Rol_Acciones.Find(id);
            db.Rol_Acciones.Remove(rol_Acciones);
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
    }
}
