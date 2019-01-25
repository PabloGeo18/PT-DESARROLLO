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
    public class Estados_FaseController : Controller
    {
        private Protal_webEntities db = new Protal_webEntities();

        // GET: Comercializacion/Estados_Fase
        public ActionResult Index()
        {
            return View(db.Pt_Estados_Fase.Where(x => x.eliminado == false).ToList().OrderBy(ef => ef.cefa_descripcion));
        }

        // GET: Comercializacion/Estados_Fase/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Estados_Fase pt_Estados_Fase = db.Pt_Estados_Fase.Find(id);
            if (pt_Estados_Fase == null)
            {
                return HttpNotFound();
            }
            return View(pt_Estados_Fase);
        }

        // GET: Comercializacion/Estados_Fase/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comercializacion/Estados_Fase/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pt_Estados_Fase estados_Fase)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                estados_Fase.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                estados_Fase.fecha_creacion = DateTime.Now;
                estados_Fase.activo = true;
                estados_Fase.eliminado = false;
                db.Pt_Estados_Fase.Add(estados_Fase);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(estados_Fase);
        }

        // GET: Comercializacion/Estados_Fase/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Estados_Fase pt_Estados_Fase = db.Pt_Estados_Fase.Find(id);
            if (pt_Estados_Fase == null)
            {
                return HttpNotFound();
            }
            return View(pt_Estados_Fase);
        }

        // POST: Comercializacion/Estados_Fase/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pt_Estados_Fase estados_Fase)
        {
            if (ModelState.IsValid)
            {
                Pt_Estados_Fase estadosFaseEdit = db.Pt_Estados_Fase.Find(estados_Fase.cefa_id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                estadosFaseEdit.cefa_descripcion = estados_Fase.cefa_descripcion;
                estadosFaseEdit.activo = true;
                estadosFaseEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                estadosFaseEdit.fecha_modificacion = DateTime.Now;
                estadosFaseEdit.eliminado = false;
                db.Entry(estadosFaseEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(estados_Fase);
        }

        // GET: Comercializacion/Estados_Fase/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Estados_Fase pt_Estados_Fase = db.Pt_Estados_Fase.Find(id);
            if (pt_Estados_Fase == null)
            {
                return HttpNotFound();
            }
            return View(pt_Estados_Fase);
        }

        // POST: Comercializacion/Estados_Fase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pt_Estados_Fase estadosFase = db.Pt_Estados_Fase.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            estadosFase.activo = false;
            estadosFase.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            estadosFase.fecha_eliminacion = DateTime.Now;
            estadosFase.eliminado = true;
            db.Entry(estadosFase).State = EntityState.Modified;
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
