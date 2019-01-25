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
    public class Empleado_Permisos_AusenciasController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Empleado_Permisos_Ausencias
        public ActionResult Index()
        {
            var empleado_Permisos_Ausencias = db.Empleado_Permisos_Ausencias.Where(e => e.activo).OrderByDescending(e => e.fecha);
            return View(empleado_Permisos_Ausencias);
        }

        // GET: rrhh/Empleado_Permisos_Ausencias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado_Permisos_Ausencias empleado_Permisos_Ausencias = db.Empleado_Permisos_Ausencias.SingleOrDefault(e => e.activo && e.id_empleado_permiso_ausencia == id);
            if (empleado_Permisos_Ausencias == null)
            {
                return HttpNotFound();
            }
            return View(empleado_Permisos_Ausencias);
        }

        // GET: rrhh/Empleado_Permisos_Ausencias/Create
        public ActionResult Create()
        {
            ViewBag.tipo_permiso_ausencia = new SelectList(db.Tipo_Permiso_Ausencia.Where(e => e.activo), "id_tipo_permiso_ausencia", "descripcion");
            ViewBag.fecha = "";
            return View();
        }

        // POST: rrhh/Empleado_Permisos_Ausencias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "id_empleado_permiso_ausencia,id_empleado,id_tipo_permiso_ausencia,fecha,dias,activo,eliminado,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion")] Empleado_Permisos_Ausencias empleado_Permisos_Ausencias)
        {
            if (ModelState.IsValid)
            {
                empleado_Permisos_Ausencias.activo = true;
                empleado_Permisos_Ausencias.eliminado = false;
                empleado_Permisos_Ausencias.fecha_creacion = DateTime.Now;
                empleado_Permisos_Ausencias.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Empleado_Permisos_Ausencias.Add(empleado_Permisos_Ausencias);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.fecha = empleado_Permisos_Ausencias.fecha.ToString("dd/MM/yyyy");
            ViewBag.tipo_permiso_ausencia = new SelectList(db.Tipo_Permiso_Ausencia, "id_tipo_permiso_ausencia", "descripcion", empleado_Permisos_Ausencias.id_tipo_permiso_ausencia);
            return View(empleado_Permisos_Ausencias);
        }

        // GET: rrhh/Empleado_Permisos_Ausencias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado_Permisos_Ausencias empleado_Permisos_Ausencias = db.Empleado_Permisos_Ausencias.SingleOrDefault(e => e.activo && e.id_empleado_permiso_ausencia == id);
            if (empleado_Permisos_Ausencias == null)
            {
                return HttpNotFound();
            }
            ViewBag.fecha = empleado_Permisos_Ausencias.fecha.ToString("dd/MM/yyyy");
            ViewBag.tipo_permiso_ausencia = new SelectList(db.Tipo_Permiso_Ausencia, "id_tipo_permiso_ausencia", "descripcion", empleado_Permisos_Ausencias.id_tipo_permiso_ausencia);
            return View(empleado_Permisos_Ausencias);
        }

        // POST: rrhh/Empleado_Permisos_Ausencias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id_empleado_permiso_ausencia,id_empleado,id_tipo_permiso_ausencia,fecha,dias,activo,eliminado,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion")] Empleado_Permisos_Ausencias empleado_Permisos_Ausencias)
        {
            if (ModelState.IsValid)
            {
                Empleado_Permisos_Ausencias epa = db.Empleado_Permisos_Ausencias.SingleOrDefault(e => e.activo && e.id_empleado_permiso_ausencia == empleado_Permisos_Ausencias.id_empleado_permiso_ausencia);
                if (epa == null)
                {
                    return HttpNotFound();
                }
                epa.fecha_modificacion = DateTime.Now;
                epa.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                epa.id_tipo_permiso_ausencia = empleado_Permisos_Ausencias.id_tipo_permiso_ausencia;
                epa.dias = empleado_Permisos_Ausencias.dias;
                epa.fecha = empleado_Permisos_Ausencias.fecha;
                db.Entry(epa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.fecha = empleado_Permisos_Ausencias.fecha.ToString("dd/MM/yyyy");
            ViewBag.tipo_permiso_ausencia = new SelectList(db.Tipo_Permiso_Ausencia, "id_tipo_permiso_ausencia", "descripcion", empleado_Permisos_Ausencias.id_tipo_permiso_ausencia);
            return View(empleado_Permisos_Ausencias);
        }

        // GET: rrhh/Empleado_Permisos_Ausencias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Empleado_Permisos_Ausencias empleado_Permisos_Ausencias = db.Empleado_Permisos_Ausencias.SingleOrDefault(e => e.activo && e.id_empleado_permiso_ausencia == id);
            if (empleado_Permisos_Ausencias == null)
            {
                return HttpNotFound();
            }
            return View(empleado_Permisos_Ausencias);
        }

        // POST: rrhh/Empleado_Permisos_Ausencias/Delete/5
        [HttpPost]
        public ActionResult Eliminar(int id_empleado_permiso_ausencia)
        {
            Empleado_Permisos_Ausencias empleado_Permisos_Ausencias = db.Empleado_Permisos_Ausencias.SingleOrDefault(e => e.activo && e.id_empleado_permiso_ausencia == id_empleado_permiso_ausencia);
            if (empleado_Permisos_Ausencias == null)
            {
                return HttpNotFound();
            }
            empleado_Permisos_Ausencias.activo = false;
            empleado_Permisos_Ausencias.eliminado = true;
            empleado_Permisos_Ausencias.fecha_eliminacion = DateTime.Now;
            empleado_Permisos_Ausencias.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            db.Entry(empleado_Permisos_Ausencias).State = EntityState.Modified;
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
