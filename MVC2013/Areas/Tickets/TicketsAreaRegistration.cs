using System.Web.Mvc;

namespace MVC2013.Areas.Tickets
{
    public class TicketsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Tickets";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Tickets_default",
                "Tickets/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MVC2013.Areas.Tickets.Controllers" }
            );
        }
    }
}