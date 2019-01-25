using MVC2013.Src.Seguridad.Filtros;
using System.Web;
using System.Web.Mvc;

namespace MVC2013
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new System.Web.Mvc.AuthorizeAttribute());

            filters.Add(new FiltroSeguridad());

            //filters.Add(new RequireHttpsAttribute());
        }
    }
}
