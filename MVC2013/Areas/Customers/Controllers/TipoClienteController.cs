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
    public class TipoClienteController : Controller
    {
        private AppEntities db = new AppEntities(Constantes.getDataSource());

        // GET: Administracion/TipoCliente
        public ActionResult Index()
        {
            var tipoClienteList = db.Tipo_Cliente.Where(x => x.eliminado == false).ToList();
            return View(tipoClienteList);
        }

        // GET: Administracion/TipoCliente/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Cliente tipo_Cliente = db.Tipo_Cliente.Find(id);
            if (tipo_Cliente == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Cliente);
        }

        // GET: Administracion/TipoCliente/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/TipoCliente/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_tipo_cliente,nombre")] Tipo_Cliente tipo_Cliente)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                tipo_Cliente.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                tipo_Cliente.fecha_creacion = DateTime.Now;
                tipo_Cliente.activo = true;
                tipo_Cliente.eliminado = false;
                db.Tipo_Cliente.Add(tipo_Cliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipo_Cliente);
        }

        // GET: Administracion/TipoCliente/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Cliente tipo_Cliente = db.Tipo_Cliente.Find(id);
            if (tipo_Cliente == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Cliente);
        }

        // POST: Administracion/TipoCliente/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_tipo_cliente,nombre,activo")] Tipo_Cliente tipo_Cliente)
        {
            if (ModelState.IsValid)
            {
                Tipo_Cliente tipoClienteEdit = db.Tipo_Cliente.Find(tipo_Cliente.id_tipo_cliente);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

                tipoClienteEdit.nombre = tipo_Cliente.nombre;
                tipoClienteEdit.activo = tipo_Cliente.activo;
                tipoClienteEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                tipoClienteEdit.fecha_modificacion = DateTime.Now;
                tipoClienteEdit.eliminado = false;
                db.Entry(tipoClienteEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipo_Cliente);
        }

        // GET: Administracion/TipoCliente/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Cliente tipo_Cliente = db.Tipo_Cliente.Find(id);
            if (tipo_Cliente == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Cliente);
        }

        // POST: Administracion/TipoCliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tipo_Cliente tipo_Cliente = db.Tipo_Cliente.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            tipo_Cliente.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            tipo_Cliente.fecha_eliminacion = DateTime.Now;
            tipo_Cliente.eliminado = true;
            tipo_Cliente.activo = false;
            db.Entry(tipo_Cliente).State = EntityState.Modified;
            db.SaveChanges();
            //db.Tipo_Cliente.Remove(tipo_Cliente);
            //db.SaveChanges();
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
