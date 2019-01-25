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

namespace MVC2013.Areas.Customers.Controllers
{
    public class MunicipiosController : Controller
    {
        private AppEntities db = new AppEntities(Constantes.getDataSource());

        // GET: Administracion/Municipios
        public ActionResult Index()
        {
            var municipios = db.Municipios.Include(m => m.Departamentos).OrderBy(x => x.id_departamento).ThenBy(x => x.nombre);
            return View(municipios.ToList());
        }

        // GET: Administracion/Municipios/Details/5
        public ActionResult Details(int? id)
        {
            Municipios municipios = db.Municipios.Find(id);
            if (municipios == null)
            {
                return HttpNotFound();
            }
            return View(municipios);
        }

        // GET: Administracion/Municipios/Create
        public ActionResult Create()
        {
            ViewBag.id_departamento = new SelectList(db.Departamentos, "id_departamento", "nombre");
            return View();
        }

        // POST: Administracion/Municipios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Municipios municipios)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                municipios.nombre = municipios.nombre.ToUpper();
                municipios.activo = true;
                municipios.eliminado = false;
                municipios.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                municipios.fecha_creacion = DateTime.Now;
                db.Municipios.Add(municipios);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_departamento = new SelectList(db.Departamentos, "id_departamento", "nombre", municipios.id_departamento);
            return View(municipios);
        }

        // GET: Administracion/Municipios/Edit/5
        public ActionResult Edit(int? id)
        {
            Municipios municipios = db.Municipios.Find(id);
            if (municipios == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_departamento = new SelectList(db.Departamentos, "id_departamento", "nombre", municipios.id_departamento);
            return View(municipios);
        }

        // POST: Administracion/Municipios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Municipios municipios)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                var municipio_edit = db.Municipios.Find(municipios.id_municipio);
                municipio_edit.nombre = municipios.nombre.ToUpper();
                municipio_edit.id_departamento = municipios.id_departamento;
                municipio_edit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                municipio_edit.fecha_modificacion = DateTime.Now;
                db.Entry(municipio_edit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_departamento = new SelectList(db.Departamentos, "id_departamento", "nombre", municipios.id_departamento);
            return View(municipios);
        }

        // GET: Administracion/Municipios/Delete/5
        public ActionResult Delete(int? id)
        {

            Municipios municipios = db.Municipios.Find(id);
            if (municipios == null)
            {
                return HttpNotFound();
            }
            return View(municipios);
        }

        // POST: Administracion/Municipios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var municipio_edit = db.Municipios.Find(id);
            municipio_edit.activo = false;
            municipio_edit.eliminado = true;
            municipio_edit.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            municipio_edit.fecha_eliminacion = DateTime.Now;
            db.Entry(municipio_edit).State = EntityState.Modified;
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
