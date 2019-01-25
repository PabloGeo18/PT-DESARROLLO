using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC2013.Models;

namespace MVC2013.Areas.rrhh.Models
{
    public class EmpleadoViewModel
    {
        public Empleado empleado { get; set; }
        public Empleado_Telefono telefono_movil { get; set; }
        public Empleado_Telefono telefono_residencial { get; set; }
        public Empleado_Direcciones direccion_residencial { get; set; }
        public Empleado_Direcciones direccion_nacimiento { get; set; }
    }
}