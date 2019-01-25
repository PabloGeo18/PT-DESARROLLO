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
using MVC2013.Src.Comun.View;
using System.Globalization;

namespace MVC2013.Areas.rrhh.Controllers
{
    public class SalarioController : Controller
    {
        private AppEntities db = new AppEntities();


        public ActionResult SueldoBase()
        {
            Periodo periodo = db.Periodo.Where(p => p.activo).FirstOrDefault();
            ViewBag.historico = db.Periodo.Where(p => !p.activo && !p.eliminado);
            return View(periodo);
        }

        [HttpPost]
        public ActionResult EditarSueldoBase(int id_periodo, string salario_minimo, string bono_decreto)
        {
            decimal salario = 0;
            decimal bono = 0;
            Periodo periodo = db.Periodo.Find(id_periodo);
            if(!String.IsNullOrEmpty(salario_minimo))
            {
                salario = Convert.ToDecimal(salario_minimo, CultureInfo.InvariantCulture);
                periodo.salario_minimo = salario;
            }
            if (!String.IsNullOrEmpty(bono_decreto))
            {
                bono = Convert.ToDecimal(bono_decreto, CultureInfo.InvariantCulture);
                periodo.bono_decreto = bono;
            }
            periodo.fecha_modificacion = DateTime.Now;
            periodo.id_usuario_modificacion = Cache.DiccionarioUsuariosLogueados[User.Identity.Name].usuario.id_usuario;
            try
            {
                db.Entry(periodo).State = EntityState.Modified;
                db.Modificar_Sueldo_Contratos(salario, bono);
                db.SaveChanges();
                ContextMessage msg = new ContextMessage(ContextMessage.Success, "Cambios guardados exitosamente.");
                msg.ReturnUrl = Url.Action("SueldoBase");
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
            catch
            {
                ContextMessage msg = new ContextMessage(ContextMessage.Error, "Ocurrio un error con la conexión del servidor. Cambios no guardados.");
                msg.ReturnUrl = Url.Action("SueldoBase");
                TempData[User.Identity.Name] = msg;
                return RedirectToAction("Mensaje");
            }
        }

        public ViewResult Mensaje()
        {
            ContextMessage msg = (ContextMessage)TempData[User.Identity.Name];
            return View(ContextMessage.ViewLocation(this), msg);
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
