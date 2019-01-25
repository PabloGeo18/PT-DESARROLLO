using System.Web.Mvc;

namespace MVC2013.Areas.Customers
{
    public class ClientesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Customers";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Customers_default",
                "Customers/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MVC2013.Areas.Customers.Controllers" }
            );


        }
    }
}