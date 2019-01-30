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
    
    public partial class Tickets_Movimiento
    {
        public long id_ticket_movimiento { get; set; }
        public int id_ticket_tipo_movimiento { get; set; }
        public int id_ticket_tipo { get; set; }
        public int id_usuario { get; set; }
        public int debe { get; set; }
        public int haber { get; set; }
        public Nullable<int> id_empleado { get; set; }
        public int desde { get; set; }
        public int hasta { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
    
        public virtual Tickets_Tipo Tickets_Tipo { get; set; }
        public virtual Tickets_Tipo_Movimiento Tickets_Tipo_Movimiento { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
        public virtual Usuarios Usuarios3 { get; set; }
    }
}
