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

namespace MVC2013.Areas.Tickets.Controllers
{
    public class Tickets_Tipo_MovimientoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Tickets/Tickets_Tipo_Movimiento
        public ActionResult Index()
        {
            var tickets_Tipo_Movimiento = db.Tickets_Tipo_Movimiento.Include(t => t.Usuarios).Include(t => t.Usuarios1).Include(t => t.Usuarios2).Where(x => x.activo == true && x.eliminado == false);
            return View(tickets_Tipo_Movimiento.ToList());
        }

        // GET: Tickets/Tickets_Tipo_Movimiento/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Tipo_Movimiento tickets_Tipo_Movimiento = db.Tickets_Tipo_Movimiento.Find(id);
            if (tickets_Tipo_Movimiento == null)
            {
                return HttpNotFound();
            }
            return View(tickets_Tipo_Movimiento);
        }

        // GET: Tickets/Tickets_Tipo_Movimiento/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Tickets/Tickets_Tipo_Movimiento/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tickets_Tipo_Movimiento tickets_Tipo_Movimiento)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                tickets_Tipo_Movimiento.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                tickets_Tipo_Movimiento.fecha_creacion = DateTime.Now;
                tickets_Tipo_Movimiento.activo = true;
                tickets_Tipo_Movimiento.eliminado = false;
                tickets_Tipo_Movimiento.es_detallado = false; 
                db.Tickets_Tipo_Movimiento.Add(tickets_Tipo_Movimiento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tickets_Tipo_Movimiento);
        }

        // GET: Tickets/Tickets_Tipo_Movimiento/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Tipo_Movimiento tickets_Tipo_Movimiento = db.Tickets_Tipo_Movimiento.Find(id);
            if (tickets_Tipo_Movimiento == null)
            {
                return HttpNotFound();
            }

            return View(tickets_Tipo_Movimiento);
        }

        // POST: Tickets/Tickets_Tipo_Movimiento/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tickets_Tipo_Movimiento tickets_Tipo_Movimiento)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                var ttm_temp = db.Tickets_Tipo_Movimiento.Where(x => x.id_ticket_tipo_movimiento == tickets_Tipo_Movimiento.id_ticket_tipo_movimiento).ToList().FirstOrDefault();
                ttm_temp.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                ttm_temp.nombre = tickets_Tipo_Movimiento.nombre;
                ttm_temp.fecha_modificacion = DateTime.Now;
                db.Entry(ttm_temp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tickets_Tipo_Movimiento);
        }

        // GET: Tickets/Tickets_Tipo_Movimiento/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Tipo_Movimiento tickets_Tipo_Movimiento = db.Tickets_Tipo_Movimiento.Find(id);
            if (tickets_Tipo_Movimiento == null)
            {
                return HttpNotFound();
            }
            return View(tickets_Tipo_Movimiento);
        }

        // POST: Tickets/Tickets_Tipo_Movimiento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name]; 
            var ttm_temp = db.Tickets_Tipo_Movimiento.Where(x => x.id_ticket_tipo_movimiento == id).ToList().FirstOrDefault();
            ttm_temp.activo = false;
            ttm_temp.eliminado = true;
            ttm_temp.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            ttm_temp.fecha_eliminacion = DateTime.Now;
            db.Entry(ttm_temp).State = EntityState.Modified;
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
