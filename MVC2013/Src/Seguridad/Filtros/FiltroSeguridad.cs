using log4net;
using MVC2013.Src.Comun.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace MVC2013.Src.Seguridad.Filtros
{
    public class FiltroSeguridad : ActionFilterAttribute
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        
        
        private static string GetAreaName(RouteBase route)
        {
            var area = route as IRouteWithArea;
            if (area != null)
            {
                return area.Area;
            }
            var route2 = route as Route;
            if ((route2 != null) && (route2.DataTokens != null))
            {
                return (route2.DataTokens["area"] as string);
            }
            return null;
        }


        private static string GetAreaName(RouteData routeData)
        {
            object obj2;
            if (routeData.DataTokens.TryGetValue("area", out obj2))
            {
                return (obj2 as string);
            }
            return GetAreaName(routeData.Route);
        }


        // Método ejecutado justo antes de la ejecución de la acción
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string tmpMsg = string.Empty;
            base.OnActionExecuting(filterContext);     

            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string areaName  = GetAreaName(filterContext.RouteData);
            string userName  = filterContext.HttpContext.User.Identity.Name;
            bool enableRoles = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["EnabledRoles"]);

            List<string> actionFilterExceptions = new List<string>
            {
                "MasterLogin"
                ,"GetImage"
            };

            if (!actionFilterExceptions.Contains(actionName)) 
            {
                if (!String.IsNullOrEmpty(userName) && Cache.DiccionarioUsuariosLogueados.ContainsKey(userName) && actionName != "Login")
                {
                    if (Cache.DiccionarioUsuariosLogueados[userName].SessionId != HttpContext.Current.Session.SessionID)
                    {
                        FormsAuthentication.SignOut();
                        filterContext.Controller.TempData["SessionExpired"] = true;
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary 
                        { 
                            { "area", ""},
                            { "controller", "Home" }, 
                            { "action", "Index" } 
                        }
                        );
                    }
                }

                if (!Cache.DiccionarioUsuariosLogueados.ContainsKey(userName) && actionName != "Login")
                {
                    FormsAuthentication.SignOut();
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary 
                        { 
                            { "area", ""},
                            { "controller", "Home" }, 
                            { "action", "Index" } 
                        });
                }
                else if (!string.IsNullOrEmpty(areaName) && !string.IsNullOrEmpty(userName) && actionName != "PermisoDenegado" && enableRoles)
                {
                    if (!Cache.DiccionarioUsuariosLogueados[userName].havePermissions(areaName, controllerName, actionName))
                    {
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary 
                        { 
                            { "area", areaName != null ? areaName : ""},
                            { "controller", "Home" }, 
                            { "action", "PermisoDenegado" } 
                        });
                    }
                }

                log.Debug("Method: " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
            }

        }




        // Método ejecutado justo después de la ejecución de la acción
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //string tmpMsg = string.Empty;
            //base.OnActionExecuted(filterContext);

            //// Almacenamos el nombre del método
            //log.Debug(System.Reflection.MethodBase.GetCurrentMethod().ToString());

            //// Recogemos el resultado
            //ActionResult result = filterContext.Result;
            //log.Debug("ActionResult: " + result.ToString());

            //// Comprobamos si se ha producido alguna excepción durante la ejecución.
            //// En caso afirmativo, la almacenamos
            //if (filterContext.Exception != null)
            //    log.Error("Error durante la ejecución de la acción", filterContext.Exception);

            //log.Debug(" --------------------------------------- ");

        }

        // Método ejecutado justo antes de la ejecución del resultado
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //base.OnResultExecuting(filterContext);
            //log.Debug(System.Reflection.MethodBase.GetCurrentMethod().ToString());

            //// Recogemos el resultado
            //ActionResult result = filterContext.Result;
            //log.Debug("ActionResult: " + result.ToString());

            //log.Debug(" --------------------------------------- ");
        }

        // Método ejecutado justo después de la ejecución del resultado
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //base.OnResultExecuted(filterContext);
            //log.Debug(System.Reflection.MethodBase.GetCurrentMethod().ToString());

            //// Recogemos el resultado
            //ActionResult result = filterContext.Result;
            //log.Debug("ActionResult: " + result.ToString());

            //log.Debug(" --------------------------------------- ");
        }


    }
}