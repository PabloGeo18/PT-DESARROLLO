using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC2013.Models;

namespace MVC2013.Areas.rrhh.Models
{
    public class EmpleadoPlanilla
    {
        public Empleado_Encabezado_Planilla empleado_encabezado { get; set; }
        public Planilla planilla { get; set; }
        public IEnumerable<Obtener_Bonificaciones_Result> bonos { get; set; }
        public IEnumerable<Obtener_Descuentos_Result> descuentos { get; set; }
    }
}