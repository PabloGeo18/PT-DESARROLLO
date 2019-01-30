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
    
    public partial class Razon_Social_Grupos_Factura
    {
        public Razon_Social_Grupos_Factura()
        {
            this.Contratos_Agentes = new HashSet<Contratos_Agentes>();
            this.Contratos_Otros_Servicios = new HashSet<Contratos_Otros_Servicios>();
            this.Procesos_Facturacion_Detalle = new HashSet<Procesos_Facturacion_Detalle>();
        }
    
        public int id_razon_social_grupo_factura { get; set; }
        public int id_razon_social { get; set; }
        public string nombre { get; set; }
        public int correlativo { get; set; }
        public string descripcion_factura { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
    
        public virtual ICollection<Contratos_Agentes> Contratos_Agentes { get; set; }
        public virtual ICollection<Contratos_Otros_Servicios> Contratos_Otros_Servicios { get; set; }
        public virtual Razones_Sociales Razones_Sociales { get; set; }
        public virtual ICollection<Procesos_Facturacion_Detalle> Procesos_Facturacion_Detalle { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
    }
}
