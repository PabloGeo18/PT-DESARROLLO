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

namespace MVC2013.Areas.Administracion.Controllers
{
    public class MunicipiosController : Controller
    {
        private AppEntities db = new AppEntities(Constantes.getDataSource());

        // GET: Administracion/Municipios
        public ActionResult Index()
        {
            var municipios = db.Municipios.Include(m => m.Departamentos);
            return View(municipios.ToList());
        }

        // GET: Administracion/Municipios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Municipios municipios = db.Municipios.Find(id);
            if (municipios == null)
            {
                return HttpNotFound();
            }
            return View(municipios);
        }

        // GET: Administracion/Municipios/Create
        public ActionResult Create()
        {
            ViewBag.id_departamento = new SelectList(db.Departamentos, "id_departamento", "nombre");
            return View();
        }

        // POST: Administracion/Municipios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_municipio,id_departamento,nombre,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Municipios municipios)
        {
            if (ModelState.IsValid)
            {
                db.Municipios.Add(municipios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_departamento = new SelectList(db.Departamentos, "id_departamento", "nombre", municipios.id_departamento);
            return View(municipios);
        }

        // GET: Administracion/Municipios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Municipios municipios = db.Municipios.Find(id);
            if (municipios == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_departamento = new SelectList(db.Departamentos, "id_departamento", "nombre", municipios.id_departamento);
            return View(municipios);
        }

        // POST: Administracion/Municipios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_municipio,id_departamento,nombre,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Municipios municipios)
        {
            if (ModelState.IsValid)
            {
                db.Entry(municipios).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_departamento = new SelectList(db.Departamentos, "id_departamento", "nombre", municipios.id_departamento);
            return View(municipios);
        }

        // GET: Administracion/Municipios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Municipios municipios = db.Municipios.Find(id);
            if (municipios == null)
            {
                return HttpNotFound();
            }
            return View(municipios);
        }

        // POST: Administracion/Municipios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Municipios municipios = db.Municipios.Find(id);
            db.Municipios.Remove(municipios);
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
