using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;

namespace MVC2013.Areas.Inventario.Controllers
{
    public class Bodega_Inventario_MunicionesController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Bodega_Inventario_Municiones
        public ActionResult Index()
        {
            ViewBag.id_bodega = db.Bodegas.Where(x => x.activo && !x.eliminado);
            var bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Include(b => b.Usuarios).Include(b => b.Usuarios1).Include(b => b.Usuarios2).Include(b => b.Bodegas).Include(b => b.Municiones).Where(b => b.activo && !b.eliminado);
            return View(bodega_Inventario_Municiones.ToList());
        }

        // GET: Inventario/Bodega_Inventario_Municiones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bodega_Inventario_Municiones bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Find(id);
            if (bodega_Inventario_Municiones == null)
            {
                return HttpNotFound();
            }
            return View(bodega_Inventario_Municiones);
        }

        // GET: Inventario/Bodega_Inventario_Municiones/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion");
            ViewBag.id_municion = new SelectList(db.Municiones, "id_municion", "descripcion");
            return View();
        }

        // POST: Inventario/Bodega_Inventario_Municiones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_bodega_inventario_municiones,id_municion,id_bodega,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Bodega_Inventario_Municiones bodega_Inventario_Municiones)
        {
            if (ModelState.IsValid)
            {
                db.Bodega_Inventario_Municiones.Add(bodega_Inventario_Municiones);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Municiones.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Municiones.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Municiones.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion", bodega_Inventario_Municiones.id_bodega);
            ViewBag.id_municion = new SelectList(db.Municiones, "id_municion", "descripcion", bodega_Inventario_Municiones.id_municion);
            return View(bodega_Inventario_Municiones);
        }

        // GET: Inventario/Bodega_Inventario_Municiones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bodega_Inventario_Municiones bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Find(id);
            if (bodega_Inventario_Municiones == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Municiones.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Municiones.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Municiones.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion", bodega_Inventario_Municiones.id_bodega);
            ViewBag.id_municion = new SelectList(db.Municiones, "id_municion", "descripcion", bodega_Inventario_Municiones.id_municion);
            return View(bodega_Inventario_Municiones);
        }

        // POST: Inventario/Bodega_Inventario_Municiones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_bodega_inventario_municiones,id_municion,id_bodega,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Bodega_Inventario_Municiones bodega_Inventario_Municiones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bodega_Inventario_Municiones).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Municiones.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Municiones.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Municiones.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion", bodega_Inventario_Municiones.id_bodega);
            ViewBag.id_municion = new SelectList(db.Municiones, "id_municion", "descripcion", bodega_Inventario_Municiones.id_municion);
            return View(bodega_Inventario_Municiones);
        }

        // GET: Inventario/Bodega_Inventario_Municiones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bodega_Inventario_Municiones bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Find(id);
            if (bodega_Inventario_Municiones == null)
            {
                return HttpNotFound();
            }
            return View(bodega_Inventario_Municiones);
        }

        // POST: Inventario/Bodega_Inventario_Municiones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bodega_Inventario_Municiones bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Find(id);
            db.Bodega_Inventario_Municiones.Remove(bodega_Inventario_Municiones);
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
