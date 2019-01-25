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
    public class Tipo_Plan_PagoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: rrhh/Tipo_Plan_Pago
        public ActionResult Index()
        {
            var tipo_Plan_Pago = db.Tipo_Plan_Pago.Where(t => !t.eliminado).OrderBy(t => t.nombre);
            return View(tipo_Plan_Pago.ToList());
        }

        // GET: rrhh/Tipo_Plan_Pago/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Plan_Pago tipo_Plan_Pago = db.Tipo_Plan_Pago.Find(id);
            if (tipo_Plan_Pago == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Plan_Pago);
        }

        // GET: rrhh/Tipo_Plan_Pago/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: rrhh/Tipo_Plan_Pago/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "id_tipo_plan_pago,nombre,activo,eliminado,fecha_creacion,fecha_modificacion,fecha_eliminaciion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion")] Tipo_Plan_Pago tipo_Plan_Pago)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    tipo_Plan_Pago.activo = true;
                    tipo_Plan_Pago.eliminado = false;
                    tipo_Plan_Pago.fecha_creacion = DateTime.Now;
                    tipo_Plan_Pago.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Tipo_Plan_Pago.Add(tipo_Plan_Pago);
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    tran.Rollback();
                }
            }
            return View(tipo_Plan_Pago);
        }

        // GET: rrhh/Tipo_Plan_Pago/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Plan_Pago tipo_Plan_Pago = db.Tipo_Plan_Pago.Find(id);
            if (tipo_Plan_Pago == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Plan_Pago);
        }

        // POST: rrhh/Tipo_Plan_Pago/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id_tipo_plan_pago,nombre,activo,eliminado,fecha_creacion,fecha_modificacion,fecha_eliminaciion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion")] Tipo_Plan_Pago tipo_Plan_Pago)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Tipo_Plan_Pago tpp = db.Tipo_Plan_Pago.Find(tipo_Plan_Pago.id_tipo_plan_pago);
                    tpp.nombre = tipo_Plan_Pago.nombre;
                    tpp.fecha_modificacion = DateTime.Now;
                    tpp.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    db.Entry(tpp).State = EntityState.Modified;
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    tran.Rollback();
                }
            }
            return View(tipo_Plan_Pago);
        }

        // GET: rrhh/Tipo_Plan_Pago/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Plan_Pago tipo_Plan_Pago = db.Tipo_Plan_Pago.Find(id);
            if (tipo_Plan_Pago == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Plan_Pago);
        }

        // POST: rrhh/Tipo_Plan_Pago/Delete/5
        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            using (DbContextTransaction tran = db.Database.BeginTransaction())
            {
                try
                {
                    Tipo_Plan_Pago tpp = db.Tipo_Plan_Pago.Find(id);
                    tpp.fecha_eliminacion = DateTime.Now;
                    tpp.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                    tpp.activo = false;
                    tpp.eliminado = true;
                    db.Entry(tpp).State = EntityState.Modified;
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
