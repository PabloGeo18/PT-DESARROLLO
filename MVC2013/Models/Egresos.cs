//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVC2013.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Egresos
    {
        public Egresos()
        {
            this.Egreso_Detalle = new HashSet<Egreso_Detalle>();
        }
    
        public int id_egreso { get; set; }
        public int id_egreso_estado { get; set; }
        public int id_bodega { get; set; }
        public Nullable<int> id_cliente { get; set; }
        public Nullable<int> id_usuario_autorizacion { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public int id_egreso_tipo { get; set; }
        public Nullable<int> id_bodega_destino { get; set; }
    
        public virtual Clientes Clientes { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
        public virtual Usuarios Usuarios3 { get; set; }
        public virtual Bodegas Bodegas { get; set; }
        public virtual Bodegas Bodegas1 { get; set; }
        public virtual ICollection<Egreso_Detalle> Egreso_Detalle { get; set; }
        public virtual Egreso_Estado Egreso_Estado { get; set; }
        public virtual Egreso_Tipo Egreso_Tipo { get; set; }
    }
}
