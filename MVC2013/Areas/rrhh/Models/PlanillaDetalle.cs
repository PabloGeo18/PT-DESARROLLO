using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC2013.Models;

namespace MVC2013.Areas.rrhh.Models
{
    public class PlanillaDetalle
    {
        public Encabezado_Planilla encabezado_planilla { get; set; }
        public IEnumerable<Resumen_Empleados_Planilla_Result> resumen_planilla { get; set; }
        public IEnumerable<Resumen_Empleados_Pre_Planilla_Result> resumen_pre_planilla { get; set; }
        public IEnumerable<Resumen_Planilla_Bono_Aguinaldo_Result> resumen_bono_aguinaldo { get; set; }
    }
}