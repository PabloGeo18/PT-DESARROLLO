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
    
    public partial class Tickets_Tipo
    {
        public Tickets_Tipo()
        {
            this.Tickets_Generacion = new HashSet<Tickets_Generacion>();
            this.Tickets_Movimiento = new HashSet<Tickets_Movimiento>();
        }
    
        public int id_ticket_tipo { get; set; }
        public string nombre { get; set; }
        public bool es_detallado { get; set; }
        public decimal valor { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
    
        public virtual ICollection<Tickets_Generacion> Tickets_Generacion { get; set; }
        public virtual ICollection<Tickets_Movimiento> Tickets_Movimiento { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
    }
}
