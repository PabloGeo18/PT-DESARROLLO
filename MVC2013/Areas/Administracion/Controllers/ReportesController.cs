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
    public class ReportesController : Controller
    {
        private AppEntities db = new AppEntities();
        private int appSalaConteo = 2;
        private int appAdministracion = 4;

        public ActionResult AddRol(int? RolId, int? ReporteId)
        {
            if (RolId == null || ReporteId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Roles rol = db.Roles.Find(RolId);
            Reportes reporte = db.Reportes.Find(ReporteId);
            if (rol == null || reporte == null)
            {
                return HttpNotFound();
            }
            Usuarios usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario;
            Reporte_Rol reporte_rol = new Reporte_Rol();
            reporte_rol.id_rol = rol.id_rol;
            reporte_rol.id_reporte = reporte.id_reporte;

            db.Reporte_Rol.Add(reporte_rol);
            db.SaveChanges();
            return RedirectToAction("Edit", new { id = ReporteId });
        }
        public ActionResult DeleteReporteRol(int? ReporteRolId, int? ReporteId)
        {
            if (ReporteRolId == null || ReporteId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reporte_Rol reporte_rol = db.Reporte_Rol.Where(x => x.id_reporte_rol == ReporteRolId).DefaultIfEmpty(null).SingleOrDefault();
            Reportes reporte = db.Reportes.Find(ReporteId);
            if (reporte_rol == null || reporte == null)
            {
                return HttpNotFound();
            }
            Usuarios usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario;

            db.Entry(reporte_rol).State = EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = ReporteId });
        }

        // GET: SalaConteo/Reportes
        public ActionResult Index()
        {
            var reportes = db.Reportes.Include(r => r.Reporte_Carpeta).Include(r => r.Reporte_Grupo);
            return View(reportes.ToList());
        }


        // GET: SalaConteo/Reportes
        public ActionResult ViewReport()
        {
            //UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[User.Identity.Name];

            Usuarios usuario = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario;

            IEnumerable<Roles> roles = usuario.Roles;
            List<int> reportesAcceso = new List<int>();

            //List<Roles> roles = usuario.Roles.ToList();
            
            foreach (Roles rol in roles)
            {
                if (rol.id_aplicacion == appAdministracion)
                {
                    db.Reporte_Rol.Where(r => r.id_rol == rol.id_rol).ToList().ForEach(x => reportesAcceso.Add(x.id_reporte));
                }
                
            }

            var reportes = db.Reportes.Include(r => r.Reporte_Carpeta).Include(r => r.Reporte_Grupo).Where(r => reportesAcceso.Contains(r.id_reporte));
            ViewBag.grupos = db.Reporte_Grupo.Where(r => r.Reportes.Where(x => reportesAcceso.Contains(x.id_reporte)).Count() > 0).ToList();
            return View(reportes.ToList());
        }

        // GET: SalaConteo/Reportes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reportes reportes = db.Reportes.Find(id);
            if (reportes == null)
            {
                return HttpNotFound();
            }
            return View(reportes);
        }

        // GET: SalaConteo/Reportes/Create
        public ActionResult Create()
        {
            ViewBag.id_reporte_carpeta = new SelectList(db.Reporte_Carpeta, "id_reporte_carpeta", "nombre");
            ViewBag.id_reporte_grupo = new SelectList(db.Reporte_Grupo, "id_reporte_grupo", "nombre");
            return View();
        }

        // POST: SalaConteo/Reportes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_reporte,nombre,descripcion,reporte,id_reporte_carpeta,id_reporte_grupo,url")] Reportes reportes)
        {
            if (ModelState.IsValid)
            {
                db.Reportes.Add(reportes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_reporte_carpeta = new SelectList(db.Reporte_Carpeta, "id_reporte_carpeta", "nombre", reportes.id_reporte_carpeta);
            ViewBag.id_reporte_grupo = new SelectList(db.Reporte_Grupo, "id_reporte_grupo", "nombre", reportes.id_reporte_grupo);
            return View(reportes);
        }

        // GET: SalaConteo/Reportes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reportes reportes = db.Reportes.Find(id);
            if (reportes == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_reporte_carpeta = new SelectList(db.Reporte_Carpeta, "id_reporte_carpeta", "nombre", reportes.id_reporte_carpeta);
            ViewBag.id_reporte_grupo = new SelectList(db.Reporte_Grupo, "id_reporte_grupo", "nombre", reportes.id_reporte_grupo);
            
            ViewBag.rolesToAdd = db.Roles.Where(x => x.Reporte_Rol.All(r => r.id_reporte != id)).ToList();
            ViewBag.reporte_rolesAdd = db.Reporte_Rol.Where(x => x.id_reporte == id).ToList();
            return View(reportes);
        }

        // POST: SalaConteo/Reportes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_reporte,nombre,descripcion,reporte,id_reporte_carpeta,id_reporte_grupo,url")] Reportes reportes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reportes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_reporte_carpeta = new SelectList(db.Reporte_Carpeta, "id_reporte_carpeta", "nombre", reportes.id_reporte_carpeta);
            ViewBag.id_reporte_grupo = new SelectList(db.Reporte_Grupo, "id_reporte_grupo", "nombre", reportes.id_reporte_grupo);
            return View(reportes);
        }

        // GET: SalaConteo/Reportes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reportes reportes = db.Reportes.Find(id);
            if (reportes == null)
            {
                return HttpNotFound();
            }
            return View(reportes);
        }

        // POST: SalaConteo/Reportes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reportes reportes = db.Reportes.Find(id);
            db.Reportes.Remove(reportes);
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
