using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Net;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using MVC2013.Src.Comun.Util;
using System.Web.Mvc;

namespace MVC2013.Src.Rpt
{
    public partial class Report : System.Web.UI.Page
    {
        private string cadenaConexionPT = System.Configuration.ConfigurationManager.ConnectionStrings["proteccion_total"].ToString();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                
                if (MVC2013.Src.Comun.Util.Cache.DiccionarioUsuariosLogueados.Count(x => x.Value.SessionId == HttpContext.Current.Session.SessionID)>0) { 
                
                if (Request["f"] == null)
                {
                    // Set the processing mode for the ReportViewer to Remote
                    ReportViewer1.ProcessingMode = ProcessingMode.Remote;
                    //String folder = Request["f"];
                    String rptId = Request["r"];

                    String sqlSTR = "select reporte, carpeta from adm.Reportes rpt join adm.Reporte_Carpeta rptC on rpt.id_reporte_carpeta = rptC.id_reporte_carpeta where id_reporte =" + rptId;

                    DataSet dataS = new DataSet();
                    dataS = SqlSelectMSSQL(cadenaConexionPT, sqlSTR, "rpt");

                    if (dataS.Tables[0].Rows.Count > 0)
                    {

                        String folder = dataS.Tables[0].Rows[0][1].ToString();
                        String rptName = dataS.Tables[0].Rows[0][0].ToString();
                        ServerReport serverReport = ReportViewer1.ServerReport;
                        string dominio = ConfigurationManager.AppSettings["dominio"];
                        string usuarioDominio = ConfigurationManager.AppSettings["usuarioDominio"];
                        string passwordUsuarioDominio = ConfigurationManager.AppSettings["passwordUsuarioDominio"];
                        string servidorRPT = ConfigurationManager.AppSettings["servidorRpt"];

                        //serverReport.ReportServerCredentials = new CustomReportCredentials("administrador", "distclase1990*", "orion");
                        serverReport.ReportServerCredentials = new CustomReportCredentials(usuarioDominio, passwordUsuarioDominio, dominio);

                        // Set the report server URL and report path
                        serverReport.ReportServerUrl = new Uri("http://" + servidorRPT + "/reportserver");
                        serverReport.ReportPath = String.Format("/{0}/{1}", folder, rptName);

                    }

                }
                else if (Request["f"] != null)
                {
                    // Set the processing mode for the ReportViewer to Remote
                    ReportViewer1.ProcessingMode = ProcessingMode.Remote;
                    String folder = Request["f"];
                    String rptName = Request["r"];
                    ServerReport serverReport = ReportViewer1.ServerReport;
                    string dominio = ConfigurationManager.AppSettings["dominio"];
                    string usuarioDominio = ConfigurationManager.AppSettings["usuarioDominio"];
                    string passwordUsuarioDominio = ConfigurationManager.AppSettings["passwordUsuarioDominio"];
                    string servidorRPT = ConfigurationManager.AppSettings["servidorRpt"];

                    //serverReport.ReportServerCredentials = new CustomReportCredentials("administrador", "distclase1990*", "orion");
                    serverReport.ReportServerCredentials = new CustomReportCredentials(usuarioDominio, passwordUsuarioDominio, dominio);

                    // Set the report server URL and report path
                    serverReport.ReportServerUrl = new Uri("http://" + servidorRPT + "/reportserver");
                    serverReport.ReportPath = String.Format("/{0}/{1}", folder, rptName);
                    //Response.Write(serverReport.ReportServerUrl);

                    ReportViewer1.ShowPrintButton = true;
                    ReportViewer1.ShowRefreshButton = true;
                    ReportViewer1.ShowZoomControl = true;
                    ReportViewer1.ShowToolBar = true;

                    // Create the sales order number report parameter
                    //ReportParameter parametro_id_jornada = new ReportParameter();
                    //parametro_id_jornada.Name = "id_jornada";
                    //parametro_id_jornada.Values.Add("7");

                    //// Set the report parameters for the report
                    //if (ReportViewer1.ServerReport.GetParameters().Count(p => p.Name == "id_jornada") > 0)
                    //{
                    //    ReportViewer1.ServerReport.SetParameters(
                    //    new ReportParameter[] { parametro_id_jornada });
                    //}


                    //// Create the sales order number report parameter
                    //ReportParameter parametro_id_usuario = new ReportParameter();
                    //parametro_id_usuario.Name = "id_usuario";
                    //parametro_id_usuario.Values.Add("7");


                    //if (ReportViewer1.ServerReport.GetParameters().Count(p=>p.Name=="id_usuario")>0) {
                    //    ReportViewer1.ServerReport.SetParameters(
                    //    new ReportParameter[] { parametro_id_usuario });
                    //}
                    // Set the report parameters for the report


                }
                } else {
                    Response.Redirect("../../Home/Index");
                    //return RedirectToAction("Index", "Home");
                    //var page = HttpContext.Current.Handler as Page;
                    //Response.Redirect(page.GetRouteUrl("Home/Index",
                    //    new { Controller = "Home", Action = "Index" }), false);
                }
            }
        }

        public DataSet SqlSelectMSSQL(string connexionString, string commandoString, string NombreTablaString)
        {
            SqlConnection dbConnection = new SqlConnection();
            //Dim dbDataSet As DataSet
            try
            {
                dbConnection = new SqlConnection(connexionString);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
            dbConnection.Open();
            DataSet dbDataSet = new DataSet();
            SqlDataAdapter dbDataAdapter = new SqlDataAdapter(commandoString, dbConnection);

            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dbDataAdapter);
            //dbDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
            try
            {
                dbDataAdapter.Fill(dbDataSet, NombreTablaString);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                if (dbConnection != null && dbConnection.State != ConnectionState.Closed)
                {
                    dbConnection.Close();
                }
            }
            return dbDataSet;
        }
    }
}