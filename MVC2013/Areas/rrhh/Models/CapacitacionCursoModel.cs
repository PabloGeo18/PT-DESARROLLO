using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC2013.Models;
using MVC2013.Areas.rrhh.Models;

namespace MVC2013.Areas.rrhh.Models
{
    public class CapacitacionCursoModel
    {
        public int id_academia;
        public IEnumerable<Capacitacion> capacitacion;
        public IEnumerable<Curso> curso_no_asignados;
        public IEnumerable<Curso> cursos;
        public IEnumerable<Capacitacion_Curso> capacitacion_curso;
        public IEnumerable<Capacitacion_Impartida> capacitacion_impartida;
        public List<int> no_participantes = new List<int>();
    }

    

}