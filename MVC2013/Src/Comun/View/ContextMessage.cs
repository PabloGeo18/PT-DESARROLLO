using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;

namespace MVC2013.Src.Comun.View
{
    public class ContextMessage
    {
        public static TempDataDictionary messages = new TempDataDictionary();

        public const string Success = "success";
        public const string Error = "error";
        public const string Warning = "warning";
        public const string Info = "info";
        public const string Default = "default";

        public string Type { 
            get
            {
                return this.Type;
            } 
            set 
            {
                this.SetType = value;
                this.Title = GetContextDefaultTitle();
            } 
        }

        private string SetType;
        public string Message { get; set; }
        public string Title { get; set; }
        public string CustomViewString = null;
        public string ReturnUrl = null;
        public bool HasCustomView = false;
        public bool HasFooter = true;
        public Dictionary<string, object> ViewData = new Dictionary<string, object>();
        
        public static string ViewLocation(Controller context) 
        {
            string areaName = (string)context.Request.RequestContext.RouteData.DataTokens["area"] ?? "";
            areaName = !String.IsNullOrEmpty(areaName) ? "/Areas/" + areaName : "";
            return ("~:AREA/Views/Shared/ContextMessage.cshtml").Replace(":AREA", areaName);
        }


        private string GetContextDefaultTitle() 
        {
            string title = null;
            switch (this.SetType) 
            { 
                case ContextMessage.Success:
                    title = App_GlobalResources.Resources.exito;
                    break;

                case ContextMessage.Warning:
                    title = App_GlobalResources.Resources.advertencia;
                    break;

                case ContextMessage.Error:
                    title = App_GlobalResources.Resources.error;
                    break;

                case ContextMessage.Info:
                    title = App_GlobalResources.Resources.informacion;
                    break;

                default:
                    title = App_GlobalResources.Resources.mensaje;
                    break;
            }

            return title; // We return the appropriate title if one is not specified
        }

        public string GetContextDefaultClass() 
        {
            string className = null;
            switch (this.SetType)
            {
                case ContextMessage.Success:
                    className = "panel-success";
                    break;

                case ContextMessage.Warning:
                    className = "panel-warning";
                    break;

                case ContextMessage.Error:
                    className = "panel-danger";
                    break;

                case ContextMessage.Info:
                    className = "panel-primary";
                    break;

                default:
                    className = "panel-default";
                    break;
            }

            return className; // We return the appropriate title if one is not specified
        }

        public ContextMessage() 
        {
            this.HasFooter = true;
            this.Message = null;
            this.Type = ContextMessage.Default;
            this.Title = GetContextDefaultTitle();
        }

        public ContextMessage(String message) 
        {
            this.HasFooter = true;
            this.Message = message;
            this.Type = ContextMessage.Default;
            this.Title = GetContextDefaultTitle();
        }

        public ContextMessage(String type, String message) 
        {
            this.HasFooter = true;
            this.Message = message;
            this.Type = type;
            this.Title = GetContextDefaultTitle();
        }

        public ContextMessage(String type, String title, String message) 
        {
            this.HasFooter = true;
            this.Type = type;
            this.Title = title;
            this.Message = message;
        }
    }
}