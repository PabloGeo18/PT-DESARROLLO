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
    public class Tipo_PagosController : Controller
    {
        private Protal_webEntities db = new Protal_webEntities();

        // GET: Comercializacion/Tipo_Pagos
        public ActionResult Index()
        {
            return View(db.Pt_Tipo_Pagos.Where(x => x.eliminado == false).ToList().OrderBy(tp => tp.ctpa_descripcion));
        }

        // GET: Comercializacion/Tipo_Pagos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Tipo_Pagos pt_Tipo_Pagos = db.Pt_Tipo_Pagos.Find(id);
            if (pt_Tipo_Pagos == null)
            {
                return HttpNotFound();
            }
            return View(pt_Tipo_Pagos);
        }

        // GET: Comercializacion/Tipo_Pagos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comercializacion/Tipo_Pagos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pt_Tipo_Pagos tipoPagos)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                tipoPagos.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                tipoPagos.fecha_creacion = DateTime.Now;
                tipoPagos.activo = true;
                tipoPagos.eliminado = false;
                db.Pt_Tipo_Pagos.Add(tipoPagos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoPagos);
        }

        // GET: Comercializacion/Tipo_Pagos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Tipo_Pagos pt_Tipo_Pagos = db.Pt_Tipo_Pagos.Find(id);
            if (pt_Tipo_Pagos == null)
            {
                return HttpNotFound();
            }
            return View(pt_Tipo_Pagos);
        }

        // POST: Comercializacion/Tipo_Pagos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pt_Tipo_Pagos tipoPagos)
        {
            if (ModelState.IsValid)
            {
                Pt_Tipo_Pagos tipoPagosEdit = db.Pt_Tipo_Pagos.Find(tipoPagos.ctpa_id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                tipoPagosEdit.ctpa_descripcion = tipoPagos.ctpa_descripcion;
                tipoPagosEdit.activo = true;
                tipoPagosEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                tipoPagosEdit.fecha_modificacion = DateTime.Now;
                tipoPagosEdit.eliminado = false;
                db.Entry(tipoPagosEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoPagos);
        }

        // GET: Comercializacion/Tipo_Pagos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Tipo_Pagos pt_Tipo_Pagos = db.Pt_Tipo_Pagos.Find(id);
            if (pt_Tipo_Pagos == null)
            {
                return HttpNotFound();
            }
            return View(pt_Tipo_Pagos);
        }

        // POST: Comercializacion/Tipo_Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pt_Tipo_Pagos tipoPagos = db.Pt_Tipo_Pagos.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            tipoPagos.activo = false;
            tipoPagos.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            tipoPagos.fecha_eliminacion = DateTime.Now;
            tipoPagos.eliminado = true;
            db.Entry(tipoPagos).State = EntityState.Modified;
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
