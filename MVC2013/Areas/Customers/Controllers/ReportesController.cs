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
    public class ReportesController : Controller
    {
        private AppEntities db = new AppEntities();
        private int appClientes = 6;
        // GET: SalaConteo/Reportes

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
                if (rol.id_aplicacion == appClientes)
                {
                    db.Reporte_Rol.Where(r => r.id_rol == rol.id_rol).ToList().ForEach(x => reportesAcceso.Add(x.id_reporte));
                }
                
            }

            var reportes = db.Reportes.Include(r => r.Reporte_Carpeta).Include(r => r.Reporte_Grupo).Where(r => reportesAcceso.Contains(r.id_reporte));
            ViewBag.grupos = db.Reporte_Grupo.Where(r => r.Reportes.Where(x => reportesAcceso.Contains(x.id_reporte)).Count() > 0).ToList();
            return View(reportes.ToList());
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
