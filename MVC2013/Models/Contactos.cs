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
    
    public partial class Contactos
    {
        public Contactos()
        {
            this.Razon_Social_Contacto = new HashSet<Razon_Social_Contacto>();
        }
    
        public int id_contacto { get; set; }
        public string nombre { get; set; }
        public int id_contacto_puesto { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public string comentario { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
    
        public virtual Contacto_Puesto Contacto_Puesto { get; set; }
        public virtual ICollection<Razon_Social_Contacto> Razon_Social_Contacto { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
    }
}
