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
    public class Tipos_ServiciosController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Servicios_Adicionales
        public ActionResult Index()
        {
            var servicios_Adicionales = db.Tipos_Servicios.Where(x => x.activo && !x.eliminado);
            return View(servicios_Adicionales.ToList());
        }

        // GET: Administracion/Servicios_Adicionales/Details/5
        public ActionResult Details(int id)
        {
            Tipos_Servicios servicios_Adicionales = db.Tipos_Servicios.Find(id);
            if (servicios_Adicionales == null)
            {
                return HttpNotFound();
            }
            return View(servicios_Adicionales);
        }

        // GET: Administracion/Servicios_Adicionales/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Servicios_Adicionales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tipos_Servicios servicios_Adicionales)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                servicios_Adicionales.activo = true;
                servicios_Adicionales.eliminado = false;
                servicios_Adicionales.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                servicios_Adicionales.fecha_creacion = DateTime.Now;
                db.Tipos_Servicios.Add(servicios_Adicionales);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(servicios_Adicionales);
        }

        // GET: Administracion/Servicios_Adicionales/Edit/5
        public ActionResult Edit(int id)
        {
            Tipos_Servicios servicios_Adicionales = db.Tipos_Servicios.Find(id);
            if (servicios_Adicionales == null)
            {
                return HttpNotFound();
            }
            return View(servicios_Adicionales);
        }

        // POST: Administracion/Servicios_Adicionales/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tipos_Servicios servicios_Adicionales)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                Tipos_Servicios edit_sa = db.Tipos_Servicios.Where(x => x.id_tipo_servicio == servicios_Adicionales.id_tipo_servicio).ToList().FirstOrDefault();
                edit_sa.nombre = servicios_Adicionales.nombre;
                edit_sa.fecha_modificacion = DateTime.Now;
                edit_sa.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                db.Entry(edit_sa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(servicios_Adicionales);
        }

        // GET: Administracion/Servicios_Adicionales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipos_Servicios servicios_Adicionales = db.Tipos_Servicios.Find(id);
            if (servicios_Adicionales == null)
            {
                return HttpNotFound();
            }
            return View(servicios_Adicionales);
        }

        // POST: Administracion/Servicios_Adicionales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tipos_Servicios servicios_Adicionales = db.Tipos_Servicios.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            servicios_Adicionales.fecha_eliminacion = DateTime.Now;
            servicios_Adicionales.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            servicios_Adicionales.activo = false;
            servicios_Adicionales.eliminado = true;
            db.Entry(servicios_Adicionales).State = EntityState.Modified;
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
