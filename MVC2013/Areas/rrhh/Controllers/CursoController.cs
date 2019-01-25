using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Src.Seguridad.To;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Areas.rrhh.Controllers
{
    public class CursoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Cursoes
        public ActionResult Index(int id_academia)
        {
            var curso = db.Curso.Where(e => !e.eliminado).OrderBy(e => e.nombre);
            ViewBag.id_academia = id_academia;
            return View(curso.ToList());
        }


        // GET: rrhh/Cursoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Curso curso = db.Curso.Find(id);
            if (curso == null)
            {
                return HttpNotFound();
            }
            return View(curso);
        }

        // GET: rrhh/Cursoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: rrhh/Cursoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_curso,nombre,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                db.Curso.Add(curso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(curso);
        }

        // GET: rrhh/Cursoes/Edit/5
        public ActionResult Edit(int? id, int id_academia)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Curso curso = db.Curso.Find(id);
            if (curso == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_academia = id_academia;
            return View(curso);
        }

        // POST: rrhh/Cursoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string nombre, int id_curso, int id_academia)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        UsuarioTO usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        Curso curso = db.Curso.Find(id_curso);
                        curso.fecha_modificacion = DateTime.Now;
                        curso.nombre = nombre;
                        curso.id_usuario_modificacion = usuario.usuario.id_usuario;
                        db.Entry(curso).State = EntityState.Modified;
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch 
                    {
                        tran.Rollback();
                        ModelState.AddModelError("", "Error. Cambios no realizados.");
                    }
                    
                }
            }
            return RedirectToAction("Index", new { id_academia = id_academia });
        }

        // GET: rrhh/Cursoes/Delete/5
        public ActionResult Delete(int? id, int id_academia)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Curso curso = db.Curso.Find(id);
            if (curso == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_academia = id_academia;
            return View(curso);
        }

        // POST: rrhh/Cursoes/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id_curso, int id_academia)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Curso curso = db.Curso.Find(id_curso);
                    curso.activo = false;
                    curso.eliminado = true;
                    UsuarioTO usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                    curso.id_usuario_eliminacion = usuario.usuario.id_usuario;
                    curso.fecha_eliminacion = DateTime.Now;
                    db.Entry(curso).State = EntityState.Modified;
                    db.SaveChanges();
                    var capacitacion_curso = db.Capacitacion_Curso.Where(e => !e.eliminado && e.id_curso == curso.id_curso);
                    foreach (var item in capacitacion_curso)
                    {
                        item.activo = false;
                        item.eliminado = true;
                        item.fecha_eliminacion = DateTime.Now;
                        item.id_usuario_eliminacion = usuario.usuario.id_usuario;
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    ModelState.AddModelError("", "Error. Cambios no realizados.");
                }
            }
            return RedirectToAction("Index", new { id_academia = id_academia });
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
