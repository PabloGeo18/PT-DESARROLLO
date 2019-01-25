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
using MVC2013.Src.Seguridad.To;

namespace MVC2013.Areas.rrhh.Controllers
{
    public class InstructorController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Instructor
        public ActionResult Index()
        {
            var instructor = db.Instructor.Where(e => !e.eliminado);
            return View(instructor.ToList());
        }

        // GET: rrhh/Instructor/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructor.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: rrhh/Instructor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: rrhh/Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_instructor,primer_nombre,segundo_nombre,primer_apellido,segundo_apellido,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,activo,eliminado")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        instructor.fecha_creacion = DateTime.Now;
                        instructor.activo = true;
                        instructor.eliminado = false;
                        instructor.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        db.Instructor.Add(instructor);
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
            return View(instructor);
        }

        // GET: rrhh/Instructor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructor.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: rrhh/Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_instructor,primer_nombre,segundo_nombre,primer_apellido,segundo_apellido,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,activo,eliminado")] Instructor instructor)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Instructor edit_instructor = db.Instructor.Find(instructor.id_instructor);
                        edit_instructor.primer_nombre = instructor.primer_nombre;
                        edit_instructor.segundo_nombre = instructor.segundo_nombre;
                        edit_instructor.primer_apellido = instructor.primer_apellido;
                        edit_instructor.segundo_apellido = instructor.segundo_apellido;
                        edit_instructor.fecha_modificacion = DateTime.Now;
                        edit_instructor.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                        db.Entry(edit_instructor).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Index");
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
            return View(instructor);
        }

        // GET: rrhh/Instructor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructor.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: rrhh/Instructor/Delete/5
        public ActionResult DeleteConfirmed(int id)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Instructor instructor = db.Instructor.Find(id);
                    instructor.fecha_eliminacion = DateTime.Now;
                    instructor.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    instructor.activo = false;
                    instructor.eliminado = true;
                    db.Entry(instructor).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                }
            }
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
