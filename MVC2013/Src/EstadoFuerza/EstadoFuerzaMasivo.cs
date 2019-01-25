using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC2013.Models;

namespace MVC2013.Src.EstadoFuerza
{
    public class EstadoFuerzaMasivo
    {
        private Protal_webEntities db = new Protal_webEntities();

        public int id_empleado { get; set; }
        public int id_ubicacion { get; set; }
        public int id_tipo_ubicacion { get; set; }
        public int id_estado { get; set; }
        public System.DateTime fecha { get; set; }
        public int id_encabezado { get; set; }
        public string comentario { get; set; }
        public string usuario_creacion { get; set; }
        public bool error { get; set; }
        public string error_msg { get; set; }


        public bool valida_errores() { 
            //error_msg = "";
            //error = false;

            //if (db.pt_empleado.Count(x => x.PTEMPLEADOID == id_empleado && x.ESTADO.ToUpper() !="BAJA") == 0) {
            //    error = true;
            //    error_msg += "Empleado Incorrecto o de baja. ";
            //}

            //if (db.pt_ubicacion.Count(x => x.PTUBICACIONID == id_ubicacion && x.ESTADO.ToUpper() !="BAJA") == 0)
            //{
            //    error = true;
            //    error_msg += "Ubicacion Incorrecta o de baja. ";
            //}

            //if (db.pt_tipo_ubicacion.Count(x => x.PTTIPOID == id_tipo_ubicacion ) == 0)
            //{
            //    error = true;
            //    error_msg += "Tipo de Ubicacion Incorrecto. ";
            //}
            //if (db.pt_situacion.Count(x => x.id_situacion == id_estado) == 0)
            //{
            //    error = true;
            //    error_msg += "Estado Incorrecto. ";
            //}
            
            return error;
        }
    }

    public class Lista_EstadoFuerzaMasivo
    {
       public List<EstadoFuerzaMasivo> lista_carga = new List<EstadoFuerzaMasivo>();

     
    }
}