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
    
    public partial class Municiones
    {
        public Municiones()
        {
            this.Bodega_Inventario_Municiones = new HashSet<Bodega_Inventario_Municiones>();
            this.Transaccion_Inventario_Detalle = new HashSet<Transaccion_Inventario_Detalle>();
            this.Egreso_Detalle = new HashSet<Egreso_Detalle>();
            this.Ingreso_Detalle = new HashSet<Ingreso_Detalle>();
            this.Traslado_Detalle = new HashSet<Traslado_Detalle>();
        }
    
        public int id_municion { get; set; }
        public int id_calibre { get; set; }
        public string descripcion { get; set; }
        public decimal costo { get; set; }
        public decimal costo_venta { get; set; }
        public int existencia { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public bool autorizada { get; set; }
    
        public virtual ICollection<Bodega_Inventario_Municiones> Bodega_Inventario_Municiones { get; set; }
        public virtual Calibres Calibres { get; set; }
        public virtual ICollection<Transaccion_Inventario_Detalle> Transaccion_Inventario_Detalle { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
        public virtual ICollection<Egreso_Detalle> Egreso_Detalle { get; set; }
        public virtual ICollection<Ingreso_Detalle> Ingreso_Detalle { get; set; }
        public virtual ICollection<Traslado_Detalle> Traslado_Detalle { get; set; }
    }
}
