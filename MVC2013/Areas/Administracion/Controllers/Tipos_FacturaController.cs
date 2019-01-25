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
    public class Cat_Tipos_FacturacionController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Cat_Tipos_Facturacion
        public ActionResult Index()
        {
            var Cat_Tipos_Facturacion = db.Cat_Tipos_Facturacion.Where(x => x.activo && !x.eliminado).Include(t => t.Usuarios).Include(t => t.Usuarios1).Include(t => t.Usuarios2);
            return View(Cat_Tipos_Facturacion.ToList());
        }

        // GET: Administracion/Cat_Tipos_Facturacion/Details/5
        public ActionResult Details(int id)
        {
            Cat_Tipos_Facturacion Cat_Tipos_Facturacion = db.Cat_Tipos_Facturacion.Find(id);
            if (Cat_Tipos_Facturacion == null)
            {
                return HttpNotFound();
            }
            return View(Cat_Tipos_Facturacion);
        }

        // GET: Administracion/Cat_Tipos_Facturacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Cat_Tipos_Facturacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cat_Tipos_Facturacion Cat_Tipos_Facturacion)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                Cat_Tipos_Facturacion.activo = true;
                Cat_Tipos_Facturacion.eliminado = false;
                Cat_Tipos_Facturacion.fecha_creacion = DateTime.Now;
                Cat_Tipos_Facturacion.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                db.Cat_Tipos_Facturacion.Add(Cat_Tipos_Facturacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Cat_Tipos_Facturacion);
        }

        // GET: Administracion/Cat_Tipos_Facturacion/Edit/5
        public ActionResult Edit(int id)
        {
            Cat_Tipos_Facturacion Cat_Tipos_Facturacion = db.Cat_Tipos_Facturacion.Find(id);
            if (Cat_Tipos_Facturacion == null)
            {
                return HttpNotFound();
            }
            return View(Cat_Tipos_Facturacion);
        }

        // POST: Administracion/Cat_Tipos_Facturacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cat_Tipos_Facturacion Cat_Tipos_Facturacion)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                Cat_Tipos_Facturacion edit_tipo_factura = db.Cat_Tipos_Facturacion.Where(x => x.id_cat_tipo_facturacion == Cat_Tipos_Facturacion.id_cat_tipo_facturacion).FirstOrDefault();
                edit_tipo_factura.nombre = Cat_Tipos_Facturacion.nombre;
                //edit_tipo_factura.serie = Cat_Tipos_Facturacion.serie;
                edit_tipo_factura.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                edit_tipo_factura.fecha_modificacion = DateTime.Now;

                db.Entry(edit_tipo_factura).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(Cat_Tipos_Facturacion);
        }

        // GET: Administracion/Cat_Tipos_Facturacion/Delete/5
        public ActionResult Delete(int id)
        {

            Cat_Tipos_Facturacion Cat_Tipos_Facturacion = db.Cat_Tipos_Facturacion.Find(id);
            if (Cat_Tipos_Facturacion == null)
            {
                return HttpNotFound();
            }
            return View(Cat_Tipos_Facturacion);
        }

        // POST: Administracion/Cat_Tipos_Facturacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cat_Tipos_Facturacion edit_tipo_factura = db.Cat_Tipos_Facturacion.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            edit_tipo_factura.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            edit_tipo_factura.fecha_eliminacion = DateTime.Now;
            edit_tipo_factura.activo = false;
            edit_tipo_factura.eliminado = true;
            db.Entry(edit_tipo_factura).State = EntityState.Modified;
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
