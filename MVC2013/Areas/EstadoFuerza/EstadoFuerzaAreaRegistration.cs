using System.Web.Mvc;

namespace MVC2013.Areas.EstadoFuerza
{
    public class EstadoFuerzaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "EstadoFuerza";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "EstadoFuerza_default",
                "EstadoFuerza/{controller}/{action}/{id}",
                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MVC2013.Areas.EstadoFuerza.Controllers" }

            );
        }
    }
}