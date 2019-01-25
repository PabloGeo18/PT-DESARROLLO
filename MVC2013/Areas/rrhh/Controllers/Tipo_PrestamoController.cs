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
    public class Tipo_PrestamoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Tipo_Prestamo
        public ActionResult Index()
        {
            var tipo_Prestamo = db.Tipo_Prestamo.Where(t => !t.eliminado).OrderBy(t => t.nombre);
            return View(tipo_Prestamo.ToList());
        }

        // GET: rrhh/Tipo_Prestamo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Prestamo tipo_Prestamo = db.Tipo_Prestamo.Find(id);
            if (tipo_Prestamo == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Prestamo);
        }

        // GET: rrhh/Tipo_Prestamo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: rrhh/Tipo_Prestamo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "id_tipo_prestamo,nombre,activo,eliminado,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion")] Tipo_Prestamo tipo_Prestamo)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    tipo_Prestamo.activo = true;
                    tipo_Prestamo.eliminado = false;
                    tipo_Prestamo.fecha_creacion = DateTime.Now;
                    tipo_Prestamo.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Tipo_Prestamo.Add(tipo_Prestamo);
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    tran.Rollback();
                }
            }
            return View(tipo_Prestamo);
        }

        // GET: rrhh/Tipo_Prestamo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Prestamo tipo_Prestamo = db.Tipo_Prestamo.Find(id);
            if (tipo_Prestamo == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Prestamo);
        }

        // POST: rrhh/Tipo_Prestamo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id_tipo_prestamo,nombre,activo,eliminado,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion")] Tipo_Prestamo tipo_Prestamo)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Tipo_Prestamo tp = db.Tipo_Prestamo.Find(tipo_Prestamo.id_tipo_prestamo);
                    tp.nombre = tipo_Prestamo.nombre;
                    tp.fecha_modificacion = DateTime.Now;
                    tp.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(tp).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    tran.Rollback();
                }
            }
            return View(tipo_Prestamo);
        }

        // GET: rrhh/Tipo_Prestamo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Prestamo tipo_Prestamo = db.Tipo_Prestamo.Find(id);
            if (tipo_Prestamo == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Prestamo);
        }

        // POST: rrhh/Tipo_Prestamo/Delete/5
        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            using(DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Tipo_Prestamo tipo_prestamo = db.Tipo_Prestamo.Find(id);
                    tipo_prestamo.fecha_eliminacion = DateTime.Now;
                    tipo_prestamo.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    tipo_prestamo.activo = false;
                    tipo_prestamo.eliminado = true;
                    db.Entry(tipo_prestamo).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return Json(new { msg = "", response = true });
                }
                catch
                {
                    tran.Rollback();
                    return Json(new { msg = "Cambios no guardados.", response = false });
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
