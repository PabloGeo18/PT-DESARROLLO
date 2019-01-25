using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Web.Mvc.Html;
using MVC2013.Src.Comun.Util;

namespace MVC2013.Src.Comun.Helper
{
    public static class ButtonExtension
    {
        public static MvcHtmlString MenuLinkButton(this HtmlHelper htmlHelper, string linkText, string action, string controller, string userName, string icon, string clase, Object param, bool link, String id, string styles, string titleB)
        {
            string areaName = (htmlHelper.ViewContext.RouteData.DataTokens["area"] != null && string.IsNullOrEmpty((string)htmlHelper.ViewContext.RouteData.DataTokens["area"])) ? string.Empty : (string)htmlHelper.ViewContext.RouteData.DataTokens["area"];
            if (Cache.DiccionarioUsuariosLogueados.ContainsKey(userName) && Cache.DiccionarioUsuariosLogueados[userName].havePermissions(areaName, controller, action))
            {
                if (!link)
                {
                    var requestContext = HttpContext.Current.Request.RequestContext;
                    UrlHelper UrlHelper = new UrlHelper(requestContext);
                    var result = new StringBuilder();
                    result.Append("<button data-toggle='tooltip' data-placement='top' title=\""+titleB+"\" class='" + clase.ToString() + "' id=\"" + id + "\" style=\""+styles+"\">");
                    if (!String.IsNullOrEmpty(icon))
                    {
                        result.Append("<span class='" + icon + "'></span>&nbsp");
                    }
                    result.Append(linkText + "</button>");
                    return new MvcHtmlString(result.ToString());
                }
                else
                {
                    var requestContext = HttpContext.Current.Request.RequestContext;
                    UrlHelper UrlHelper = new UrlHelper(requestContext);
                    var result = new StringBuilder();
                    result.Append("<button data-toggle='tooltip' data-placement='top' title=\"" + titleB + "\" class='" + clase.ToString() + "' onclick=\"location.href=");
                    result.Append("'" + UrlHelper.Action(action, controller, param) + "'");
                    result.Append(" \" style=\"" + styles + "\">");
                    if (!String.IsNullOrEmpty(icon))
                    {
                        result.Append("<span class='" + icon + "'></span>&nbsp");
                    }
                    result.Append(linkText + "</button>");
                    return new MvcHtmlString(result.ToString());
                }

            }
            return new MvcHtmlString("");
        }

    }
}