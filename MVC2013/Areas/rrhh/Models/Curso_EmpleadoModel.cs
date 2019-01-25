using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC2013.Models;

namespace MVC2013.Areas.rrhh.Models
{
    public class Curso_EmpleadoModel
    {
        public Capacitacion_Impartida capacitacion_impartida;
        public List<object> empleados = new List<object>();
        public int id_academia;

    }
}