using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC2013.Models;

namespace MVC2013.Areas.rrhh.Models
{
    public class ContratoSalario
    {
        public Contratacion contratacion { get; set; }
        public Salario salario { get; set; }
        public int id_empleado { get; set; }
    }
}