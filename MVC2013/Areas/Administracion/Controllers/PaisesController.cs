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
    public class PaisesController : Controller
    {
        private AppEntities db = new AppEntities(Constantes.getDataSource());

        // GET: Administracion/Paises
        public ActionResult Index()
        {
            return View(db.Paises.Where(x => x.eliminado == false).ToList());
        }

        // GET: Administracion/Paises/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paises paises = db.Paises.Find(id);
            if (paises == null)
            {
                return HttpNotFound();
            }
            return View(paises);
        }

        // GET: Administracion/Paises/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Paises/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_pais,nombre")] Paises paises)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                paises.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                paises.fecha_creacion = DateTime.Now;
                paises.activo = true;
                paises.eliminado = false;
                db.Paises.Add(paises);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(paises);
        }

        // GET: Administracion/Paises/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paises paises = db.Paises.Find(id);
            if (paises == null)
            {
                return HttpNotFound();
            }
            return View(paises);
        }

        // POST: Administracion/Paises/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_pais,nombre,activo")] Paises paises)
        {
            if (ModelState.IsValid)
            {
                Paises paisEdit = db.Paises.Find(paises.id_pais);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                paisEdit.nombre = paises.nombre;
                paisEdit.activo = paises.activo;
                paisEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                paisEdit.fecha_modificacion = DateTime.Now;
                paisEdit.eliminado = false;
                db.Entry(paisEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(paises);
        }

        // GET: Administracion/Paises/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paises paises = db.Paises.Find(id);
            if (paises == null)
            {
                return HttpNotFound();
            }
            return View(paises);
        }

        // POST: Administracion/Paises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Paises paises = db.Paises.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            paises.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            paises.fecha_eliminacion = DateTime.Now;
            paises.eliminado = true;
            db.Entry(paises).State = EntityState.Modified;
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
