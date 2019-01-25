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
    public class Bodega_Inventario_ConsumiblesController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Bodega_Inventario_Consumibles
        public ActionResult Index()
        {
            ViewBag.id_bodega = db.Bodegas.Where(x => x.activo && !x.eliminado);
            var bodega_Inventario_Consumibles = db.Bodega_Inventario_Consumibles.Include(b => b.Usuarios).Include(b => b.Usuarios1).Include(b => b.Usuarios2).Include(b => b.Bodegas).Include(b => b.Consumibles).Where(b => b.activo && !b.eliminado);
            return View(bodega_Inventario_Consumibles.ToList());
        }

        // GET: Inventario/Bodega_Inventario_Consumibles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bodega_Inventario_Consumibles bodega_Inventario_Consumibles = db.Bodega_Inventario_Consumibles.Find(id);
            if (bodega_Inventario_Consumibles == null)
            {
                return HttpNotFound();
            }
            return View(bodega_Inventario_Consumibles);
        }

        // GET: Inventario/Bodega_Inventario_Consumibles/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion");
            ViewBag.id_consumible = new SelectList(db.Consumibles, "id_consumible", "descripcion");
            return View();
        }

        // POST: Inventario/Bodega_Inventario_Consumibles/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_bodega_inventario_consumibles,id_consumible,id_bodega,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Bodega_Inventario_Consumibles bodega_Inventario_Consumibles)
        {
            if (ModelState.IsValid)
            {
                db.Bodega_Inventario_Consumibles.Add(bodega_Inventario_Consumibles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Consumibles.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Consumibles.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Consumibles.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion", bodega_Inventario_Consumibles.id_bodega);
            ViewBag.id_consumible = new SelectList(db.Consumibles, "id_consumible", "descripcion", bodega_Inventario_Consumibles.id_consumible);
            return View(bodega_Inventario_Consumibles);
        }

        // GET: Inventario/Bodega_Inventario_Consumibles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bodega_Inventario_Consumibles bodega_Inventario_Consumibles = db.Bodega_Inventario_Consumibles.Find(id);
            if (bodega_Inventario_Consumibles == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Consumibles.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Consumibles.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Consumibles.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion", bodega_Inventario_Consumibles.id_bodega);
            ViewBag.id_consumible = new SelectList(db.Consumibles, "id_consumible", "descripcion", bodega_Inventario_Consumibles.id_consumible);
            return View(bodega_Inventario_Consumibles);
        }

        // POST: Inventario/Bodega_Inventario_Consumibles/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_bodega_inventario_consumibles,id_consumible,id_bodega,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Bodega_Inventario_Consumibles bodega_Inventario_Consumibles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bodega_Inventario_Consumibles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Consumibles.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Consumibles.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", bodega_Inventario_Consumibles.id_usuario_modificacion);
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion", bodega_Inventario_Consumibles.id_bodega);
            ViewBag.id_consumible = new SelectList(db.Consumibles, "id_consumible", "descripcion", bodega_Inventario_Consumibles.id_consumible);
            return View(bodega_Inventario_Consumibles);
        }

        // GET: Inventario/Bodega_Inventario_Consumibles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bodega_Inventario_Consumibles bodega_Inventario_Consumibles = db.Bodega_Inventario_Consumibles.Find(id);
            if (bodega_Inventario_Consumibles == null)
            {
                return HttpNotFound();
            }
            return View(bodega_Inventario_Consumibles);
        }

        // POST: Inventario/Bodega_Inventario_Consumibles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bodega_Inventario_Consumibles bodega_Inventario_Consumibles = db.Bodega_Inventario_Consumibles.Find(id);
            db.Bodega_Inventario_Consumibles.Remove(bodega_Inventario_Consumibles);
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
