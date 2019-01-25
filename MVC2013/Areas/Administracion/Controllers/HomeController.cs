using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC2013.Areas.Administracion.Controllers
{
    public class HomeController : Controller
    {
        // GET: Administracion/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PermisoDenegado() {
            return View();
        }



    }
}