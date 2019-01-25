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
    public class Grado_EstudioController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Grado_Estudio
        public ActionResult Index()
        {
            return View(db.Grado_Estudio.Where(g => !g.eliminado).OrderBy(g => g.nombre));
        }

        // GET: Administracion/Grado_Estudio/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grado_Estudio grado_Estudio = db.Grado_Estudio.Find(id);
            if (grado_Estudio == null)
            {
                return HttpNotFound();
            }
            return View(grado_Estudio);
        }

        // GET: Administracion/Grado_Estudio/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Grado_Estudio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_grado_estudio,nombre")] Grado_Estudio grado_Estudio)
        {
            if (ModelState.IsValid)
            {
                grado_Estudio.fecha_creacion = DateTime.Now;
                grado_Estudio.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                grado_Estudio.activo = true;
                grado_Estudio.eliminado = false;
                db.Grado_Estudio.Add(grado_Estudio);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(grado_Estudio);
        }

        // GET: Administracion/Grado_Estudio/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grado_Estudio grado_Estudio = db.Grado_Estudio.Find(id);
            if (grado_Estudio == null)
            {
                return HttpNotFound();
            }
            return View(grado_Estudio);
        }

        // POST: Administracion/Grado_Estudio/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_grado_estudio,nombre")] Grado_Estudio grado_Estudio)
        {
            if (ModelState.IsValid)
            {
                Grado_Estudio edit_grado_estudio = db.Grado_Estudio.Find(grado_Estudio.id_grado_estudio);
                edit_grado_estudio.fecha_modificacion = DateTime.Now;
                edit_grado_estudio.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                edit_grado_estudio.nombre = grado_Estudio.nombre;
                db.Entry(edit_grado_estudio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(grado_Estudio);
        }

        // GET: Administracion/Grado_Estudio/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grado_Estudio grado_Estudio = db.Grado_Estudio.Find(id);
            if (grado_Estudio == null)
            {
                return HttpNotFound();
            }
            return View(grado_Estudio);
        }

        // POST: Administracion/Grado_Estudio/Delete/5
        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Grado_Estudio grado_Estudio = db.Grado_Estudio.Find(id);
                    grado_Estudio.activo = false;
                    grado_Estudio.eliminado = true;
                    grado_Estudio.fecha_eliminacion = DateTime.Now;
                    grado_Estudio.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(grado_Estudio).State = EntityState.Modified;
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
