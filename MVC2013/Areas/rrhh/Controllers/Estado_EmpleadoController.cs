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
    public class Estado_EmpleadoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Planilla/Estado_Empleado
        public ActionResult Index()
        {
            return View(db.Estado_Empleado.Where(e=>!e.eliminado).OrderBy(e=>e.nombre).ToList());
        }

        // GET: Planilla/Estado_Empleado/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado_Empleado estado_Empleado = db.Estado_Empleado.Find(id);
            if (estado_Empleado == null)
            {
                return HttpNotFound();
            }
            return View(estado_Empleado);
        }

        // GET: Planilla/Estado_Empleado/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Planilla/Estado_Empleado/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "id_estado_empleado,nombre")] Estado_Empleado estado_Empleado)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    estado_Empleado.fecha_creacion = DateTime.Now;
                    estado_Empleado.activo = true;
                    estado_Empleado.eliminado = false;
                    estado_Empleado.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Estado_Empleado.Add(estado_Empleado);
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    tran.Rollback();
                    return View(estado_Empleado);
                }
            }
        }

        // GET: Planilla/Estado_Empleado/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado_Empleado estado_Empleado = db.Estado_Empleado.Find(id);
            if (estado_Empleado == null)
            {
                return HttpNotFound();
            }
            return View(estado_Empleado);
        }

        // POST: Planilla/Estado_Empleado/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id_estado_empleado,nombre")] Estado_Empleado estado_Empleado)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Estado_Empleado edit_estado_empleado = db.Estado_Empleado.Find(estado_Empleado.id_estado_empleado);
                    edit_estado_empleado.nombre = estado_Empleado.nombre;
                    edit_estado_empleado.fecha_modificacion = DateTime.Now;
                    edit_estado_empleado.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(edit_estado_empleado).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    tran.Rollback();
                    return View(estado_Empleado);
                }
            }
        }

        // GET: Planilla/Estado_Empleado/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estado_Empleado estado_Empleado = db.Estado_Empleado.Find(id);
            if (estado_Empleado == null)
            {
                return HttpNotFound();
            }
            return View(estado_Empleado);
        }

        // POST: Planilla/Estado_Empleado/Delete/5
        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Estado_Empleado estado_empleado = db.Estado_Empleado.Find(id);
                    estado_empleado.fecha_eliminacion = DateTime.Now;
                    estado_empleado.activo = false;
                    estado_empleado.eliminado = true;
                    estado_empleado.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(estado_empleado).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return Json(new { response = true });
                }
                catch
                {
                    tran.Rollback();
                    return Json(new { response = false, msg = "Cambios no guardados" });
                }
            }
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
