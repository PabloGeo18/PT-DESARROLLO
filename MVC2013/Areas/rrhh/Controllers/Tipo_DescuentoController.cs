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
    public class Tipo_DescuentoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Tipo_Descuento
        public ActionResult Index()
        {
            var tipo_Descuento = db.Tipo_Descuento.Where(t=>!t.eliminado).OrderBy(t=>t.nombre);
            return View("Index", tipo_Descuento.ToList());
        }

        // GET: rrhh/Tipo_Descuento/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: rrhh/Tipo_Descuento/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(string nombre)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Tipo_Descuento tipo_descuento = NuevoTipoDescuento();
                    tipo_descuento.nombre = nombre;
                    db.Tipo_Descuento.Add(tipo_descuento);
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

        public Tipo_Descuento NuevoTipoDescuento()
        {
            Tipo_Descuento tipo_descuento = new Tipo_Descuento();
            tipo_descuento.activo = true;
            tipo_descuento.eliminado = false;
            tipo_descuento.fecha_creacion = DateTime.Now;
            tipo_descuento.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            return tipo_descuento;
        }

        // GET: rrhh/Tipo_Descuento/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Descuento tipo_Descuento = db.Tipo_Descuento.Find(id);
            if (tipo_Descuento == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Descuento);
        }

        // POST: rrhh/Tipo_Descuento/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(int id_tipo_descuento, string nombre)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Tipo_Descuento tipo_descuento = db.Tipo_Descuento.Find(id_tipo_descuento);
                    tipo_descuento.nombre = nombre;
                    tipo_descuento.fecha_modificacion = DateTime.Now;
                    tipo_descuento.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(tipo_descuento).State = EntityState.Modified;
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
                    Tipo_Descuento tipo_descuento = db.Tipo_Descuento.Find(id);
                    tipo_descuento.fecha_eliminacion = DateTime.Now;
                    tipo_descuento.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    tipo_descuento.activo = false;
                    tipo_descuento.eliminado = true;
                    db.Entry(tipo_descuento).State = EntityState.Modified;
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
