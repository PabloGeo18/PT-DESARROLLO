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
    public class Inventario_TipoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Inventario_Tipo
        public ActionResult Index()
        {
            return View(db.Inventario_Tipo.ToList());
        }

        // GET: Inventario/Inventario_Tipo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventario_Tipo inventario_Tipo = db.Inventario_Tipo.Find(id);
            if (inventario_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(inventario_Tipo);
        }

        // GET: Inventario/Inventario_Tipo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inventario/Inventario_Tipo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_inventario_tipo,descripcion")] Inventario_Tipo inventario_Tipo)
        {
            if (ModelState.IsValid)
            {
                db.Inventario_Tipo.Add(inventario_Tipo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(inventario_Tipo);
        }

        // GET: Inventario/Inventario_Tipo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventario_Tipo inventario_Tipo = db.Inventario_Tipo.Find(id);
            if (inventario_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(inventario_Tipo);
        }

        // POST: Inventario/Inventario_Tipo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_inventario_tipo,descripcion")] Inventario_Tipo inventario_Tipo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventario_Tipo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inventario_Tipo);
        }

        // GET: Inventario/Inventario_Tipo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventario_Tipo inventario_Tipo = db.Inventario_Tipo.Find(id);
            if (inventario_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(inventario_Tipo);
        }

        // POST: Inventario/Inventario_Tipo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inventario_Tipo inventario_Tipo = db.Inventario_Tipo.Find(id);
            db.Inventario_Tipo.Remove(inventario_Tipo);
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
