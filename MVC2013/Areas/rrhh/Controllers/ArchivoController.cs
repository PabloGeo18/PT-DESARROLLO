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
using MVC2013.Src.Seguridad.To;

namespace MVC2013.Areas.Administracion.Controllers
{
    public class ArchivoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Archivoes
        public ActionResult Index()
        {
            var archivo = db.Archivo.Where(a => !a.eliminado);
            return View(archivo.ToList());
        }

        // GET: Administracion/Archivoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Archivo archivo = db.Archivo.Find(id);
            if (archivo == null)
            {
                return HttpNotFound();
            }
            return View(archivo);
        }

        // GET: Administracion/Archivoes/Create
        public ActionResult Create()
        {
            ViewBag.id_tipo_archivo = new SelectList(db.Tipo_Archivo, "id_tipo_archivo", "nombre");
            return View();
        }

        // POST: Administracion/Archivoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_archivo,id_tipo_archivo,nombre,ubicacion,activo,eliminado,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion")] Archivo archivo)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                archivo.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                archivo.fecha_creacion = DateTime.Now;
                archivo.activo = true;
                archivo.eliminado = false;
                db.Archivo.Add(archivo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_tipo_archivo = new SelectList(db.Tipo_Archivo, "id_tipo_archivo", "nombre", archivo.id_tipo_archivo);
            return View(archivo);
        }

        // GET: Administracion/Archivoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Archivo archivo = db.Archivo.Find(id);
            if (archivo == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_tipo_archivo = new SelectList(db.Tipo_Archivo, "id_tipo_archivo", "nombre", archivo.id_tipo_archivo);
            return View(archivo);
        }

        // POST: Administracion/Archivoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_archivo,id_tipo_archivo,nombre,ubicacion,activo,eliminado,fecha_creacion,fecha_modificacion,fecha_eliminacion,id_usuario_creacion,id_usuario_modificacion,id_usuario_eliminacion")] Archivo archivo)
        {
            if (ModelState.IsValid)
            {
                Archivo archivoEdit = db.Archivo.Find(archivo.id_archivo);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                archivoEdit.nombre = archivo.nombre;
                archivoEdit.fecha_modificacion = DateTime.Now;
                archivoEdit.eliminado = false;
                db.Entry(archivoEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_tipo_archivo = new SelectList(db.Tipo_Archivo, "id_tipo_archivo", "nombre", archivo.id_tipo_archivo);
            return View(archivo);
        }

        // GET: Administracion/Archivoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Archivo archivo = db.Archivo.Find(id);
            if (archivo == null)
            {
                return HttpNotFound();
            }
            return View(archivo);
        }

        // POST: Administracion/Archivoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Archivo archivo = db.Archivo.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            archivo.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            archivo.fecha_eliminacion = DateTime.Now;
            archivo.eliminado = true;
            db.Archivo.Remove(archivo);
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
