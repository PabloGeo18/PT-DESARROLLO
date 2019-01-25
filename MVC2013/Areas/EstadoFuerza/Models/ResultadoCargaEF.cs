using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC2013.Models;

namespace MVC2013.Areas.EstadoFuerza.Models
{
    public class ResultadoCargaEF
    {
        public string id_empleado { get; set; }
        public string id_situacion { get; set; }
        public string id_cat_tipo_agente { get; set; }
        public string observacion { get; set; }
        public string id_ubicacion { get; set; }
        public bool correcto { get; set; }
        public string error { get; set; }
    }
}