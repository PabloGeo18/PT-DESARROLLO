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

namespace MVC2013.Areas.EstadoFuerza.Controllers
{
    public class SituacionController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: EstadoFuerza/Situacion
        public ActionResult Index()
        {
            var situacion = db.Situacion.Where(s => s.activo && !s.eliminado);
            return View(situacion);
        }

        // GET: EstadoFuerza/Situacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Situacion situacion = db.Situacion.SingleOrDefault(s=>s.id_situacion == id && s.activo && !s.eliminado);
            if (situacion == null)
            {
                return HttpNotFound();
            }
            return View(situacion);
        }

        // GET: EstadoFuerza/Situacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EstadoFuerza/Situacion/Create
        [HttpPost]
        public ActionResult Create(Situacion situacion)
        {
            if (ModelState.IsValid)
            {
                situacion.activo = true;
                situacion.eliminado = false;
                situacion.fecha_creacion = DateTime.Now;
                situacion.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Situacion.Add(situacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(situacion);
        }

        // GET: EstadoFuerza/Situacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Situacion situacion = db.Situacion.SingleOrDefault(s => s.id_situacion == id && s.activo && !s.eliminado);
            if (situacion == null)
            {
                return HttpNotFound();
            }
            return View(situacion);
        }

        // POST: EstadoFuerza/Situacion/Edit/5
        [HttpPost]
        public ActionResult Edit(Situacion situacion)
        {
            if (ModelState.IsValid)
            {
                Situacion editSituacion = db.Situacion.SingleOrDefault(s => s.id_situacion == situacion.id_situacion && s.activo && !s.eliminado);
                if(editSituacion == null)
                {
                    return HttpNotFound();
                }
                editSituacion.nombre = situacion.nombre;
                editSituacion.es_pago = situacion.es_pago;
                editSituacion.baja_rrhh = situacion.baja_rrhh;
                editSituacion.fecha_modificacion = DateTime.Now;
                editSituacion.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Entry(editSituacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(situacion);
        }

        // GET: EstadoFuerza/Situacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Situacion situacion = db.Situacion.SingleOrDefault(s => s.id_situacion == id && s.activo && !s.eliminado);
            if (situacion == null)
            {
                return HttpNotFound();
            }
            return View(situacion);
        }

        // POST: EstadoFuerza/Situacion/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            Situacion situacion = db.Situacion.SingleOrDefault(s => s.id_situacion == id && s.activo && !s.eliminado);
            if(situacion == null)
            {
                return HttpNotFound();
            }
            situacion.activo = false;
            situacion.eliminado = true;
            situacion.fecha_eliminacion = DateTime.Now;
            situacion.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            db.Entry(situacion).State = EntityState.Modified;
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
