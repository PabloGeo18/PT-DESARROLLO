using System.Web.Mvc;

namespace MVC2013.Areas.Inventario
{
    public class InventarioAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Inventario";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Inventario_default",
                "Inventario/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MVC2013.Areas.Inventario.Controllers" }
            );
        }
    }
}