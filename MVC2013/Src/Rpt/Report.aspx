<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="MVC2013.Src.Rpt.Report" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Consulta de Reportes - Proteccion Total</title>

        
</head>
<body style="margin:0px 0px 0px 0px;">
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1"  AsyncPostBackTimeout="56000" runat="server">
        </asp:ScriptManager>
        <div id="rpt-container" >
        <rsweb:ReportViewer ID="ReportViewer1"  SizeToReportContent="true" Width="100%" Height="100%" runat="server">
        </rsweb:ReportViewer>
        </div>
    
    </div>
    </form>
</body>
</html>
