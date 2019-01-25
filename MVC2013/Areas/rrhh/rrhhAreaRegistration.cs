using System.Web.Mvc;

namespace MVC2013.Areas.rrhh
{
    public class rrhhAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "rrhh";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "rrhh_default",
                "rrhh/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MVC2013.Areas.rrhh.Controllers" }
            );
        }
    }
}