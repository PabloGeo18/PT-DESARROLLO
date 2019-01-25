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
    public class ArticulosController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Articulos
        public ActionResult Index()
        {
            ViewBag.id_bodega = db.Bodegas.Where(x => x.activo && !x.eliminado);
            var articulos = db.Articulos.Include(a => a.Clientes).Include(a => a.Usuarios).Include(a => a.Usuarios1).Include(a => a.Usuarios2).Include(a => a.Articulo_Tipo).Include(a => a.Bodegas).Include(a => a.Marcas).Include(a => a.Proveedores).Where(a => a.activo).Where(a => a.eliminado == false);
            return View(articulos.ToList());
        }

        // GET: Inventario/Articulos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articulos articulos = db.Articulos.Find(id);
            if (articulos == null)
            {
                return HttpNotFound();
            }
            return View(articulos);
        }

        // GET: Inventario/Articulos/Create
        public ActionResult Create()
        {
            ViewBag.id_cliente = new SelectList(db.Clientes, "id_cliente", "nombre");
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_articulo_tipo = new SelectList(db.Articulo_Tipo, "id_articulo_tipo", "descripcion");
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion");
            ViewBag.id_marca = new SelectList(db.Marcas, "id_marca", "descripcion");
            ViewBag.id_proveedor = new SelectList(db.Proveedores, "id_proveedor", "descripcion");
            return View();
        }

        // POST: Inventario/Articulos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_articulo,id_articulo_tipo,serie,id_marca,id_proveedor,costo,costo_venta,id_cliente,id_bodega,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Articulos articulos)
        {
            if (ModelState.IsValid)
            {
                db.Articulos.Add(articulos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_cliente = new SelectList(db.Clientes, "id_cliente", "nombre", articulos.id_cliente);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", articulos.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", articulos.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", articulos.id_usuario_modificacion);
            ViewBag.id_articulo_tipo = new SelectList(db.Articulo_Tipo, "id_articulo_tipo", "descripcion", articulos.id_articulo_tipo);
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion", articulos.id_bodega);
            ViewBag.id_marca = new SelectList(db.Marcas, "id_marca", "descripcion", articulos.id_marca);
            ViewBag.id_proveedor = new SelectList(db.Proveedores, "id_proveedor", "descripcion", articulos.id_proveedor);
            return View(articulos);
        }

        // GET: Inventario/Articulos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articulos articulos = db.Articulos.Find(id);
            if (articulos == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_cliente = new SelectList(db.Clientes, "id_cliente", "nombre", articulos.id_cliente);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", articulos.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", articulos.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", articulos.id_usuario_modificacion);
            ViewBag.id_articulo_tipo = new SelectList(db.Articulo_Tipo, "id_articulo_tipo", "descripcion", articulos.id_articulo_tipo);
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion", articulos.id_bodega);
            ViewBag.id_marca = new SelectList(db.Marcas, "id_marca", "descripcion", articulos.id_marca);
            ViewBag.id_proveedor = new SelectList(db.Proveedores, "id_proveedor", "descripcion", articulos.id_proveedor);
            return View(articulos);
        }

        // POST: Inventario/Articulos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_articulo,id_articulo_tipo,serie,id_marca,id_proveedor,costo,costo_venta,id_cliente,id_bodega,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Articulos articulos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(articulos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_cliente = new SelectList(db.Clientes, "id_cliente", "nombre", articulos.id_cliente);
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", articulos.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", articulos.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", articulos.id_usuario_modificacion);
            ViewBag.id_articulo_tipo = new SelectList(db.Articulo_Tipo, "id_articulo_tipo", "descripcion", articulos.id_articulo_tipo);
            ViewBag.id_bodega = new SelectList(db.Bodegas, "id_bodega", "descripcion", articulos.id_bodega);
            ViewBag.id_marca = new SelectList(db.Marcas, "id_marca", "descripcion", articulos.id_marca);
            ViewBag.id_proveedor = new SelectList(db.Proveedores, "id_proveedor", "descripcion", articulos.id_proveedor);
            return View(articulos);
        }

        // GET: Inventario/Articulos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articulos articulos = db.Articulos.Find(id);
            if (articulos == null)
            {
                return HttpNotFound();
            }
            return View(articulos);
        }

        // POST: Inventario/Articulos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Articulos articulos = db.Articulos.Find(id);
            db.Articulos.Remove(articulos);
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
