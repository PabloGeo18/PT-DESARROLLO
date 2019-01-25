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
    public class Cat_Razones_AnulacionController : Controller
    {
        private AppEntities db = new AppEntities();
        
        // GET: Customers/Cat_Razones_Anulacion
        public ActionResult Index()
        {
            var cat_Razones_Anulacion = db.Cat_Razones_Anulacion.Where(x => x.activo && !x.eliminado);
            return View(cat_Razones_Anulacion.ToList());
        }

        // GET: Customers/Cat_Razones_Anulacion/Details/5
        public ActionResult Details(int? id)
        {
            Cat_Razones_Anulacion cat_Razones_Anulacion = db.Cat_Razones_Anulacion.Find(id);
            if (cat_Razones_Anulacion == null)
            {
                return HttpNotFound();
            }
            return View(cat_Razones_Anulacion);
        }

        // GET: Customers/Cat_Razones_Anulacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Cat_Razones_Anulacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cat_Razones_Anulacion cat_Razones_Anulacion)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            if (ModelState.IsValid)
            {
                cat_Razones_Anulacion.nombre = cat_Razones_Anulacion.nombre.ToUpper();
                cat_Razones_Anulacion.activo = true;
                cat_Razones_Anulacion.eliminado = false;
                cat_Razones_Anulacion.fecha_creacion = DateTime.Now;
                cat_Razones_Anulacion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                db.Cat_Razones_Anulacion.Add(cat_Razones_Anulacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cat_Razones_Anulacion);
        }

        // GET: Customers/Cat_Razones_Anulacion/Edit/5
        public ActionResult Edit(int? id)
        {
            Cat_Razones_Anulacion cat_Razones_Anulacion = db.Cat_Razones_Anulacion.Find(id);
            if (cat_Razones_Anulacion == null)
            {
                return HttpNotFound();
            }
            return View(cat_Razones_Anulacion);
        }

        // POST: Customers/Cat_Razones_Anulacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cat_Razones_Anulacion cat_Razones_Anulacion)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            if (ModelState.IsValid)
            {
                var razon_anulacion_edit = db.Cat_Razones_Anulacion.Find(cat_Razones_Anulacion.id_cat_razon_anulacion);
                razon_anulacion_edit.nombre = cat_Razones_Anulacion.nombre.ToUpper();
                razon_anulacion_edit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                razon_anulacion_edit.fecha_modificacion = DateTime.Now;
                db.Entry(razon_anulacion_edit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cat_Razones_Anulacion);
        }

        // GET: Customers/Cat_Razones_Anulacion/Delete/5
        public ActionResult Delete(int? id)
        {
            Cat_Razones_Anulacion cat_Razones_Anulacion = db.Cat_Razones_Anulacion.Find(id);
            if (cat_Razones_Anulacion == null)
            {
                return HttpNotFound();
            }
            return View(cat_Razones_Anulacion);
        }

        // POST: Customers/Cat_Razones_Anulacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var razon_anulacion_edit = db.Cat_Razones_Anulacion.Find(id);
            razon_anulacion_edit.activo = false;
            razon_anulacion_edit.eliminado = true;
            razon_anulacion_edit.fecha_eliminacion = DateTime.Now;
            razon_anulacion_edit.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            db.Entry(razon_anulacion_edit).State = EntityState.Modified;
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
