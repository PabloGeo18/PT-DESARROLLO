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
    public class Razon_Social_Grupos_FacturaController : Controller
    {
        private AppEntities db = new AppEntities();

        // GET: Customers/Razon_Social_Grupos_Factura
        public ActionResult Index()
        {
            var razon_Social_Grupos_Factura = db.Razon_Social_Grupos_Factura.Where(x => x.activo && !x.eliminado);
            return View(razon_Social_Grupos_Factura.ToList());
        }

        // GET: Customers/Razon_Social_Grupos_Factura/Details/5
        public ActionResult Details(int? id)
        {
            Razon_Social_Grupos_Factura razon_Social_Grupos_Factura = db.Razon_Social_Grupos_Factura.Find(id);
            if (razon_Social_Grupos_Factura == null)
            {
                return HttpNotFound();
            }
            return View(razon_Social_Grupos_Factura);
        }

        // GET: Customers/Razon_Social_Grupos_Factura/Create
        public ActionResult Create(int id)
        {
            ViewBag.razon_social = db.Razones_Sociales.Find(id);
            return View();
        }

        // POST: Customers/Razon_Social_Grupos_Factura/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Razon_Social_Grupos_Factura razon_Social_Grupos_Factura, string descripcion_factura1, string descripcion_factura2)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                razon_Social_Grupos_Factura.nombre = razon_Social_Grupos_Factura.nombre.ToUpper();
                razon_Social_Grupos_Factura.activo = true;
                razon_Social_Grupos_Factura.eliminado = false;
                razon_Social_Grupos_Factura.descripcion_factura = descripcion_factura1.ToUpper() + " @Meses " + descripcion_factura2.ToUpper();
                razon_Social_Grupos_Factura.id_usuario_creacion = usuarioTO.usuario.id_usuario;
                razon_Social_Grupos_Factura.fecha_creacion = DateTime.Now;
                int correlativo = 1;
                var temp_rsgf = db.Razon_Social_Grupos_Factura.Where(x => x.id_razon_social == razon_Social_Grupos_Factura.id_razon_social);
                if (temp_rsgf.ToList().Count() > 0)
                {
                    correlativo = temp_rsgf.ToList().LastOrDefault().correlativo + 1;
                }
                razon_Social_Grupos_Factura.correlativo = correlativo;
                db.Razon_Social_Grupos_Factura.Add(razon_Social_Grupos_Factura);
                db.SaveChanges();
                return RedirectToAction("Details_RS", "Clientes", new { id_Razon_Social = razon_Social_Grupos_Factura.id_razon_social});
            }
            return View(razon_Social_Grupos_Factura);
        }

        // GET: Customers/Razon_Social_Grupos_Factura/Edit/5
        public ActionResult Edit(int? id)
        {
            Razon_Social_Grupos_Factura razon_Social_Grupos_Factura = db.Razon_Social_Grupos_Factura.Find(id);
            if (razon_Social_Grupos_Factura == null)
            {
                return HttpNotFound();
            }
            String[] desc = razon_Social_Grupos_Factura.descripcion_factura.Replace("@Meses", "|").Split('|');
            @ViewBag.descripcion1 = desc[0];
            @ViewBag.descripcion2 = desc[1];
            return View(razon_Social_Grupos_Factura);
        }

        // POST: Customers/Razon_Social_Grupos_Factura/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Razon_Social_Grupos_Factura razon_Social_Grupos_Factura, string descripcion_factura1, string descripcion_factura2)
        {
            if (ModelState.IsValid)
            {
                UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
                var temp_rsgf = db.Razon_Social_Grupos_Factura.Find(razon_Social_Grupos_Factura.id_razon_social_grupo_factura);
                temp_rsgf.nombre = razon_Social_Grupos_Factura.nombre.ToUpper();
                temp_rsgf.descripcion_factura = descripcion_factura1.ToUpper() + " @Meses " + descripcion_factura2.ToUpper();
                temp_rsgf.fecha_modificacion = DateTime.Now;
                temp_rsgf.id_usuario_modificacion = usuarioTO.usuario.id_usuario;
                db.Entry(temp_rsgf).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details_RS", "Clientes", new { id_Razon_Social = temp_rsgf.id_razon_social });
            }
            return View(razon_Social_Grupos_Factura);
        }

        // GET: Customers/Razon_Social_Grupos_Factura/Delete/5
        public ActionResult Delete(int? id)
        {
            Razon_Social_Grupos_Factura razon_Social_Grupos_Factura = db.Razon_Social_Grupos_Factura.Find(id);
            if (razon_Social_Grupos_Factura == null)
            {
                return HttpNotFound();
            }
            return View(razon_Social_Grupos_Factura);
        }

        // POST: Customers/Razon_Social_Grupos_Factura/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];
            var temp_rsgf = db.Razon_Social_Grupos_Factura.Find(id);
            temp_rsgf.fecha_eliminacion = DateTime.Now;
            temp_rsgf.id_usuario_eliminacion = usuarioTO.usuario.id_usuario;
            temp_rsgf.activo = false;
            temp_rsgf.eliminado = true;
            db.Entry(temp_rsgf).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details_RS", "Clientes", new { id_Razon_Social = temp_rsgf.id_razon_social });
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
