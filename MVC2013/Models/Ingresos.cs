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
    
    public partial class Ingresos
    {
        public Ingresos()
        {
            this.Ingreso_Detalle = new HashSet<Ingreso_Detalle>();
        }
    
        public int id_ingreso { get; set; }
        public int id_bodega { get; set; }
        public int id_ingreso_estado { get; set; }
        public string referencia { get; set; }
        public System.DateTime fecha_factura { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public Nullable<int> id_usuario_autorizacion { get; set; }
    
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
        public virtual Usuarios Usuarios3 { get; set; }
        public virtual Bodegas Bodegas { get; set; }
        public virtual ICollection<Ingreso_Detalle> Ingreso_Detalle { get; set; }
        public virtual Ingreso_Estado Ingreso_Estado { get; set; }
    }
}
