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

namespace MVC2013.Areas.Tickets.Controllers
{
    public class Tickets_InventarioController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Tickets/Tickets_Inventario
        public ActionResult Index()
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            ViewBag.Tickets = db.Tickets_Tipo.Where(x => x.activo && !x.eliminado);
            var tickets_Inventario = db.Tickets_Inventario/*.Include(t => t.Usuarios).Include(t => t.Usuarios1).Include(t => t.Usuarios2).Include(t => t.Usuarios3)*/.Where(x => x.activo && !x.eliminado && x.id_usuario==usuarioTO.usuario.id_usuario);
            return View(tickets_Inventario.ToList());
        }

        public ActionResult InventarioByEncargados()
        {
            ViewBag.Tickets = db.Tickets_Tipo.Where(x => x.activo && !x.eliminado);
            var tickets_Inventario = db.Tickets_Inventario.Include(t => t.Usuarios).Include(t => t.Usuarios1).Include(t => t.Usuarios2).Include(t => t.Usuarios3).Where(x => x.activo && !x.eliminado);
            return View(tickets_Inventario.ToList());
        }

        // GET: Tickets/Tickets_Inventario/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Inventario tickets_Inventario = db.Tickets_Inventario.Find(id);
            if (tickets_Inventario == null)
            {
                return HttpNotFound();
            }
            return View(tickets_Inventario);
        }

        // GET: Tickets/Tickets_Inventario/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tickets/Tickets_Inventario/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tickets_Inventario tickets_Inventario)
        {
            if (ModelState.IsValid)
            {
                db.Tickets_Inventario.Add(tickets_Inventario);
                db.SaveChanges();
                return RedirectToAction("Index", "Tickets_Generacion");
            }
            return RedirectToAction("Create", "Tickets_Generacion");
        }

        // GET: Tickets/Tickets_Inventario/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Inventario tickets_Inventario = db.Tickets_Inventario.Find(id);
            if (tickets_Inventario == null)
            {
                return HttpNotFound();
            }
            return View(tickets_Inventario);
        }

        // POST: Tickets/Tickets_Inventario/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tickets_Inventario tickets_Inventario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tickets_Inventario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Tickets_Generacion");
            }
            return RedirectToAction("Create", "Tickets_Generacion");
        }

        // GET: Tickets/Tickets_Inventario/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets_Inventario tickets_Inventario = db.Tickets_Inventario.Find(id);
            if (tickets_Inventario == null)
            {
                return HttpNotFound();
            }
            return View(tickets_Inventario);
        }

        // POST: Tickets/Tickets_Inventario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tickets_Inventario tickets_Inventario = db.Tickets_Inventario.Find(id);
            db.Tickets_Inventario.Remove(tickets_Inventario);
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
