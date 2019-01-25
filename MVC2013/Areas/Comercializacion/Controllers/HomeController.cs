using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC2013.Src.Comun.View;

namespace MVC2013.Areas.Comercializacion.Controllers
{
    public class HomeController : Controller
    {
        // GET: Comercializacion/Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PermisoDenegado()
        {
            return View();
        }

        public ViewResult Mensaje()
        {
            ContextMessage msg = (ContextMessage)TempData[User.Identity.Name];
            return View(ContextMessage.ViewLocation(this), msg);
        }
    }
}