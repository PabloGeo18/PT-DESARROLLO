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
    public class ContactosController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Contactos
        public ActionResult Index()
        {
            var contactos = db.Contactos.Where(x => x.eliminado == false);
            return View(contactos.ToList());
        }

        // GET: Administracion/Contactos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contactos contactos = db.Contactos.Find(id);
            if (contactos == null)
            {
                return HttpNotFound();
            }
            return View(contactos);
        }

        // GET: Administracion/Contactos/Create
        public ActionResult Create()
        {
            ViewBag.id_contacto_puesto = new SelectList(db.Contacto_Puesto.Where(x => x.eliminado == false), "id_contacto_puesto", "descripcion");
            return View();
        }

        // POST: Administracion/Contactos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contactos contactos)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                contactos.eliminado = false;
                contactos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                contactos.fecha_creacion = DateTime.Now;
                db.Contactos.Add(contactos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_contacto_puesto = new SelectList(db.Contacto_Puesto.Where(x => x.eliminado == false), "id_contacto_puesto", "descripcion", contactos.id_contacto_puesto);
            return View(contactos);
        }

        // GET: Administracion/Contactos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contactos contactos = db.Contactos.Find(id);
            if (contactos == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_contacto_puesto = new SelectList(db.Contacto_Puesto.Where(x => x.eliminado == false), "id_contacto_puesto", "descripcion", contactos.id_contacto_puesto);
            return View(contactos);
        }

        // POST: Administracion/Contactos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Contactos contactos)
        {
            if (ModelState.IsValid)
            {
                Contactos contactos_edit = db.Contactos.Find(contactos.id_contacto);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                contactos_edit.nombre = contactos.nombre;
                contactos_edit.id_contacto_puesto = contactos.id_contacto_puesto;
                contactos_edit.telefono = contactos.telefono;
                contactos_edit.email = contactos.email;
                contactos_edit.comentario = contactos.comentario;
                contactos_edit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                contactos_edit.fecha_modificacion = DateTime.Now;
                db.Entry(contactos_edit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_contacto_puesto = new SelectList(db.Contacto_Puesto.Where(x => x.eliminado == false), "id_contacto_puesto", "descripcion", contactos.id_contacto_puesto);
            return View(contactos);
        }

        // GET: Administracion/Contactos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contactos contactos = db.Contactos.Find(id);
            if (contactos == null)
            {
                return HttpNotFound();
            }
            return View(contactos);
        }

        // POST: Administracion/Contactos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contactos contactos = db.Contactos.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            contactos.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            contactos.fecha_eliminacion = DateTime.Now;
            contactos.eliminado = true;
            contactos.activo = false;
            db.Entry(contactos).State = EntityState.Modified;
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
