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
    public class UniformesController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Uniformes
        public ActionResult Index()
        {
            var uniformes = db.Uniformes.Include(u => u.Usuarios).Include(u => u.Usuarios1).Include(u => u.Usuarios2).Include(u => u.Bodega_Inventario_Uniformes).Include(u => u.Uniforme_Tipo);
            return View(uniformes.ToList());
        }

        // GET: Inventario/Uniformes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Uniformes uniformes = db.Uniformes.Find(id);
            if (uniformes == null)
            {
                return HttpNotFound();
            }
            return View(uniformes);
        }

        // GET: Inventario/Uniformes/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_uniforme = new SelectList(db.Bodega_Inventario_Uniformes, "id_uniforme", "id_uniforme");
            ViewBag.id_uniforme_tipo = new SelectList(db.Uniforme_Tipo, "id_uniforme_tipo", "descripcion");
            return View();
        }

        // POST: Inventario/Uniformes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_uniforme,id_uniforme_tipo,descripcion,talla,costo,costo_venta,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Uniformes uniformes)
        {
            if (ModelState.IsValid)
            {
                db.Uniformes.Add(uniformes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", uniformes.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", uniformes.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", uniformes.id_usuario_modificacion);
            ViewBag.id_uniforme = new SelectList(db.Bodega_Inventario_Uniformes, "id_uniforme", "id_uniforme", uniformes.id_uniforme);
            ViewBag.id_uniforme_tipo = new SelectList(db.Uniforme_Tipo, "id_uniforme_tipo", "descripcion", uniformes.id_uniforme_tipo);
            return View(uniformes);
        }

        // GET: Inventario/Uniformes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Uniformes uniformes = db.Uniformes.Find(id);
            if (uniformes == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", uniformes.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", uniformes.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", uniformes.id_usuario_modificacion);
            ViewBag.id_uniforme = new SelectList(db.Bodega_Inventario_Uniformes, "id_uniforme", "id_uniforme", uniformes.id_uniforme);
            ViewBag.id_uniforme_tipo = new SelectList(db.Uniforme_Tipo, "id_uniforme_tipo", "descripcion", uniformes.id_uniforme_tipo);
            return View(uniformes);
        }

        // POST: Inventario/Uniformes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_uniforme,id_uniforme_tipo,descripcion,talla,costo,costo_venta,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Uniformes uniformes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(uniformes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", uniformes.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", uniformes.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", uniformes.id_usuario_modificacion);
            ViewBag.id_uniforme = new SelectList(db.Bodega_Inventario_Uniformes, "id_uniforme", "id_uniforme", uniformes.id_uniforme);
            ViewBag.id_uniforme_tipo = new SelectList(db.Uniforme_Tipo, "id_uniforme_tipo", "descripcion", uniformes.id_uniforme_tipo);
            return View(uniformes);
        }

        // GET: Inventario/Uniformes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Uniformes uniformes = db.Uniformes.Find(id);
            if (uniformes == null)
            {
                return HttpNotFound();
            }
            return View(uniformes);
        }

        // POST: Inventario/Uniformes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Uniformes uniformes = db.Uniformes.Find(id);
            db.Uniformes.Remove(uniformes);
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
