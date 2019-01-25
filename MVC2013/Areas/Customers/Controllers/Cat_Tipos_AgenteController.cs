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
    public class Cat_Tipos_AgenteController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Customers/Cat_Tipos_Agente
        public ActionResult Index()
        {
            var cat_Tipos_Agente = db.Cat_Tipos_Agente.Where(x => x.activo && !x.eliminado);
            return View(cat_Tipos_Agente.ToList());
        }

        // GET: Customers/Cat_Tipos_Agente/Details/5
        public ActionResult Details(int? id)
        {
            Cat_Tipos_Agente cat_Tipos_Agente = db.Cat_Tipos_Agente.Find(id);
            if (cat_Tipos_Agente == null)
            {
                return HttpNotFound();
            }
            return View(cat_Tipos_Agente);
        }

        // GET: Customers/Cat_Tipos_Agente/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Cat_Tipos_Agente/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cat_Tipos_Agente cat_Tipos_Agente)
        {

            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                cat_Tipos_Agente.activo = true;
                cat_Tipos_Agente.eliminado = false;
                cat_Tipos_Agente.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                cat_Tipos_Agente.fecha_creacion = DateTime.Now;
                cat_Tipos_Agente.nombre = cat_Tipos_Agente.nombre.ToUpper();
                db.Cat_Tipos_Agente.Add(cat_Tipos_Agente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View(cat_Tipos_Agente);
        }

        // GET: Customers/Cat_Tipos_Agente/Edit/5
        public ActionResult Edit(int? id)
        {
            Cat_Tipos_Agente cat_Tipos_Agente = db.Cat_Tipos_Agente.Find(id);
            if (cat_Tipos_Agente == null)
            {
                return HttpNotFound();
            }

            return View(cat_Tipos_Agente);
        }

        // POST: Customers/Cat_Tipos_Agente/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cat_Tipos_Agente cat_Tipos_Agente)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                var edit_cat_tipo_agente = db.Cat_Tipos_Agente.Find(cat_Tipos_Agente.id_cat_tipo_agente);
                edit_cat_tipo_agente.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                edit_cat_tipo_agente.fecha_modificacion = DateTime.Now;
                edit_cat_tipo_agente.nombre = cat_Tipos_Agente.nombre.ToUpper();
                db.Entry(edit_cat_tipo_agente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cat_Tipos_Agente);
        }

        // GET: Customers/Cat_Tipos_Agente/Delete/5
        public ActionResult Delete(int? id)
        {
            Cat_Tipos_Agente cat_Tipos_Agente = db.Cat_Tipos_Agente.Find(id);
            if (cat_Tipos_Agente == null)
            {
                return HttpNotFound();
            }
            return View(cat_Tipos_Agente);
        }

        // POST: Customers/Cat_Tipos_Agente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var edit_cat_tipo_agente = db.Cat_Tipos_Agente.Find(id);
            edit_cat_tipo_agente.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            edit_cat_tipo_agente.fecha_eliminacion = DateTime.Now;
            edit_cat_tipo_agente.activo = false;
            edit_cat_tipo_agente.eliminado = true;
            db.Entry(edit_cat_tipo_agente).State = EntityState.Modified;
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
