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
    
    public partial class Encabezado_Planilla
    {
        public Encabezado_Planilla()
        {
            this.Planilla = new HashSet<Planilla>();
            this.Empleado_Encabezado_Planilla = new HashSet<Empleado_Encabezado_Planilla>();
            this.Empleado_Encabezado_Planilla1 = new HashSet<Empleado_Encabezado_Planilla>();
        }
    
        public int id_encabezado_planilla { get; set; }
        public int id_tipo_planilla { get; set; }
        public int usuario_apertura { get; set; }
        public Nullable<int> usuario_finalizacion { get; set; }
        public System.DateTime fecha_apertura { get; set; }
        public Nullable<System.DateTime> fecha_finalizacion { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public int id_empresa { get; set; }
        public Nullable<System.DateTime> fechaInicio_EstadoFuerza { get; set; }
        public Nullable<System.DateTime> fechaFin_EstadoFuerza { get; set; }
    
        public virtual Tipo_Planilla Tipo_Planilla { get; set; }
        public virtual ICollection<Planilla> Planilla { get; set; }
        public virtual ICollection<Empleado_Encabezado_Planilla> Empleado_Encabezado_Planilla { get; set; }
        public virtual ICollection<Empleado_Encabezado_Planilla> Empleado_Encabezado_Planilla1 { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
        public virtual Usuarios Usuarios3 { get; set; }
        public virtual Usuarios Usuarios4 { get; set; }
    }
}
