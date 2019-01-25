using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using System.IO;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Areas.Customers.Controllers
{
    public class HomeController : Controller
    {
        // GET: Administracion/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PermisoDenegado()
        {
            return View();
        }
        
    }
}