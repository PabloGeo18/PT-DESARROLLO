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
    public class CotizacionesController : Controller
    {
        private Protal_webEntities db = new Protal_webEntities();

        // GET: Comercializacion/Cotizaciones
        public ActionResult Index()
        {
            return View(db.Pt_Cotizaciones.Where(x => x.eliminado == false && x.ccot_finaliza_cotizacion==false).ToList().OrderBy(i => i.ccot_nombre_proyecto));
        }

        public ActionResult CotizacionesEliminadas()
        {
            return View(db.Pt_Cotizaciones.Where(x => x.activo==false && x.eliminado == true).ToList().OrderBy(i => i.ccot_nombre_proyecto));
        }

        //GET Comercializacion/Cotizaciones/CotizacionesFinalizadas
        public ActionResult CotizacionesFinalizadas()
        {
            return View(db.Pt_Cotizaciones.Where(x => x.eliminado == false && x.ccot_finaliza_cotizacion == true).ToList().OrderBy(i => i.ccot_nombre_proyecto));
        }

        // GET: Comercializacion/Cotizaciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Cotizaciones pt_Cotizaciones = db.Pt_Cotizaciones.Find(id);
            if (pt_Cotizaciones == null)
            {
                return HttpNotFound();
            }
            return View(pt_Cotizaciones);
        }

        // GET: Comercializacion/Cotizaciones/Create
        public ActionResult Create()
        {
            ViewBag.ccot_ctpo_id = new SelectList(db.Pt_Tipos_Proyecto.Where(tp=>tp.activo && !tp.eliminado), "ctpo_id", "ctpo_descripcion");
            return View();
        }

        // POST: Comercializacion/Cotizaciones/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pt_Cotizaciones cotizaciones)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                cotizaciones.ccot_finaliza_cotizacion = false;
                cotizaciones.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                cotizaciones.fecha_creacion = DateTime.Now;
                cotizaciones.activo = true;
                cotizaciones.eliminado = false;
                db.Pt_Cotizaciones.Add(cotizaciones);
                db.SaveChanges();
                return RedirectToAction("Index/"+ cotizaciones.ccot_id, "Fases_Cotizacion");
            }
            ViewBag.ccot_ctpo_id = new SelectList(db.Pt_Tipos_Proyecto.Where(tp=>tp.activo && !tp.eliminado), "ctpo_id", "ctpo_descripcion", cotizaciones.ccot_ctpo_id);
            return View(cotizaciones);
        }

        // GET: Comercializacion/Cotizaciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Cotizaciones pt_Cotizaciones = db.Pt_Cotizaciones.Find(id);
            if (pt_Cotizaciones == null)
            {
                return HttpNotFound();
            }
            ViewBag.fechaEntrega = Convert.ToDateTime(pt_Cotizaciones.ccot_fecha_entrega).ToString("yyyy-MM-dd");
            ViewBag.ccot_ctpo_id = new SelectList(db.Pt_Tipos_Proyecto.Where(tp=>tp.activo && !tp.eliminado), "ctpo_id", "ctpo_descripcion", pt_Cotizaciones.ccot_ctpo_id);
            return View(pt_Cotizaciones);
        }

        // POST: Comercializacion/Cotizaciones/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pt_Cotizaciones cotizaciones)
        {
            if (ModelState.IsValid)
            {
                Pt_Cotizaciones cotizacionesEdit = db.Pt_Cotizaciones.Find(cotizaciones.ccot_id);
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                cotizacionesEdit.ccot_nombre_proyecto = cotizaciones.ccot_nombre_proyecto;
                cotizacionesEdit.ccot_descripcion_general = cotizaciones.ccot_descripcion_general;
                cotizacionesEdit.ccot_contacto = cotizaciones.ccot_contacto;
                cotizacionesEdit.ccot_telefono_contacto = cotizaciones.ccot_telefono_contacto;
                cotizacionesEdit.ccot_correo_contacto = cotizaciones.ccot_correo_contacto;
                cotizacionesEdit.ccot_direccion_contacto = cotizaciones.ccot_direccion_contacto;
                cotizacionesEdit.ccot_fecha_entrega = cotizaciones.ccot_fecha_entrega;
                cotizacionesEdit.ccot_ctpo_id = cotizaciones.ccot_ctpo_id;
                cotizacionesEdit.activo = true;
                cotizacionesEdit.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                cotizacionesEdit.fecha_modificacion = DateTime.Now;
                cotizacionesEdit.eliminado = false;
                db.Entry(cotizacionesEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ccot_ctpo_id = new SelectList(db.Pt_Tipos_Proyecto.Where(tp=>tp.activo && !tp.eliminado), "ctpo_id", "ctpo_descripcion", cotizaciones.ccot_ctpo_id);
            return View(cotizaciones);
        }

        // GET: Comercializacion/Cotizaciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Cotizaciones pt_Cotizaciones = db.Pt_Cotizaciones.Find(id);
            if (pt_Cotizaciones == null)
            {
                return HttpNotFound();
            }
            return View(pt_Cotizaciones);
        }

        //GET Comercializacion/Cotizaciones/ClonarCotizacion/5
        public ActionResult ClonarCotizacion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Cotizaciones cotizacion = db.Pt_Cotizaciones.Find(id);
            if (cotizacion == null)
            {
                return HttpNotFound();
            }
            return View(cotizacion);
        }

        // POST: Comercializacion/Cotizaciones/ClonarCotizacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClonarCotizacion(int id)
        {
            Pt_Cotizaciones cotizaciones = db.Pt_Cotizaciones.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            List<Pt_Fases_Cotizacion> fasesList = db.Pt_Fases_Cotizacion.Where(fc => fc.activo && !fc.eliminado && fc.cfas_ccot_id == cotizaciones.ccot_id).ToList();
            foreach (Pt_Fases_Cotizacion fc in fasesList) {
                fc.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                fc.fecha_creacion = DateTime.Now;
                fc.activo = true;
                fc.eliminado = false;
                db.Pt_Fases_Cotizacion.Add(fc);
                List<Pt_Tmp_Propuesta_Fase_Puesto> temp = db.Pt_Tmp_Propuesta_Fase_Puesto.Where(t => t.ctpf_cfas_id == fc.cfas_id && t.activo && !t.eliminado).ToList();
                foreach (var t in temp)
                {
                    t.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                    t.fecha_creacion = DateTime.Now;
                    t.activo = true;
                    t.eliminado = false;
                    db.Pt_Tmp_Propuesta_Fase_Puesto.Add(t);
                }
            }
            cotizaciones.ccot_nombre_proyecto = "Clon " + cotizaciones.ccot_nombre_proyecto;
            cotizaciones.ccot_finaliza_cotizacion = false;
            cotizaciones.id_usuario_creacion = usuarioTO.usuario.id_usuario;
            cotizaciones.fecha_creacion = DateTime.Now;
            cotizaciones.activo = true;
            cotizaciones.eliminado = false;
            db.Pt_Cotizaciones.Add(cotizaciones);
            db.SaveChanges();
            return RedirectToAction("Index/" + cotizaciones.ccot_id);
        }

        // GET: Comercializacion/Cotizaciones/FinalizarCotizacion/5
        public ActionResult FinalizarCotizacion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pt_Cotizaciones pt_Cotizaciones = db.Pt_Cotizaciones.Find(id);
            if (pt_Cotizaciones == null)
            {
                return HttpNotFound();
            }
            return View(pt_Cotizaciones);
        }

        // POST: FinalizarCotizacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinalizarCotizacion(int id)
        {
            Pt_Cotizaciones cotizaciones = db.Pt_Cotizaciones.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            cotizaciones.ccot_finaliza_cotizacion = true;
            cotizaciones.activo = true;
            cotizaciones.ccot_fecha_finalizacion = DateTime.Now;
            cotizaciones.eliminado = false;
            db.Entry(cotizaciones).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Comercializacion/Cotizaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pt_Cotizaciones cotizaciones = db.Pt_Cotizaciones.Find(id);
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            cotizaciones.activo = false;
            cotizaciones.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            cotizaciones.fecha_eliminacion = DateTime.Now;
            cotizaciones.eliminado = true;
            db.Entry(cotizaciones).State = EntityState.Modified;
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
