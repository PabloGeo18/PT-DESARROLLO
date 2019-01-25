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
    public class Contacto_PuestoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Contacto_Puesto
        public ActionResult Index()
        {
            var contacto_Puesto = db.Contacto_Puesto.Where(x => x.eliminado == false);
            return View(contacto_Puesto.ToList());
        }

        // GET: Administracion/Contacto_Puesto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contacto_Puesto contacto_Puesto = db.Contacto_Puesto.Find(id);
            if (contacto_Puesto == null)
            {
                return HttpNotFound();
            }
            return View(contacto_Puesto);
        }

        // GET: Administracion/Contacto_Puesto/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Contacto_Puesto/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_contacto_puesto,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Contacto_Puesto contacto_Puesto)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                contacto_Puesto.eliminado = false;
                contacto_Puesto.activo = true;
                contacto_Puesto.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                contacto_Puesto.fecha_creacion = DateTime.Now;
                db.Contacto_Puesto.Add(contacto_Puesto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contacto_Puesto);
        }

        // GET: Administracion/Contacto_Puesto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contacto_Puesto contacto_Puesto = db.Contacto_Puesto.Find(id);
            if (contacto_Puesto == null)
            {
                return HttpNotFound();
            }
            return View(contacto_Puesto);
        }

        // POST: Administracion/Contacto_Puesto/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_contacto_puesto,descripcion,activo,eliminado,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion,fecha_creacion,fecha_modificacion,fecha_eliminacion")] Contacto_Puesto contacto_Puesto)
        {
            if (ModelState.IsValid)
            {
                Contacto_Puesto contacto_puesto_edit = db.Contacto_Puesto.Find(contacto_Puesto.id_contacto_puesto);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                contacto_puesto_edit.descripcion = contacto_Puesto.descripcion;

                contacto_puesto_edit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                contacto_puesto_edit.fecha_modificacion = DateTime.Now;
                contacto_puesto_edit.eliminado = false;
                contacto_puesto_edit.activo = true;
                db.Entry(contacto_puesto_edit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contacto_Puesto);
        }

        // GET: Administracion/Contacto_Puesto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contacto_Puesto contacto_Puesto = db.Contacto_Puesto.Find(id);
            if (contacto_Puesto == null)
            {
                return HttpNotFound();
            }
            return View(contacto_Puesto);
        }

        // POST: Administracion/Contacto_Puesto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contacto_Puesto contacto_Puesto = db.Contacto_Puesto.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            contacto_Puesto.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            contacto_Puesto.fecha_eliminacion = DateTime.Now;
            contacto_Puesto.eliminado = true;
            contacto_Puesto.activo = false;
            db.Entry(contacto_Puesto).State = EntityState.Modified;
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
