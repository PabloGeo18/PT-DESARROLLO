using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Comun.Util
{
    public class ContextStyleUtil
    {
        public static string getContextBgStyleByStatus(bool status) {
            switch (status)
            {
                case true:
                    return "success";
                case false:
                    return "danger";
                default:
                    return "";
            }
        }

        public static string getContextTextStyleByStatus(bool status) {
            switch (status) { 
                case true:
                    return "text-success";
                case false:
                    return "text-danger";
                default:
                    return "";
            }
        }

        public static string getContextStringByStatus(bool status) {
            switch (status)
            {
                case true:
                    return @App_GlobalResources.Resources.estado_activo;
                case false:
                    return @App_GlobalResources.Resources.estado_inactivo;
                default:
                    return "";
            }
        }

        public static string getTableRowContextClassByTransporteEstado(int id)
        {
            switch (id)
            {
                case (int)Catalogos.Estados_Solicitud.Recibir:
                    return "text-info info";
                case (int)Catalogos.Estados_Solicitud.Entregar:
                    return "text-warning warning";
                case (int)Catalogos.Estados_Solicitud.Entregada:
                case (int)Catalogos.Estados_Solicitud.Finalizada:
                    return "text-success success";
                case (int)Catalogos.Estados_Solicitud.Anulada:
                case (int)Catalogos.Estados_Solicitud.AnuladaOPS:
                    return "text-danger danger";
            }
            return "";
        }
    }
}