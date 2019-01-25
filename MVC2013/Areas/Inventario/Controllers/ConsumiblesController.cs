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
    public class ConsumiblesController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Consumibles
        public ActionResult Index()
        {
            var consumibles = db.Consumibles.Include(c => c.Usuarios).Include(c => c.Usuarios1).Include(c => c.Usuarios2).Include(c => c.Bodega_Inventario_Consumibles).Include(c => c.Consumible_Tipo);
            return View(consumibles.ToList());
        }

        // GET: Inventario/Consumibles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consumibles consumibles = db.Consumibles.Find(id);
            if (consumibles == null)
            {
                return HttpNotFound();
            }
            return View(consumibles);
        }

        // GET: Inventario/Consumibles/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_consumible = new SelectList(db.Bodega_Inventario_Consumibles, "id_consumible", "id_consumible");
            ViewBag.id_consumible_tipo = new SelectList(db.Consumible_Tipo, "id_consumible_tipo", "descripcion");
            return View();
        }

        // POST: Inventario/Consumibles/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_consumible,descripcion,costo,costo_venta,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_consumible_tipo")] Consumibles consumibles)
        {
            if (ModelState.IsValid)
            {
                db.Consumibles.Add(consumibles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", consumibles.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", consumibles.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", consumibles.id_usuario_modificacion);
            ViewBag.id_consumible = new SelectList(db.Bodega_Inventario_Consumibles, "id_consumible", "id_consumible", consumibles.id_consumible);
            ViewBag.id_consumible_tipo = new SelectList(db.Consumible_Tipo, "id_consumible_tipo", "descripcion", consumibles.id_consumible_tipo);
            return View(consumibles);
        }

        // GET: Inventario/Consumibles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consumibles consumibles = db.Consumibles.Find(id);
            if (consumibles == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", consumibles.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", consumibles.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", consumibles.id_usuario_modificacion);
            ViewBag.id_consumible = new SelectList(db.Bodega_Inventario_Consumibles, "id_consumible", "id_consumible", consumibles.id_consumible);
            ViewBag.id_consumible_tipo = new SelectList(db.Consumible_Tipo, "id_consumible_tipo", "descripcion", consumibles.id_consumible_tipo);
            return View(consumibles);
        }

        // POST: Inventario/Consumibles/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_consumible,descripcion,costo,costo_venta,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_consumible_tipo")] Consumibles consumibles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(consumibles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", consumibles.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", consumibles.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", consumibles.id_usuario_modificacion);
            ViewBag.id_consumible = new SelectList(db.Bodega_Inventario_Consumibles, "id_consumible", "id_consumible", consumibles.id_consumible);
            ViewBag.id_consumible_tipo = new SelectList(db.Consumible_Tipo, "id_consumible_tipo", "descripcion", consumibles.id_consumible_tipo);
            return View(consumibles);
        }

        // GET: Inventario/Consumibles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consumibles consumibles = db.Consumibles.Find(id);
            if (consumibles == null)
            {
                return HttpNotFound();
            }
            return View(consumibles);
        }

        // POST: Inventario/Consumibles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Consumibles consumibles = db.Consumibles.Find(id);
            db.Consumibles.Remove(consumibles);
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
