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
    public class Cat_Tipos_AgenteController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Administracion/Tipo_Agentes
        public ActionResult Index()
        {
            var tipo_Agentes = db.Cat_Tipos_Agente.Where(t => t.activo && !t.eliminado).ToList();
            return View(tipo_Agentes);
        }

        // GET: Administracion/Tipo_Agentes/Details/5
        public ActionResult Details(int id)
        {
            Cat_Tipos_Agente tipo_Agentes = db.Cat_Tipos_Agente.Find(id);
            if (tipo_Agentes == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Agentes);
        }

        // GET: Administracion/Tipo_Agentes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Tipo_Agentes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cat_Tipos_Agente tipo_Agentes)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                tipo_Agentes.activo = true;
                tipo_Agentes.eliminado = false;
                tipo_Agentes.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                tipo_Agentes.fecha_creacion = DateTime.Now;
                db.Cat_Tipos_Agente.Add(tipo_Agentes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipo_Agentes);
        }

        // GET: Administracion/Tipo_Agentes/Edit/5
        public ActionResult Edit(int id)
        {
            Cat_Tipos_Agente tipo_Agentes = db.Cat_Tipos_Agente.Find(id);
            if (tipo_Agentes == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Agentes);
        }

        // POST: Administracion/Tipo_Agentes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cat_Tipos_Agente tipo_Agentes)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                Cat_Tipos_Agente edit_agente = db.Cat_Tipos_Agente.Where(x => x.id_cat_tipo_agente == tipo_Agentes.id_cat_tipo_agente).FirstOrDefault();
                edit_agente.nombre = tipo_Agentes.nombre;
                edit_agente.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                edit_agente.fecha_modificacion = DateTime.Now;
                db.Entry(edit_agente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipo_Agentes);
        }

        // GET: Administracion/Tipo_Agentes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cat_Tipos_Agente tipo_Agentes = db.Cat_Tipos_Agente.Find(id);
            if (tipo_Agentes == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Agentes);
        }

        // POST: Administracion/Tipo_Agentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cat_Tipos_Agente tipo_Agentes = db.Cat_Tipos_Agente.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            tipo_Agentes.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            tipo_Agentes.eliminado = true;
            tipo_Agentes.activo = false;
            tipo_Agentes.fecha_eliminacion = DateTime.Now;
            db.Entry(tipo_Agentes).State = EntityState.Modified;
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
