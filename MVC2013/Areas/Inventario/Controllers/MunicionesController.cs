using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Src.Seguridad.To;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Areas.Inventario.Controllers
{
    public class MunicionesController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Inventario/Municiones
        public ActionResult Index()
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var roles = usuarioTO.usuario.Roles;
            int rol = roles.Where(x => x.id_aplicacion == Convert.ToInt32(Catalogos.ModulosPT.Inventario)).Select(x => x.id_rol).SingleOrDefault();
            ViewBag.rol = rol;
            if (rol == Convert.ToInt32(Catalogos.InventarioRoles.Bodeguero))
            {
                var bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_bodega == 1 && x.autorizada).OrderBy(x=>x.id_municion);//.Include(b => b.Usuarios).Include(b => b.Usuarios1).Include(b => b.Usuarios2).Include(b => b.Bodegas).Include(b => b.Municiones).Where(b => b.activo && !b.eliminado).Where(b => b.id_bodega == egresos.id_bodega);
                return View(bodega_Inventario_Municiones.ToList());
            }
            else
            {
                var bodega_Inventario_Municiones = db.Bodega_Inventario_Municiones.Where(x => x.activo && !x.eliminado && x.id_bodega == 4);//.Include(b => b.Usuarios).Include(b => b.Usuarios1).Include(b => b.Usuarios2).Include(b => b.Bodegas).Include(b => b.Municiones).Where(b => b.activo && !b.eliminado).Where(b => b.id_bodega == egresos.id_bodega);
                return View(bodega_Inventario_Municiones.ToList());
            }
        }

        // GET: Inventario/Municiones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Municiones municiones = db.Municiones.Find(id);
            if (municiones == null)
            {
                return HttpNotFound();
            }
            return View(municiones);
        }

        // GET: Inventario/Municiones/Create
        public ActionResult Create()
        {
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email");
            ViewBag.id_municion = new SelectList(db.Bodega_Inventario_Municiones, "id_municion", "id_municion");
            ViewBag.id_calibre = new SelectList(db.Calibres, "id_calibre", "descripcion");
            return View();
        }

        // POST: Inventario/Municiones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id, [Bind(Include = "id_municion,id_calibre,descripcion,costo,costo_venta,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Municiones municiones)
        {
            if (ModelState.IsValid)
            {
                db.Municiones.Add(municiones);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", municiones.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", municiones.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", municiones.id_usuario_modificacion);
            ViewBag.id_municion = new SelectList(db.Bodega_Inventario_Municiones, "id_municion", "id_municion", municiones.id_municion);
            ViewBag.id_calibre = new SelectList(db.Calibres, "id_calibre", "descripcion", municiones.id_calibre);
            return View(municiones);
        }

        // GET: Inventario/Municiones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Municiones municiones = db.Municiones.Find(id);
            if (municiones == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", municiones.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", municiones.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", municiones.id_usuario_modificacion);
            ViewBag.id_municion = new SelectList(db.Bodega_Inventario_Municiones, "id_municion", "id_municion", municiones.id_municion);
            ViewBag.id_calibre = new SelectList(db.Calibres, "id_calibre", "descripcion", municiones.id_calibre);
            return View(municiones);
        }

        // POST: Inventario/Municiones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_municion,id_calibre,descripcion,costo,costo_venta,existencia,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Municiones municiones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(municiones).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_usuario_creacion = new SelectList(db.Usuarios, "id_usuario", "email", municiones.id_usuario_creacion);
            ViewBag.id_usuario_eliminacion = new SelectList(db.Usuarios, "id_usuario", "email", municiones.id_usuario_eliminacion);
            ViewBag.id_usuario_modificacion = new SelectList(db.Usuarios, "id_usuario", "email", municiones.id_usuario_modificacion);
            ViewBag.id_municion = new SelectList(db.Bodega_Inventario_Municiones, "id_municion", "id_municion", municiones.id_municion);
            ViewBag.id_calibre = new SelectList(db.Calibres, "id_calibre", "descripcion", municiones.id_calibre);
            return View(municiones);
        }

        // GET: Inventario/Municiones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Municiones municiones = db.Municiones.Find(id);
            if (municiones == null)
            {
                return HttpNotFound();
            }
            return View(municiones);
        }

        // POST: Inventario/Municiones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Municiones municiones = db.Municiones.Find(id);
            db.Municiones.Remove(municiones);
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
