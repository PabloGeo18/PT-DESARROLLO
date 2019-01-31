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
    
    public partial class Armas
    {
        public Armas()
        {
            this.Transaccion_Inventario_Detalle = new HashSet<Transaccion_Inventario_Detalle>();
            this.Egreso_Detalle = new HashSet<Egreso_Detalle>();
            this.Ingreso_Detalle = new HashSet<Ingreso_Detalle>();
            this.Traslado_Detalle = new HashSet<Traslado_Detalle>();
        }
    
        public int id_arma { get; set; }
        public int id_arma_estado { get; set; }
        public int id_arma_estado_policia { get; set; }
        public int id_arma_tipo { get; set; }
        public int id_proveedor { get; set; }
        public int id_marca { get; set; }
        public int id_calibre { get; set; }
        public int cantidad_municion { get; set; }
        public string registro { get; set; }
        public int tenencia { get; set; }
        public int portacion { get; set; }
        public Nullable<int> id_cliente { get; set; }
        public Nullable<int> id_ubicacion { get; set; }
        public Nullable<int> id_bodega { get; set; }
        public decimal valor { get; set; }
        public string marcage { get; set; }
        public int id_modelo { get; set; }
        public Nullable<int> huella { get; set; }
        public string largo { get; set; }
        public System.DateTime fecha_vencimiento { get; set; }
        public Nullable<System.DateTime> fecha_robo { get; set; }
        public string expediente { get; set; }
        public string responsable { get; set; }
        public string observaciones { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public bool autorizada { get; set; }
        public bool trans_autorizada { get; set; }
        public bool comprometida { get; set; }
        public bool trans_cliente { get; set; }
        public bool autorizacion_cliente { get; set; }
        public bool proceso_retorno { get; set; }
        public Nullable<int> id_empleado { get; set; }
        public int id_estado_tipo { get; set; }
    
        public virtual Clientes Clientes { get; set; }
        public virtual Arma_Estado Arma_Estado { get; set; }
        public virtual Arma_Estado_Policia Arma_Estado_Policia { get; set; }
        public virtual Arma_Tipo Arma_Tipo { get; set; }
        public virtual Bodegas Bodegas { get; set; }
        public virtual Calibres Calibres { get; set; }
        public virtual Marcas Marcas { get; set; }
        public virtual Modelos Modelos { get; set; }
        public virtual Proveedores Proveedores { get; set; }
        public virtual ICollection<Transaccion_Inventario_Detalle> Transaccion_Inventario_Detalle { get; set; }
        public virtual Ubicaciones Ubicaciones { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual ICollection<Egreso_Detalle> Egreso_Detalle { get; set; }
        public virtual ICollection<Ingreso_Detalle> Ingreso_Detalle { get; set; }
        public virtual ICollection<Traslado_Detalle> Traslado_Detalle { get; set; }
        public virtual Estado_Tipo Estado_Tipo { get; set; }
    }
}
