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
    
    public partial class Cat_Tipos_Respuesta_Gestion
    {
        public Cat_Tipos_Respuesta_Gestion()
        {
            this.Gestion_Cobro_Detalle = new HashSet<Gestion_Cobro_Detalle>();
        }
    
        public int id_cat_tipo_respuesta_gestion { get; set; }
        public string nombre { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
    
        public virtual ICollection<Gestion_Cobro_Detalle> Gestion_Cobro_Detalle { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
        public virtual Usuarios Usuarios3 { get; set; }
        public virtual Usuarios Usuarios4 { get; set; }
        public virtual Usuarios Usuarios5 { get; set; }
    }
}