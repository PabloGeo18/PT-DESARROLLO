using MVC2013.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Comun.Util
{
    public class Constantes
    {

        public static string getDataSource(){
            
            string currentUser = HttpContext.Current.User.Identity.Name;
            if (Cache.DiccionarioUsuariosLogueados.ContainsKey(currentUser))
                return Cache.DiccionarioUsuariosLogueados[currentUser].EmpresaDS;
            else
                return "";
        }

        public static int getPagerSize()
        {
            string pagerSizeStr = ConfigurationManager.AppSettings["PagerSize"].ToString();
            int pagerSize = (String.IsNullOrEmpty(pagerSizeStr) ? 15 : int.Parse(pagerSizeStr.Trim()));
            return pagerSize;
        }



        public static string getReportesUrl(){
            return ConfigurationManager.AppSettings["ReportesUrl"].ToString();
        }

        public static string getReportesUsr()
        {
            return ConfigurationManager.AppSettings["ReportesUsr"].ToString();
        }

        public static string getReportesPwd()
        {
            return ConfigurationManager.AppSettings["ReportesPwd"].ToString();
        }

        public static string getReportesDom()
        {
            return ConfigurationManager.AppSettings["ReportesDom"].ToString();
        }

       

    }
}