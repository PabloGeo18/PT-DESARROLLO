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
using System.Reflection;

namespace MVC2013.Areas.Administracion.Controllers
{
    public class AplicacionesController : Controller
    {
        private AppEntities db = new AppEntities(Constantes.getDataSource());

        // GET: Administracion/Aplicaciones
        public ActionResult Index()
        {
            return View(db.Aplicaciones.ToList());
        }

        public ActionResult Lista_controladores()
        {
            Assembly asm = Assembly.GetAssembly(typeof(MVC2013.MvcApplication));

            var controlleractionlist = asm.GetTypes()
                    .Where(type => typeof(System.Web.Mvc.Controller).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                    .Select(x => new { Controller = x.DeclaringType.Name, Action = x.Name, ReturnType = x.ReturnType.Name, Attributes = String.Join(",", x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", ""))) })
                    .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();
            ViewBag.controllers = controlleractionlist;
            return View();
        }

        // GET: Administracion/Aplicaciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aplicaciones aplicaciones = db.Aplicaciones.Find(id);
            if (aplicaciones == null)
            {
                return HttpNotFound();
            }
            return View(aplicaciones);
        }

        // GET: Administracion/Aplicaciones/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administracion/Aplicaciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_aplicacion,nombre,ruta")] Aplicaciones aplicaciones)
        {
            if (ModelState.IsValid)
            {
                db.Aplicaciones.Add(aplicaciones);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aplicaciones);
        }

        // GET: Administracion/Aplicaciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aplicaciones aplicaciones = db.Aplicaciones.Find(id);
            if (aplicaciones == null)
            {
                return HttpNotFound();
            }
            return View(aplicaciones);
        }

        // POST: Administracion/Aplicaciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_aplicacion,nombre,ruta")] Aplicaciones aplicaciones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aplicaciones).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aplicaciones);
        }

        // GET: Administracion/Aplicaciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aplicaciones aplicaciones = db.Aplicaciones.Find(id);
            if (aplicaciones == null)
            {
                return HttpNotFound();
            }
            return View(aplicaciones);
        }

        // POST: Administracion/Aplicaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Aplicaciones aplicaciones = db.Aplicaciones.Find(id);
            db.Aplicaciones.Remove(aplicaciones);
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
