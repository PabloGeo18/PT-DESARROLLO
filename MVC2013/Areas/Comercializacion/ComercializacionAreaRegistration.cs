using System.Web.Mvc;

namespace MVC2013.Areas.Comercializacion
{
    public class ComercializacionAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Comercializacion";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Comercializacion_default",
                "Comercializacion/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}