using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Models;
using MVC2013.Src.Comun.Util;
using MVC2013.Src.Seguridad.To;
using System.IO;

namespace MVC2013.Areas.Administracion.Controllers
{
    public class Tipo_ArchivoController : Controller
    {
        private AppEntities db = new AppEntities(Constantes.getDataSource());

        // GET: Administracion/Tipo_Archivo
        public ActionResult Index()
        {
            return View(db.Tipo_Archivo.Where(t=>!t.eliminado).OrderBy(t=>t.nombre).ToList());
        }

        // GET: Administracion/Tipo_Archivo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Archivo tipo_Archivo = db.Tipo_Archivo.Find(id);
            if (tipo_Archivo == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Archivo);
        }

        // GET: Administracion/Tipo_Archivo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Tipo_Archivo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_tipo_archivo,nombre")] Tipo_Archivo tipo_Archivo)
        {
            if (ModelState.IsValid)
            {
                tipo_Archivo.activo = true;
                tipo_Archivo.eliminado = false;
                tipo_Archivo.fecha_creacion = DateTime.Now;
                tipo_Archivo.id_usuario_creacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                db.Tipo_Archivo.Add(tipo_Archivo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipo_Archivo);
        }

        // GET: Administracion/Tipo_Archivo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Archivo tipo_Archivo = db.Tipo_Archivo.Find(id);
            if (tipo_Archivo == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Archivo);
        }

        // POST: Administracion/Tipo_Archivo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_tipo_archivo,nombre")] Tipo_Archivo tipo_Archivo)
        {
            if (ModelState.IsValid)
            {
                //string oldpath = db.Tipo_Archivo.Find(tipo_Archivo.id_tipo_archivo).nombre;
                Tipo_Archivo edit_tipo_archivo = db.Tipo_Archivo.Find(tipo_Archivo.id_tipo_archivo);
                edit_tipo_archivo.nombre = tipo_Archivo.nombre;
                edit_tipo_archivo.fecha_modificacion = DateTime.Now;
                edit_tipo_archivo.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
                //Directory.Move(App_GlobalResources.Resources.FilePath + "\\" + oldpath, App_GlobalResources.Resources.FilePath + "\\" + tipo_Archivo.nombre);
                db.Entry(edit_tipo_archivo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipo_Archivo);
        }

        // GET: Administracion/Tipo_Archivo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Archivo tipo_Archivo = db.Tipo_Archivo.Find(id);
            if (tipo_Archivo == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Archivo);
        }

        // POST: Administracion/Tipo_Archivo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tipo_Archivo tipo_Archivo = db.Tipo_Archivo.Find(id);
            tipo_Archivo.activo = false;
            tipo_Archivo.eliminado = true;
            tipo_Archivo.fecha_eliminacion = DateTime.Now;
            tipo_Archivo.id_usuario_eliminacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            db.Entry(tipo_Archivo).State = EntityState.Modified;
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
