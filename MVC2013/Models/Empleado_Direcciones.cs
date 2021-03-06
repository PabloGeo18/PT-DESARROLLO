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
    
    public partial class Empleado_Direcciones
    {
        public int id_empleado_direcciones { get; set; }
        public int id_empleado { get; set; }
        public int id_tipo_direccion { get; set; }
        public string calle { get; set; }
        public string avenida { get; set; }
        public string no_casa { get; set; }
        public string local { get; set; }
        public Nullable<int> zona { get; set; }
        public string colonia_cc_edificio { get; set; }
        public int id_municipio { get; set; }
        public string comentario { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public string kilometro { get; set; }
        public string direccion { get; set; }
    
        public virtual Municipios Municipios { get; set; }
        public virtual Tipo_Direccion Tipo_Direccion { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
    }
}
