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

namespace MVC2013.Areas.Customers.Controllers
{
    public class Cat_Razones_EliminacionController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Customers/Cat_Razones_Eliminacion
        public ActionResult Index()
        {
            var Cat_Razones_Eliminacion = db.Cat_Razones_Eliminacion.Where(x => x.activo && !x.eliminado);
            return View(Cat_Razones_Eliminacion.ToList());
        }

        // GET: Customers/Cat_Razones_Eliminacion/Details/5
        public ActionResult Details(int? id)
        {
            Cat_Razones_Eliminacion cat_razones_eliminacion = db.Cat_Razones_Eliminacion.Find(id);
            if (cat_razones_eliminacion == null)
            {
                return HttpNotFound();
            }
            return View(cat_razones_eliminacion);
        }

        // GET: Customers/Cat_Razones_Eliminacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Cat_Razones_Eliminacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cat_Razones_Eliminacion cat_razones_eliminacion)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            if (ModelState.IsValid)
            {
                cat_razones_eliminacion.nombre = cat_razones_eliminacion.nombre.ToUpper();
                cat_razones_eliminacion.activo = true;
                cat_razones_eliminacion.eliminado = false;
                cat_razones_eliminacion.fecha_creacion = DateTime.Now;
                cat_razones_eliminacion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                db.Cat_Razones_Eliminacion.Add(cat_razones_eliminacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cat_razones_eliminacion);
        }

        // GET: Customers/Cat_Razones_Eliminacion/Edit/5
        public ActionResult Edit(int? id)
        {
            Cat_Razones_Eliminacion cat_razones_eliminacion = db.Cat_Razones_Eliminacion.Find(id);
            if (cat_razones_eliminacion == null)
            {
                return HttpNotFound();
            }
            return View(cat_razones_eliminacion);
        }

        // POST: Customers/Cat_Razones_Eliminacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cat_Razones_Eliminacion cat_razones_eliminacion)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            if (ModelState.IsValid)
            {
                var razon_eliminacion_edit = db.Cat_Razones_Eliminacion.Find(cat_razones_eliminacion.id_cat_razon_eliminacion);
                razon_eliminacion_edit.nombre = cat_razones_eliminacion.nombre.ToUpper();
                razon_eliminacion_edit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                razon_eliminacion_edit.fecha_modificacion = DateTime.Now;
                db.Entry(razon_eliminacion_edit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cat_razones_eliminacion);
        }

        // GET: Customers/Cat_Razones_Eliminacion/Delete/5
        public ActionResult Delete(int? id)
        {
            Cat_Razones_Eliminacion cat_razones_eliminacion = db.Cat_Razones_Eliminacion.Find(id);
            if (cat_razones_eliminacion == null)
            {
                return HttpNotFound();
            }
            return View(cat_razones_eliminacion);
        }

        // POST: Customers/Cat_Razones_Eliminacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var razon_eliminacion_edit = db.Cat_Razones_Eliminacion.Find(id);
            razon_eliminacion_edit.activo = false;
            razon_eliminacion_edit.eliminado = true;
            razon_eliminacion_edit.fecha_eliminacion = DateTime.Now;
            razon_eliminacion_edit.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            db.Entry(razon_eliminacion_edit).State = EntityState.Modified;
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
