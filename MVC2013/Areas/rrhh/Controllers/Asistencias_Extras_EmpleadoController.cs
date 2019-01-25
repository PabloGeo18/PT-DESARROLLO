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
    public class Asistencias_Extras_EmpleadoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Asistencias_Extras_Empleado
        public ActionResult Index()
        {
            var asistencias_Extras_Empleado = db.Asistencias_Extras_Empleado.Where(e=>e.activo).OrderByDescending(e=>e.fecha);
            return View(asistencias_Extras_Empleado);
        }

        // GET: rrhh/Asistencias_Extras_Empleado/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asistencias_Extras_Empleado asistencias_Extras_Empleado = db.Asistencias_Extras_Empleado.SingleOrDefault(e => e.activo && e.id_asistencias_extras_empleados == id);
            if (asistencias_Extras_Empleado == null)
            {
                return HttpNotFound();
            }
            return View(asistencias_Extras_Empleado);
        }

        // GET: rrhh/Asistencias_Extras_Empleado/Create
        public ActionResult Create()
        {
            ViewBag.fecha = "";
            return View();
        }

        // POST: rrhh/Asistencias_Extras_Empleado/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "id_asistencias_extras_empleados,id_empleado,dias,fecha,comentario,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Asistencias_Extras_Empleado asistencias_Extras_Empleado)
        {
            if (ModelState.IsValid)
            {
                asistencias_Extras_Empleado.activo = true;
                asistencias_Extras_Empleado.eliminado = false;
                asistencias_Extras_Empleado.fecha_creacion = DateTime.Now;
                asistencias_Extras_Empleado.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Asistencias_Extras_Empleado.Add(asistencias_Extras_Empleado);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.fecha = asistencias_Extras_Empleado.fecha.ToString("dd/MM/yyyy");
            return View(asistencias_Extras_Empleado);
        }

        // GET: rrhh/Asistencias_Extras_Empleado/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asistencias_Extras_Empleado asistencias_Extras_Empleado = db.Asistencias_Extras_Empleado.SingleOrDefault(e => e.activo && e.id_asistencias_extras_empleados == id);
            if (asistencias_Extras_Empleado == null)
            {
                return HttpNotFound();
            }
            ViewBag.fecha = asistencias_Extras_Empleado.fecha.ToString("dd/MM/yyyy");
            return View(asistencias_Extras_Empleado);
        }

        // POST: rrhh/Asistencias_Extras_Empleado/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id_asistencias_extras_empleados,id_empleado,dias,fecha,comentario,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Asistencias_Extras_Empleado asistencias_Extras_Empleado)
        {
            if (ModelState.IsValid)
            {
                Asistencias_Extras_Empleado aee = db.Asistencias_Extras_Empleado.SingleOrDefault(e => e.activo && e.id_asistencias_extras_empleados == asistencias_Extras_Empleado.id_asistencias_extras_empleados);
                if (aee == null)
                {
                    return HttpNotFound();
                }
                aee.fecha_modificacion = DateTime.Now;
                aee.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                aee.dias = asistencias_Extras_Empleado.dias;
                aee.fecha = asistencias_Extras_Empleado.fecha;
                db.Entry(aee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.fecha = asistencias_Extras_Empleado.fecha.ToString("dd/MM/yyyy");
            return View(asistencias_Extras_Empleado);
        }

        // GET: rrhh/Asistencias_Extras_Empleado/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asistencias_Extras_Empleado asistencias_Extras_Empleado = db.Asistencias_Extras_Empleado.SingleOrDefault(e => e.activo && e.id_asistencias_extras_empleados == id);
            if (asistencias_Extras_Empleado == null)
            {
                return HttpNotFound();
            }
            return View(asistencias_Extras_Empleado);
        }

        // POST: rrhh/Asistencias_Extras_Empleado/Delete/5
        [HttpPost]
        public ActionResult Eliminar(int id_asistencias_extras_empleados)
        {
            Asistencias_Extras_Empleado asistencias_Extras_Empleado = db.Asistencias_Extras_Empleado.SingleOrDefault(e => e.activo && e.id_asistencias_extras_empleados == id_asistencias_extras_empleados);
            if (asistencias_Extras_Empleado == null)
            {
                return HttpNotFound();
            }
            asistencias_Extras_Empleado.activo = false;
            asistencias_Extras_Empleado.eliminado = true;
            asistencias_Extras_Empleado.fecha_eliminacion = DateTime.Now;
            asistencias_Extras_Empleado.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            db.Entry(asistencias_Extras_Empleado).State = EntityState.Modified;
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
