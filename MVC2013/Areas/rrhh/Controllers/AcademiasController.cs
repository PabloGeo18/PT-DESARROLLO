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
    public class AcademiasController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Planilla/Academias
        public ActionResult Index()
        {
            var academia = db.Academia.Where(e => !e.eliminado);
            return View(academia.ToList());
        }

        // GET: Planilla/Academias/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Academia academia = db.Academia.SingleOrDefault(a=>a.id_academia == id && a.activo);
            if (academia == null)
            {
                return HttpNotFound();
            }
            return View(academia);
        }

        // GET: Planilla/Academias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Planilla/Academias/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_academia,nombre,ubicacion,activo,eliminado,id_usuario_crecion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Academia academia)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        UsuarioTO usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        academia.nombre = ConversionMayusculas(academia.nombre);
                        academia.ubicacion = ConversionMayusculas(academia.ubicacion);
                        academia.id_usuario_crecion = usuario.usuario.id_usuario;
                        academia.fecha_creacion = DateTime.Now;
                        academia.activo = true;
                        academia.eliminado = false;
                        db.Academia.Add(academia);
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
            return View(academia);
        }

        // GET: Planilla/Academias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Academia academia = db.Academia.SingleOrDefault(a=>a.id_academia == id && a.activo);
            if (academia == null)
            {
                return HttpNotFound();
            }
            return View(academia);
        }

        // POST: Planilla/Academias/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_academia,nombre,ubicacion,activo,eliminado,id_usuario_crecion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Academia academia)
        {
            if (ModelState.IsValid)
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        Academia academiaEdit = db.Academia.SingleOrDefault(a => a.activo && a.id_academia == academia.id_academia);
                        if (academiaEdit == null)
                        {
                            return HttpNotFound();
                        }
                        academiaEdit.nombre = ConversionMayusculas(academia.nombre);
                        academiaEdit.ubicacion = ConversionMayusculas(academia.ubicacion);
                        UsuarioTO usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                        academiaEdit.fecha_modificacion = DateTime.Now;
                        academiaEdit.id_usuario_modificacion = usuario.usuario.id_usuario;
                        db.Entry(academiaEdit).State = EntityState.Modified;
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
            return View(academia);
        }

        // GET: Planilla/Academias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Academia academia = db.Academia.SingleOrDefault(a=>a.id_academia == id && a.activo);
            if (academia == null)
            {
                return HttpNotFound();
            }
            return View(academia);
        }

        // POST: Planilla/Academias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Academia academia = db.Academia.SingleOrDefault(a => a.activo && a.id_academia == id);
                    if (academia == null)
                    {
                        return HttpNotFound();
                    }
                    UsuarioTO usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                    academia.id_usuario_eliminacion = usuario.usuario.id_usuario;
                    academia.fecha_eliminacion = DateTime.Now;
                    academia.activo = false;
                    academia.eliminado = true;
                    db.Entry(academia).State = EntityState.Modified;
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

        private string ConversionMayusculas(string cadena)
        {
            if(!String.IsNullOrEmpty(cadena))
            {
                return cadena.ToUpper();
            }
            return null;
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
