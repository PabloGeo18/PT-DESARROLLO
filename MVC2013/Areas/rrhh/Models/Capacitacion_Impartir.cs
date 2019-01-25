using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC2013.Models;

namespace MVC2013.Areas.rrhh.Models
{
    public class Capacitacion_Impartir
    {
        public IEnumerable<Curso> cursos;
        public List<Instructor> instructor;
        public int id_academia;
    }
}