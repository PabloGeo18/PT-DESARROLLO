using MVC2013.Src.Comun.Util;
using MVC2013.Src.Seguridad.To;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MVC2013.Src.Comun.Helper {
    public static class MenuExtension {

        public static MvcHtmlString MenuLinkLi(this HtmlHelper htmlHelper, string linkText, string action, string controller, string userName) {

            ////UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[usuario];
            //string areaName = (htmlHelper.ViewContext.RouteData.DataTokens["area"] != null && string.IsNullOrEmpty((string)htmlHelper.ViewContext.RouteData.DataTokens["area"])) ? string.Empty : (string)htmlHelper.ViewContext.RouteData.DataTokens["area"];
            ////string controllerName = (string)html.ViewContext.RouteData.GetRequiredString("controller");
            ////string actionName = (string)html.ViewContext.RouteData.DataTokens["action"];

            ///*if(Cache.DiccionarioUsuariosLogueados.ContainsKey(userName) && Cache.DiccionarioUsuariosLogueados[userName].havePermissions(areaName, controller, action)) {
            //*/

            //    var result = new StringBuilder();
            //    //LinkExtensions.ActionLink(htmlHelper, linkText, action, controller);
            //    //var url = UrlHelper.GenerateContentUrl("/" + controller + "/" + action, html.ViewContext.HttpContext);          
            //    result.Append("<li>");
            //   // result.Append("<a href=\"");
            //    //result.Append(HttpUtility.HtmlAttributeEncode(url));
            //    //result.Append("\" data-ajax-update=\"");
            //    //result.Append("\" >" + linkString + "</a>");
            //    result.Append(LinkExtensions.ActionLink(htmlHelper, linkText, action, controller).ToString());
            //    result.Append("</li>");
            //    //result.Append(HttpUtility.HtmlAttributeEncode("#" + ajaxUpdateId));
            //    // ... and so on
            //   // <li>@Html.ActionLink(@Resources.Resources.administracion_menu_roles, "Index", "Roles")</li>

            //    return new MvcHtmlString(result.ToString());
            ///*}*/
            ////return new MvcHtmlString("");

            //UsuarioTO usuarioTO = Cache.DiccionarioUsuariosLogueados[usuario];
            string areaName = (htmlHelper.ViewContext.RouteData.DataTokens["area"] != null && string.IsNullOrEmpty((string)htmlHelper.ViewContext.RouteData.DataTokens["area"])) ? string.Empty : (string)htmlHelper.ViewContext.RouteData.DataTokens["area"];
            //string controllerName = (string)html.ViewContext.RouteData.GetRequiredString("controller");
            //string actionName = (string)html.ViewContext.RouteData.DataTokens["action"];

            if (Cache.DiccionarioUsuariosLogueados.ContainsKey(userName) && Cache.DiccionarioUsuariosLogueados[userName].havePermissions(areaName, controller, action))
            {


                var result = new StringBuilder();
                //LinkExtensions.ActionLink(htmlHelper, linkText, action, controller);
                //var url = UrlHelper.GenerateContentUrl("/" + controller + "/" + action, html.ViewContext.HttpContext);          
                result.Append("<li>");
                // result.Append("<a href=\"");
                //result.Append(HttpUtility.HtmlAttributeEncode(url));
                //result.Append("\" data-ajax-update=\"");
                //result.Append("\" >" + linkString + "</a>");
                result.Append(LinkExtensions.ActionLink(htmlHelper, linkText, action, controller).ToString());
                result.Append("</li>");
                //result.Append(HttpUtility.HtmlAttributeEncode("#" + ajaxUpdateId));
                // ... and so on
                // <li>@Html.ActionLink(@Resources.Resources.administracion_menu_roles, "Index", "Roles")</li>

                return new MvcHtmlString(result.ToString());
            }
            return new MvcHtmlString("");
        }

        public static MvcHtmlString ahref(this HtmlHelper htmlHelper, string linkText, string action, string controller, string userName)
        {
            string areaName = (htmlHelper.ViewContext.RouteData.DataTokens["area"] != null && string.IsNullOrEmpty((string)htmlHelper.ViewContext.RouteData.DataTokens["area"])) ? string.Empty : (string)htmlHelper.ViewContext.RouteData.DataTokens["area"];
            if (Cache.DiccionarioUsuariosLogueados.ContainsKey(userName) && Cache.DiccionarioUsuariosLogueados[userName].havePermissions(areaName, controller, action))
            {


                var result = new StringBuilder();       
                result.Append("<a href='#'>");
                result.Append("<span>" + linkText + "</span>&nbsp");
                result.Append("<i class='fa fa-angle-left pull-right'></i>");
                // result.Append("<a href=\"");
                //result.Append(HttpUtility.HtmlAttributeEncode(url));
                //result.Append("\" data-ajax-update=\"");
                //result.Append("\" >" + linkString + "</a>");
                //result.Append(LinkExtensions.ActionLink(htmlHelper, linkText, action, controller).ToString());
                result.Append("</a>");
                //result.Append(HttpUtility.HtmlAttributeEncode("#" + ajaxUpdateId));
                // ... and so on
                // <li>@Html.ActionLink(@Resources.Resources.administracion_menu_roles, "Index", "Roles")</li>

                return new MvcHtmlString(result.ToString());
            }
            return new MvcHtmlString("");
        }

        public static MvcHtmlString SpecialMenuLinkLi(this HtmlHelper htmlHelper, string linkText, String action, String controller, string userName, String innerHtml)
        {

            string areaName = (htmlHelper.ViewContext.RouteData.DataTokens["area"] != null && string.IsNullOrEmpty((string)htmlHelper.ViewContext.RouteData.DataTokens["area"])) ? string.Empty : (string)htmlHelper.ViewContext.RouteData.DataTokens["area"];
            if (Cache.DiccionarioUsuariosLogueados.ContainsKey(userName) && Cache.DiccionarioUsuariosLogueados[userName].havePermissions(areaName, controller, action))
            {
                TagBuilder tag = new TagBuilder("a");
                tag.MergeAttribute("href", new UrlHelper(htmlHelper.ViewContext.RequestContext).Action(action, controller));
                tag.InnerHtml = innerHtml + " <span>" + linkText + " </span>";
                return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
            }
            return new MvcHtmlString("");
        }

        public static MvcHtmlString SpecialActionLink(this HtmlHelper htmlHelper, string linkText, String action, String controller, String innerHtml)
        {
            TagBuilder tag = new TagBuilder("a");
            tag.MergeAttribute("href", new UrlHelper(htmlHelper.ViewContext.RequestContext).Action(action, controller));
            tag.InnerHtml = innerHtml + " <span>" + linkText + " </span>";
            return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
        }

    }
}