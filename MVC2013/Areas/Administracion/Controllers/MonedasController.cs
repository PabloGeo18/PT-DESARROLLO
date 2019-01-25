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
    public class MonedasController : Controller
    {
        private AppEntities db = new AppEntities(Constantes.getDataSource());

        // GET: Administracion/Monedas
        public ActionResult Index()
        {
            return View(db.Monedas.Where(x => x.eliminado == false).ToList());
        }

        // GET: Administracion/Monedas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monedas monedas = db.Monedas.Find(id);
            if (monedas == null)
            {
                return HttpNotFound();
            }
            return View(monedas);
        }

        // GET: Administracion/Monedas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Monedas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_moneda,nombre,simbolo,moneda_base")] Monedas monedas)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                monedas.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                monedas.fecha_creacion = DateTime.Now;
                monedas.activo = true;
                monedas.eliminado = false;
                db.Monedas.Add(monedas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(monedas);
        }

        // GET: Administracion/Monedas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monedas monedas = db.Monedas.Find(id);
            if (monedas == null)
            {
                return HttpNotFound();
            }
            return View(monedas);
        }

        // POST: Administracion/Monedas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_moneda,nombre,simbolo,moneda_base,activo")] Monedas monedas)
        {
            if (ModelState.IsValid)
            {
                Monedas monedaEdit = db.Monedas.Find(monedas.id_moneda);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                monedaEdit.nombre = monedas.nombre;
                monedaEdit.activo = monedas.activo;
                monedaEdit.moneda_base = monedas.moneda_base;
                monedaEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                monedaEdit.fecha_modificacion = DateTime.Now;
                monedaEdit.eliminado = false;
                db.Entry(monedaEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(monedas);
        }

        // GET: Administracion/Monedas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Monedas monedas = db.Monedas.Find(id);
            if (monedas == null)
            {
                return HttpNotFound();
            }
            return View(monedas);
        }

        // POST: Administracion/Monedas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id){
            Monedas monedas = db.Monedas.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            monedas.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            monedas.fecha_eliminacion = DateTime.Now;
            monedas.eliminado = true;
            db.Entry(monedas).State = EntityState.Modified;
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
