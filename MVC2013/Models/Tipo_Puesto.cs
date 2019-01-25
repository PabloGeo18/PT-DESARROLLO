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
    
    public partial class Tipo_Puesto
    {
        public Tipo_Puesto()
        {
            this.Puesto = new HashSet<Puesto>();
        }
    
        public int id_tipo_puesto { get; set; }
        public string nombre { get; set; }
        public bool genera_estado_fuerza { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_empresa { get; set; }
        public decimal salario_minimo { get; set; }
        public decimal salario_maximo { get; set; }
    
        public virtual ICollection<Puesto> Puesto { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
    }
}
