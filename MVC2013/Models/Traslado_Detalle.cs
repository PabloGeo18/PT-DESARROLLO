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
    
    public partial class Traslado_Detalle
    {
        public int id_traslado_detalle { get; set; }
        public int id_traslado { get; set; }
        public Nullable<int> id_arma { get; set; }
        public Nullable<int> id_municion { get; set; }
        public Nullable<int> id_articulo { get; set; }
        public Nullable<int> id_uniforme { get; set; }
        public Nullable<int> id_consumible { get; set; }
        public Nullable<int> cantidad { get; set; }
        public Nullable<decimal> valor { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public Nullable<int> id_bodega_inventario_municion { get; set; }
    
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
        public virtual Armas Armas { get; set; }
        public virtual Articulos Articulos { get; set; }
        public virtual Consumibles Consumibles { get; set; }
        public virtual Municiones Municiones { get; set; }
        public virtual Traslados Traslados { get; set; }
        public virtual Uniformes Uniformes { get; set; }
    }
}
