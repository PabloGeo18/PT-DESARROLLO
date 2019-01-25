using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC2013.Models;

namespace MVC2013.Areas.rrhh.Models
{
    public class Finiquito
    {
        public rpt_Finiquito_Doc_Result finiquito { get; set; }
        public decimal indemnizacion { get; set; }
        public decimal vacaciones { get; set; }
        public decimal aguinaldo { get; set; }
        public decimal bono_14 { get; set; }
        public decimal sueldos_pendientes { get; set; }
        public decimal deducciones { get; set; }
        public decimal total { get; set; }
    }
}