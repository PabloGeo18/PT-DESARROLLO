using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC2013.Models;

namespace MVC2013.Areas.rrhh.Models
{
    public class EmpleadoContrato
    {
        public Empleado empleado { get; set; }
        public Contratacion contrato { get; set; }
    }
}