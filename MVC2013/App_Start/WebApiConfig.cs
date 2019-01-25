using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MVC2013
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.EnableCors();
            //var json = config.Formatters.JsonFormatter;
            //json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            //json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //config.Formatters.Remove(config.Formatters.XmlFormatter);

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.All;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
