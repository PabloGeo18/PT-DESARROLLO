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
    public class Cat_Otros_ServiciosController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Customers/Cat_Otros_Servicios
        public ActionResult Index()
        {
            var Cat_Otros_Servicios = db.Cat_Otros_Servicios.Where(x => x.activo && !x.eliminado);
            return View(Cat_Otros_Servicios.ToList());
        }

        // GET: Customers/Cat_Otros_Servicios/Details/5
        public ActionResult Details(int? id)
        {
            Cat_Otros_Servicios cat_Otros_Servicios = db.Cat_Otros_Servicios.Find(id);
            if (cat_Otros_Servicios == null)
            {
                return HttpNotFound();
            }
            return View(cat_Otros_Servicios);
        }

        // GET: Customers/Cat_Otros_Servicios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Cat_Otros_Servicios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cat_Otros_Servicios cat_Otros_Servicios)
        {

            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                cat_Otros_Servicios.activo = true;
                cat_Otros_Servicios.eliminado = false;
                cat_Otros_Servicios.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                cat_Otros_Servicios.fecha_creacion = DateTime.Now;
                cat_Otros_Servicios.nombre = cat_Otros_Servicios.nombre.ToUpper();
                db.Cat_Otros_Servicios.Add(cat_Otros_Servicios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View(cat_Otros_Servicios);
        }

        // GET: Customers/Cat_Otros_Servicios/Edit/5
        public ActionResult Edit(int? id)
        {
            Cat_Otros_Servicios Cat_Otros_Servicios = db.Cat_Otros_Servicios.Find(id);
            if (Cat_Otros_Servicios == null)
            {
                return HttpNotFound();
            }

            return View(Cat_Otros_Servicios);
        }

        // POST: Customers/Cat_Otros_Servicios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cat_Otros_Servicios cat_Otros_Servicios)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                var edit_cat_tipo_servicio = db.Cat_Otros_Servicios.Find(cat_Otros_Servicios.id_cat_otro_servicio);
                edit_cat_tipo_servicio.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                edit_cat_tipo_servicio.fecha_modificacion = DateTime.Now;
                edit_cat_tipo_servicio.nombre = cat_Otros_Servicios.nombre.ToUpper();
                db.Entry(edit_cat_tipo_servicio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cat_Otros_Servicios);
        }

        // GET: Customers/Cat_Otros_Servicios/Delete/5
        public ActionResult Delete(int? id)
        {
            Cat_Otros_Servicios Cat_Otros_Servicios = db.Cat_Otros_Servicios.Find(id);
            if (Cat_Otros_Servicios == null)
            {
                return HttpNotFound();
            }
            return View(Cat_Otros_Servicios);
        }

        // POST: Customers/Cat_Otros_Servicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var edit_cat_tipo_servicio = db.Cat_Otros_Servicios.Find(id);
            edit_cat_tipo_servicio.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            edit_cat_tipo_servicio.fecha_eliminacion = DateTime.Now;
            edit_cat_tipo_servicio.activo = false;
            edit_cat_tipo_servicio.eliminado = true;
            db.Entry(edit_cat_tipo_servicio).State = EntityState.Modified;
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
