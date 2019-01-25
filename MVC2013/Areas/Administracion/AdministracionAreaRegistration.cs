using System.Web.Mvc;

namespace MVC2013.Areas.Administracion
{
    public class AdministracionAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Administracion";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Administracion_default",
                "Administracion/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MVC2013.Areas.Administracion.Controllers" }
            );
        }
    }
}