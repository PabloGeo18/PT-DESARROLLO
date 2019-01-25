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
    
    public partial class Empleado_Encabezado_Planilla
    {
        public Empleado_Encabezado_Planilla()
        {
            this.Bonificacion = new HashSet<Bonificacion>();
            this.Descuento = new HashSet<Descuento>();
            this.Planilla = new HashSet<Planilla>();
        }
    
        public int id_empleado_encabezado_planilla { get; set; }
        public int id_empleado { get; set; }
        public int id_encabezado_planilla { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public int id_contratacion { get; set; }
        public int dias_trabajados { get; set; }
        public Nullable<int> id_banco { get; set; }
        public Nullable<int> id_tipo_pago { get; set; }
        public Nullable<int> id_ubicacion { get; set; }
        public Nullable<int> agencia { get; set; }
        public Nullable<long> cuenta { get; set; }
        public Nullable<int> numero { get; set; }
        public Nullable<int> correlativo_boleta { get; set; }
    
        public virtual Banco Banco { get; set; }
        public virtual ICollection<Bonificacion> Bonificacion { get; set; }
        public virtual Contratacion Contratacion { get; set; }
        public virtual Contratacion Contratacion1 { get; set; }
        public virtual ICollection<Descuento> Descuento { get; set; }
        public virtual Encabezado_Planilla Encabezado_Planilla { get; set; }
        public virtual Encabezado_Planilla Encabezado_Planilla1 { get; set; }
        public virtual Tipo_Pago Tipo_Pago { get; set; }
        public virtual ICollection<Planilla> Planilla { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Empleado Empleado1 { get; set; }
        public virtual Ubicaciones Ubicaciones { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
        public virtual Usuarios Usuarios3 { get; set; }
        public virtual Usuarios Usuarios4 { get; set; }
        public virtual Usuarios Usuarios5 { get; set; }
    }
}