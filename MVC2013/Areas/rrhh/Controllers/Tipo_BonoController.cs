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
    public class Tipo_BonoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Tipo_Bono
        public ActionResult Index()
        {
            var tipo_Bono = db.Tipo_Bono.Where(t=>!t.eliminado).OrderBy(t=>t.nombre);
            return View("Index", tipo_Bono.ToList());
        }

        // GET: rrhh/Tipo_Bono/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: rrhh/Tipo_Bono/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(string nombre)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Tipo_Bono tipo_bono = NuevoTipoBono();
                    tipo_bono.nombre = nombre;
                    db.Tipo_Bono.Add(tipo_bono);
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

        public Tipo_Bono NuevoTipoBono()
        {
            Tipo_Bono tipo_bono = new Tipo_Bono();
            tipo_bono.activo = true;
            tipo_bono.eliminado = false;
            tipo_bono.fecha_creacion = DateTime.Now;
            tipo_bono.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return tipo_bono;
        }

        // GET: rrhh/Tipo_Bono/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Bono tipo_Bono = db.Tipo_Bono.Find(id);
            if (tipo_Bono == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Bono);
        }

        // POST: rrhh/Tipo_Bono/Edit/5
        [HttpPost]
        public ActionResult Edit(int id_tipo_bono, string nombre)
        {
            using(DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Tipo_Bono tipo_Bono = db.Tipo_Bono.Find(id_tipo_bono);
                    tipo_Bono.nombre = nombre;
                    tipo_Bono.fecha_modificacion = DateTime.Now;
                    tipo_Bono.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(tipo_Bono).State = EntityState.Modified;
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

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Tipo_Bono tipo_Bono = db.Tipo_Bono.Find(id);
                    tipo_Bono.fecha_eliminacion = DateTime.Now;
                    tipo_Bono.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    tipo_Bono.activo = false;
                    tipo_Bono.eliminado = true;
                    db.Entry(tipo_Bono).State = EntityState.Modified;
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
