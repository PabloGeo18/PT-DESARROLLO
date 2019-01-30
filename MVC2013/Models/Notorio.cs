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
    
    public partial class Notorio
    {
        public Notorio()
        {
            this.Empresa_Notorio = new HashSet<Empresa_Notorio>();
        }
    
        public int id_notorio { get; set; }
        public string primer_nombre { get; set; }
        public string primer_apellido { get; set; }
        public string segundo_nombre { get; set; }
        public string segundo_apellido { get; set; }
        public long numero_registro_mercantil { get; set; }
        public int folio { get; set; }
        public int libro { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public int id_genero { get; set; }
    
        public virtual ICollection<Empresa_Notorio> Empresa_Notorio { get; set; }
        public virtual Genero Genero { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
    }
}
