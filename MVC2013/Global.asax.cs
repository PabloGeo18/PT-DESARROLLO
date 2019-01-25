using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using MVC2013.Src.Binder;

namespace MVC2013
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalNullableModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime), new DateModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateNullableModelBinder());
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);    
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //  //GlobalConfiguration.Configure(WebApiConfig.Register);
            
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);

            // Iniciamos log4net
            log4net.Config.XmlConfigurator.Configure();
        }

        /* Implementacion de internacionalizacion.
         * by: jcrodas
         */
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var handler = Context.Handler as MvcHandler;
            var routeData = handler != null ? handler.RequestContext.RouteData : null;
            var languageCookie = HttpContext.Current.Request.Cookies["lang"];
            var userLanguages = HttpContext.Current.Request.UserLanguages;

            // Set the Culture based on a route, a cookie or the browser settings,
            // or default value if something went wrong
            //var cultureInfo = new CultureInfo(languageCookie != null ? languageCookie.Value : userLanguages != null ? userLanguages[0] : "es");
            var cultureInfo = new CultureInfo("es");
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            Session["init"] = 0;
        }
    }
}
