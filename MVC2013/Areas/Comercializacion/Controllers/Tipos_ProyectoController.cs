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

namespace MVC2013.Areas.Comercializacion.Controllers
{
    public class Tipos_ProyectoController : Controller
    {
        private Protal_webEntities db = new Protal_webEntities();

        // GET: Comercializacion/Tipos_Proyecto
        public ActionResult Index()
        {
            return View(db.Pt_Tipos_Proyecto.Where(x => x.eliminado == false).ToList().OrderBy(tp => tp.ctpo_descripcion));
        }

        // GET: Comercializacion/Tipos_Proyecto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Tipos_Proyecto pt_Tipos_Proyecto = db.Pt_Tipos_Proyecto.Find(id);
            if (pt_Tipos_Proyecto == null)
            {
                return HttpNotFound();
            }
            return View(pt_Tipos_Proyecto);
        }

        // GET: Comercializacion/Tipos_Proyecto/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comercializacion/Tipos_Proyecto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pt_Tipos_Proyecto tiposProyecto)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                tiposProyecto.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                tiposProyecto.fecha_creacion = DateTime.Now;
                tiposProyecto.activo = true;
                tiposProyecto.eliminado = false;
                db.Pt_Tipos_Proyecto.Add(tiposProyecto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tiposProyecto);
        }

        // GET: Comercializacion/Tipos_Proyecto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Tipos_Proyecto pt_Tipos_Proyecto = db.Pt_Tipos_Proyecto.Find(id);
            if (pt_Tipos_Proyecto == null)
            {
                return HttpNotFound();
            }
            return View(pt_Tipos_Proyecto);
        }

        // POST: Comercializacion/Tipos_Proyecto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pt_Tipos_Proyecto tiposProyecto)
        {
            if (ModelState.IsValid)
            {
                Pt_Tipos_Proyecto tiposProyectoEdit = db.Pt_Tipos_Proyecto.Find(tiposProyecto.ctpo_id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                tiposProyectoEdit.ctpo_descripcion = tiposProyecto.ctpo_descripcion;
                tiposProyectoEdit.activo = true;
                tiposProyectoEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                tiposProyectoEdit.fecha_modificacion = DateTime.Now;
                tiposProyectoEdit.eliminado = false;
                db.Entry(tiposProyectoEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tiposProyecto);
        }

        // GET: Comercializacion/Tipos_Proyecto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Tipos_Proyecto pt_Tipos_Proyecto = db.Pt_Tipos_Proyecto.Find(id);
            if (pt_Tipos_Proyecto == null)
            {
                return HttpNotFound();
            }
            return View(pt_Tipos_Proyecto);
        }

        // POST: Comercializacion/Tipos_Proyecto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pt_Tipos_Proyecto tiposProyecto = db.Pt_Tipos_Proyecto.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            tiposProyecto.activo = false;
            tiposProyecto.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            tiposProyecto.fecha_eliminacion = DateTime.Now;
            tiposProyecto.eliminado = true;
            db.Entry(tiposProyecto).State = EntityState.Modified;
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
