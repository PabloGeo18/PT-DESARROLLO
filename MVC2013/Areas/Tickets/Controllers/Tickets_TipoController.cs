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
    public class Tickets_TipoController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Tickets/Tickets_Tipo
        public ActionResult Index()
        {
            var tickets_Tipo = db.Tickets_Tipo.Include(t => t.Usuarios).Include(t => t.Usuarios1).Include(t => t.Usuarios2).Where(t => t.activo == true && t.eliminado == false);
            return View(tickets_Tipo.ToList());
        }

        // GET: Tickets/Tickets_Tipo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Tipo tickets_Tipo = db.Tickets_Tipo.Find(id);
            if (tickets_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(tickets_Tipo);
        }

        // GET: Tickets/Tickets_Tipo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Tickets_Tipo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tickets_Tipo tickets_Tipo)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                tickets_Tipo.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                tickets_Tipo.fecha_creacion = DateTime.Now;
                tickets_Tipo.activo = true;
                tickets_Tipo.eliminado = false;
                tickets_Tipo.es_detallado = false;
                db.Tickets_Tipo.Add(tickets_Tipo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tickets_Tipo);
        }

        // GET: Tickets/Tickets_Tipo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Tipo tickets_Tipo = db.Tickets_Tipo.Find(id);
            if (tickets_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(tickets_Tipo);
        }

        // POST: Tickets/Tickets_Tipo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tickets_Tipo tickets_Tipo)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                var tt_temp = db.Tickets_Tipo.Where(x => x.id_ticket_tipo == tickets_Tipo.id_ticket_tipo).ToList().FirstOrDefault();
                tt_temp.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                tt_temp.fecha_modificacion = DateTime.Now;
                tt_temp.nombre = tickets_Tipo.nombre;
                tt_temp.valor = tickets_Tipo.valor;
                tt_temp.es_detallado = false;
                db.Entry(tt_temp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tickets_Tipo);
        }

        // GET: Tickets/Tickets_Tipo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Tipo tickets_Tipo = db.Tickets_Tipo.Find(id);
            if (tickets_Tipo == null)
            {
                return HttpNotFound();
            }
            return View(tickets_Tipo);
        }

        // POST: Tickets/Tickets_Tipo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var tt_temp = db.Tickets_Tipo.Where(x => x.id_ticket_tipo == id).ToList().FirstOrDefault();
            tt_temp.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            tt_temp.activo = false;
            tt_temp.eliminado = true;
            tt_temp.fecha_eliminacion = DateTime.Now;
            db.Entry(tt_temp).State = EntityState.Modified;
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
