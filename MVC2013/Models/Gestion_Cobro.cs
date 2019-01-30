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
    
    public partial class Gestion_Cobro
    {
        public Gestion_Cobro()
        {
            this.Gestion_Cobro_Detalle = new HashSet<Gestion_Cobro_Detalle>();
        }
    
        public int id_gestion_cobro { get; set; }
        public int id_razon_social { get; set; }
        public int id_proceso_facturacion { get; set; }
        public System.DateTime fecha_inicio { get; set; }
        public Nullable<System.DateTime> fecha_fin { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
    
        public virtual Razones_Sociales Razones_Sociales { get; set; }
        public virtual ICollection<Gestion_Cobro_Detalle> Gestion_Cobro_Detalle { get; set; }
        public virtual Procesos_Facturacion Procesos_Facturacion { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
    }
}
