//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC2013.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Bonificacion
    {
        public int id_bonificacion { get; set; }
        public int id_tipo_bono { get; set; }
        public int cantidad { get; set; }
        public decimal total { get; set; }
        public System.DateTime fecha { get; set; }
        public int id_empleado_encabezado_planilla { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public bool automatico { get; set; }
        public int id_empleado { get; set; }
    
        public virtual Tipo_Bono Tipo_Bono { get; set; }
        public virtual Empleado_Encabezado_Planilla Empleado_Encabezado_Planilla { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
    }
}
