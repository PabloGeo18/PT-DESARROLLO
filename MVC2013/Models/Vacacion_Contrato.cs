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
    
    public partial class Vacacion_Contrato
    {
        public Vacacion_Contrato()
        {
            this.Vacacion_Detalle = new HashSet<Vacacion_Detalle>();
        }
    
        public int id_vacacion_contrato { get; set; }
        public int id_contratacion { get; set; }
        public int id_periodo { get; set; }
        public int dias_total { get; set; }
        public Nullable<int> dias_tomados { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public int id_empleado { get; set; }
    
        public virtual Periodo Periodo { get; set; }
        public virtual Periodo Periodo1 { get; set; }
        public virtual ICollection<Vacacion_Detalle> Vacacion_Detalle { get; set; }
        public virtual Contratacion Contratacion { get; set; }
        public virtual Contratacion Contratacion1 { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
        public virtual Usuarios Usuarios3 { get; set; }
        public virtual Usuarios Usuarios4 { get; set; }
        public virtual Usuarios Usuarios5 { get; set; }
    }
}
