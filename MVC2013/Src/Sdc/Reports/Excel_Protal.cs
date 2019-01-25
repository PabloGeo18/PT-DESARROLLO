using MVC2013.Src.Comun.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace MVC2013.Src.Sdc.Reports
{
    public class Excel_Protal
    {
        //const string constUsuario = "reportes";
        //const string constPassword = "Transporte1.";
        //const string constDominio = "bdd";
        string constUsuario = Constantes.getReportesUsr();
        string constPassword = Constantes.getReportesPwd();
        string constDominio = Constantes.getReportesDom();

        const string constParametros = "";
        const string constIDate = "";
        const string constFDate = "";
        const int constUsuarioR = 0;//"&id_Cliente=1&fecha_desde=2015-07-31&fecha_hasta=2015-07-31&mostrar_solicitudes=1";
        string constURL = Constantes.getReportesUrl();//"http://190.111.16.58/ReportServer?";
        const string constCarpeta = "Adm_PT";
        const string constCarpetaSala = "SalaDeConteo";
        const string constCarpetaTransporte = "TransporteValores";
        const string constCarpetaAdministracion = "Adminsitracion";
        const string constCarpetaATM = "ATM";
        const string constReporte = "SalaDeConteo";
        const string constFormatoPDF = "&rs:Command=Render&rs:format=PDF";
        const string constFormatoXLS = "&rs:Command=Render&rs:format=EXCEL";
        const string constFormatoWORD = "&rs:Command=Render&rs:format=WORD";

        /*
         *  1- Administracion
         *  2- TransporteDeValores
         *  3- SalaDeConteo
         *  4- ATM
         *  5- Planilla
         */
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string Dominio { get; set; }
        public string Parametros { get; set; }
        public string URL { get; set; }
        public string Carpeta { get; set; }
        public string Reporte { get; set; }
        public string Formato { get; set; }
        

        public Excel_Protal(string nombreReporte, string parametros)
        {
            Usuario = constUsuario;
            Password = constPassword;
            Dominio = constDominio;
            Parametros = parametros;
            URL = constURL;
            Carpeta = constCarpeta;
            Reporte = nombreReporte;
            Formato = constFormatoXLS;
        }

        public byte[] obtener_reporte()
        {
            string sTargetURL = this.URL + "/" + this.Carpeta + "/" + this.Reporte + "/" + this.Formato + this.Parametros;
            HttpWebRequest req =
                  (HttpWebRequest)WebRequest.Create(sTargetURL);
            //req.PreAuthenticate = true;
            req.Credentials = new System.Net.NetworkCredential(
                this.Usuario,
                this.Password,
                this.Dominio);

            HttpWebResponse HttpWResp = (HttpWebResponse)req.GetResponse();

            Stream fStream = HttpWResp.GetResponseStream();


            byte[] fileBytes = ReadFully(fStream);
            HttpWResp.Close();

            return fileBytes;

        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}