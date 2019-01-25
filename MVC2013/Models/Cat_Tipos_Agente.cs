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
    
    public partial class Cat_Tipos_Agente
    {
        public Cat_Tipos_Agente()
        {
            this.Estado_Fuerza_Detalle = new HashSet<Estado_Fuerza_Detalle>();
            this.Contratos_Agentes = new HashSet<Contratos_Agentes>();
            this.Facturas_Detalle = new HashSet<Facturas_Detalle>();
            this.Procesos_Facturacion_Detalle = new HashSet<Procesos_Facturacion_Detalle>();
        }
    
        public int id_cat_tipo_agente { get; set; }
        public string nombre { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
    
        public virtual ICollection<Estado_Fuerza_Detalle> Estado_Fuerza_Detalle { get; set; }
        public virtual ICollection<Contratos_Agentes> Contratos_Agentes { get; set; }
        public virtual ICollection<Facturas_Detalle> Facturas_Detalle { get; set; }
        public virtual ICollection<Procesos_Facturacion_Detalle> Procesos_Facturacion_Detalle { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
    }
}
