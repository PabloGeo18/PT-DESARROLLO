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

namespace MVC2013.Areas.rrhh.Controllers
{
    public class Tipo_Permiso_AusenciaController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Tipo_Permiso_Ausencia
        public ActionResult Index()
        {
            var tipo_Permiso_Ausencia = db.Tipo_Permiso_Ausencia.Where(t => t.activo);
            return View(tipo_Permiso_Ausencia);
        }

        // GET: rrhh/Tipo_Permiso_Ausencia/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Permiso_Ausencia tipo_Permiso_Ausencia = db.Tipo_Permiso_Ausencia.SingleOrDefault(t => t.activo && t.id_tipo_permiso_ausencia == id);
            if (tipo_Permiso_Ausencia == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Permiso_Ausencia);
        }

        // GET: rrhh/Tipo_Permiso_Ausencia/Create
        public ActionResult Create()
        {
            ViewBag.situacion = new SelectList(db.Situacion.Where(s => s.activo), "id_situacion", "nombre");
            return View();
        }

        // POST: rrhh/Tipo_Permiso_Ausencia/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "id_tipo_permiso_ausencia,descripcion,id_situacion,activo,eliminado,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion")] Tipo_Permiso_Ausencia tipo_Permiso_Ausencia)
        {
            if (ModelState.IsValid)
            {
                tipo_Permiso_Ausencia.activo = true;
                tipo_Permiso_Ausencia.eliminado = false;
                tipo_Permiso_Ausencia.fecha_creacion = DateTime.Now;
                tipo_Permiso_Ausencia.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Tipo_Permiso_Ausencia.Add(tipo_Permiso_Ausencia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_situacion = new SelectList(db.Situacion.Where(s=>s.activo), "id_situacion", "nombre", tipo_Permiso_Ausencia.id_situacion);
            return View(tipo_Permiso_Ausencia);
        }

        // GET: rrhh/Tipo_Permiso_Ausencia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Permiso_Ausencia tipo_Permiso_Ausencia = db.Tipo_Permiso_Ausencia.SingleOrDefault(t => t.activo && t.id_tipo_permiso_ausencia == id);
            if (tipo_Permiso_Ausencia == null)
            {
                return HttpNotFound();
            }
            ViewBag.situacion = new SelectList(db.Situacion, "id_situacion", "nombre", tipo_Permiso_Ausencia.id_situacion);
            return View(tipo_Permiso_Ausencia);
        }

        // POST: rrhh/Tipo_Permiso_Ausencia/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id_tipo_permiso_ausencia,descripcion,id_situacion,activo,eliminado,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion")] Tipo_Permiso_Ausencia tipo_Permiso_Ausencia)
        {
            if (ModelState.IsValid)
            {
                Tipo_Permiso_Ausencia edit_tipo_permiso_ausencia = db.Tipo_Permiso_Ausencia.SingleOrDefault(t => t.activo && t.id_tipo_permiso_ausencia == tipo_Permiso_Ausencia.id_tipo_permiso_ausencia);
                edit_tipo_permiso_ausencia.id_situacion = tipo_Permiso_Ausencia.id_situacion;
                edit_tipo_permiso_ausencia.descripcion = tipo_Permiso_Ausencia.descripcion;
                edit_tipo_permiso_ausencia.fecha_modificacion = DateTime.Now;
                edit_tipo_permiso_ausencia.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Entry(edit_tipo_permiso_ausencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.situacion = new SelectList(db.Situacion, "id_situacion", "nombre", tipo_Permiso_Ausencia.id_situacion);
            return View(tipo_Permiso_Ausencia);
        }

        // GET: rrhh/Tipo_Permiso_Ausencia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Permiso_Ausencia tipo_Permiso_Ausencia = db.Tipo_Permiso_Ausencia.SingleOrDefault(t => t.activo && t.id_tipo_permiso_ausencia == id);
            if (tipo_Permiso_Ausencia == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Permiso_Ausencia);
        }

        // POST: rrhh/Tipo_Permiso_Ausencia/Delete/5
        [HttpPost]
        public ActionResult Eliminar(int id_tipo_permiso_ausencia)
        {
            Tipo_Permiso_Ausencia tipo_Permiso_Ausencia = db.Tipo_Permiso_Ausencia.SingleOrDefault(t => t.activo && t.id_tipo_permiso_ausencia == id_tipo_permiso_ausencia);
            if (tipo_Permiso_Ausencia == null)
            {
                return HttpNotFound();
            }
            tipo_Permiso_Ausencia.activo = false;
            tipo_Permiso_Ausencia.eliminado = true;
            tipo_Permiso_Ausencia.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            tipo_Permiso_Ausencia.fecha_eliminacion = DateTime.Now;
            db.Entry(tipo_Permiso_Ausencia).State = EntityState.Modified;
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
