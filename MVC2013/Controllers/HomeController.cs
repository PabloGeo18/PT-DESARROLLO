using MVC2013.Src.Comun.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC2013.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult PermisoDenegado()
        {
            return View();
        }

        public ActionResult Index()
        {
            if (TempData.ContainsKey("HasExpiredSession")) 
            {
                ViewBag.message = "Por seguridad su ultima sesion fue expirada";
                TempData.Remove("HasExpiredSession");
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Proteccion Total, S. A.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "www.protecciontotal.gt";

            return View();
        }


        public ActionResult ChangeCulture(string lang)
        {
            var langCookie = new HttpCookie("lang", lang) { HttpOnly = true };
            Response.AppendCookie(langCookie);
            return RedirectToAction("Index", "Home");
        }

    }
}