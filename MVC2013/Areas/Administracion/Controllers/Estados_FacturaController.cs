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

namespace MVC2013.Areas.Administracion.Controllers
{
    public class Estados_FacturaController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Estados_Factura
        public ActionResult Index()
        {
            var estados_Factura = db.Estados_Factura.Where(x => x.activo && !x.eliminado).Include(e => e.Usuarios).Include(e => e.Usuarios1).Include(e => e.Usuarios2);
            return View(estados_Factura.ToList());
        }

        // GET: Estados_Factura/Details/5
        public ActionResult Details(int id)
        {

            Estados_Factura estados_Factura = db.Estados_Factura.Find(id);
            if (estados_Factura == null)
            {
                return HttpNotFound();
            }
            return View(estados_Factura);
        }

        // GET: Estados_Factura/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Estados_Factura/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Estados_Factura estados_Factura)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                estados_Factura.activo = true;
                estados_Factura.eliminado = false;
                estados_Factura.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                estados_Factura.fecha_creacion = DateTime.Now;

                db.Estados_Factura.Add(estados_Factura);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View(estados_Factura);
        }

        // GET: Estados_Factura/Edit/5
        public ActionResult Edit(int id)
        {
            Estados_Factura estados_Factura = db.Estados_Factura.Find(id);
            if (estados_Factura == null)
            {
                return HttpNotFound();
            }

            return View(estados_Factura);
        }

        // POST: Estados_Factura/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Estados_Factura estados_Factura)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                Estados_Factura edit_estado = db.Estados_Factura.Where(x => x.id_estado_factura == estados_Factura.id_estado_factura).FirstOrDefault();
                edit_estado.fecha_modificacion = DateTime.Now;
                edit_estado.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                edit_estado.nombre = estados_Factura.nombre;
                db.Entry(edit_estado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(estados_Factura);
        }

        // GET: Estados_Factura/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estados_Factura estados_Factura = db.Estados_Factura.Find(id);
            if (estados_Factura == null)
            {
                return HttpNotFound();
            }
            return View(estados_Factura);
        }

        // POST: Estados_Factura/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            Estados_Factura edit_estado = db.Estados_Factura.Where(x => x.id_estado_factura == id).FirstOrDefault();
            edit_estado.fecha_eliminacion = DateTime.Now;
            edit_estado.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            edit_estado.activo = false;
            edit_estado.eliminado = true;
            db.Entry(edit_estado).State = EntityState.Modified;
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
