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
    
    public partial class Pt_Tmp_Propuesta_Fase_Puesto
    {
        public int ctpf_id { get; set; }
        public Nullable<int> ctpf_cpue_id { get; set; }
        public Nullable<int> ctpf_cfas_id { get; set; }
        public Nullable<int> ctpf_personal { get; set; }
        public int id_usuario_creacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public Nullable<decimal> ctpf_facConIVA { get; set; }
    
        public virtual Pt_Fases_Cotizacion Pt_Fases_Cotizacion { get; set; }
        public virtual Pt_Puestos Pt_Puestos { get; set; }
    }
}
