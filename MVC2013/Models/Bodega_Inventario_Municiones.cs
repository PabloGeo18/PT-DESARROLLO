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
    
    public partial class Bodega_Inventario_Municiones
    {
        public int id_bodega_inventario_municiones { get; set; }
        public int id_municion { get; set; }
        public int id_bodega { get; set; }
        public int existencia { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public Nullable<int> comprometido { get; set; }
        public bool autorizada { get; set; }
        public Nullable<int> id_empleado { get; set; }
        public Nullable<int> id_egreso_detalle { get; set; }
        public bool debitado { get; set; }
        public int cantidad_debito { get; set; }
        public Nullable<int> id_cliente { get; set; }
        public Nullable<int> retornando { get; set; }
    
        public virtual Bodegas Bodegas { get; set; }
        public virtual Municiones Municiones { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
        public virtual Egreso_Detalle Egreso_Detalle { get; set; }
        public virtual Empleado Empleado { get; set; }
    }
}
