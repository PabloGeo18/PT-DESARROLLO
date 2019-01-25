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
    
    public partial class Facturas_Detalle
    {
        public int id_factura_detalle { get; set; }
        public int id_factura { get; set; }
        public Nullable<int> id_contrato_agente { get; set; }
        public Nullable<int> id_contrato_otro_servicio { get; set; }
        public string descripcion { get; set; }
        public int cantidad { get; set; }
        public decimal costo { get; set; }
        public decimal precio_venta_unitario { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public Nullable<int> id_cat_tipo_agente { get; set; }
        public Nullable<int> id_cat_otro_servicio { get; set; }
        public Nullable<int> id_proceso_facturacion_detalle { get; set; }
    
        public virtual Contratos_Agentes Contratos_Agentes { get; set; }
        public virtual Contratos_Otros_Servicios Contratos_Otros_Servicios { get; set; }
        public virtual Cat_Otros_Servicios Cat_Otros_Servicios { get; set; }
        public virtual Cat_Tipos_Agente Cat_Tipos_Agente { get; set; }
        public virtual Facturas Facturas { get; set; }
        public virtual Procesos_Facturacion_Detalle Procesos_Facturacion_Detalle { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
    }
}
