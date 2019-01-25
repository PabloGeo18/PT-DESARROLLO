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
    
    public partial class Descuento_Periodico
    {
        public Descuento_Periodico()
        {
            this.Descuento = new HashSet<Descuento>();
        }
    
        public int id_descuento_periodico { get; set; }
        public int total_pagos { get; set; }
        public int pagos_actuales { get; set; }
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
        public decimal cantidad_pagado { get; set; }
        public decimal cantidad_a_pagar { get; set; }
        public int id_empleado { get; set; }
        public int id_tipo_prestamo { get; set; }
        public string descripcion { get; set; }
        public int id_tipo_plan_pago { get; set; }
    
        public virtual ICollection<Descuento> Descuento { get; set; }
        public virtual Tipo_Prestamo Tipo_Prestamo { get; set; }
        public virtual Tipo_Plan_Pago Tipo_Plan_Pago { get; set; }
        public virtual Empleado Empleado { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
    }
}
