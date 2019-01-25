using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;

namespace MVC2013.Areas.Customers.Controllers
{
    public class Razones_SocialesController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Customers/Razones_Sociales
        public ActionResult Index()
        {
            var razones_Sociales = db.Razones_Sociales.Include(r => r.Clientes).Include(r => r.Municipios).Include(r => r.Municipios1).Include(r => r.Municipios2).Include(r => r.Usuarios).Include(r => r.Usuarios1).Include(r => r.Usuarios2);
            return View(razones_Sociales.ToList());
        }

        // GET: Customers/Razones_Sociales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Razones_Sociales razones_Sociales = db.Razones_Sociales.Find(id);
            if (razones_Sociales == null)
            {
                return HttpNotFound();
            }
            return View(razones_Sociales);
        }

        // GET: Customers/Razones_Sociales/Create
        public ActionResult Create()
        {
            ViewBag.id_cliente = new SelectList(db.Clientes, "id_cliente", "nombre");
            ViewBag.municipio_direccion_facturacion = new SelectList(db.Municipios, "id_municipio", "nombre");
            ViewBag.municipio_direccion_fiscal = new SelectList(db.Municipios, "id_municipio", "nombre");
            ViewBag.municipio_direccion_fisica = new SelectList(db.Municipios, "id_municipio", "nombre");
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            return View();
        }

        // POST: Customers/Razones_Sociales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_razon_social,id_cliente,razon_social,nombre_comercial,nit,direccion_fisica,municipio_direccion_fisica,direccion_fiscal,municipio_direccion_fiscal,direccion_facturacion,municipio_direccion_facturacion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Razones_Sociales razones_Sociales)
        {
            if (ModelState.IsValid)
            {
                db.Razones_Sociales.Add(razones_Sociales);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_cliente = new SelectList(db.Clientes, "id_cliente", "nombre", razones_Sociales.id_cliente);
            ViewBag.municipio_direccion_facturacion = new SelectList(db.Municipios, "id_municipio", "nombre", razones_Sociales.municipio_direccion_facturacion);
            ViewBag.municipio_direccion_fiscal = new SelectList(db.Municipios, "id_municipio", "nombre", razones_Sociales.municipio_direccion_fiscal);
            ViewBag.municipio_direccion_fisica = new SelectList(db.Municipios, "id_municipio", "nombre", razones_Sociales.municipio_direccion_fisica);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", razones_Sociales.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", razones_Sociales.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", razones_Sociales.id_usuario_modificacion);
            return View(razones_Sociales);
        }

        // GET: Customers/Razones_Sociales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Razones_Sociales razones_Sociales = db.Razones_Sociales.Find(id);
            if (razones_Sociales == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cliente = new SelectList(db.Clientes, "id_cliente", "nombre", razones_Sociales.id_cliente);
            ViewBag.municipio_direccion_facturacion = new SelectList(db.Municipios, "id_municipio", "nombre", razones_Sociales.municipio_direccion_facturacion);
            ViewBag.municipio_direccion_fiscal = new SelectList(db.Municipios, "id_municipio", "nombre", razones_Sociales.municipio_direccion_fiscal);
            ViewBag.municipio_direccion_fisica = new SelectList(db.Municipios, "id_municipio", "nombre", razones_Sociales.municipio_direccion_fisica);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", razones_Sociales.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", razones_Sociales.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", razones_Sociales.id_usuario_modificacion);
            return View(razones_Sociales);
        }

        // POST: Customers/Razones_Sociales/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_razon_social,id_cliente,razon_social,nombre_comercial,nit,direccion_fisica,municipio_direccion_fisica,direccion_fiscal,municipio_direccion_fiscal,direccion_facturacion,municipio_direccion_facturacion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Razones_Sociales razones_Sociales)
        {
            if (ModelState.IsValid)
            {
                db.Entry(razones_Sociales).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_cliente = new SelectList(db.Clientes, "id_cliente", "nombre", razones_Sociales.id_cliente);
            ViewBag.municipio_direccion_facturacion = new SelectList(db.Municipios, "id_municipio", "nombre", razones_Sociales.municipio_direccion_facturacion);
            ViewBag.municipio_direccion_fiscal = new SelectList(db.Municipios, "id_municipio", "nombre", razones_Sociales.municipio_direccion_fiscal);
            ViewBag.municipio_direccion_fisica = new SelectList(db.Municipios, "id_municipio", "nombre", razones_Sociales.municipio_direccion_fisica);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", razones_Sociales.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", razones_Sociales.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", razones_Sociales.id_usuario_modificacion);
            return View(razones_Sociales);
        }

        // GET: Customers/Razones_Sociales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Razones_Sociales razones_Sociales = db.Razones_Sociales.Find(id);
            if (razones_Sociales == null)
            {
                return HttpNotFound();
            }
            return View(razones_Sociales);
        }

        // POST: Customers/Razones_Sociales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Razones_Sociales razones_Sociales = db.Razones_Sociales.Find(id);
            db.Razones_Sociales.Remove(razones_Sociales);
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
