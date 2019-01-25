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
    
    public partial class Pt_Puestos
    {
        public Pt_Puestos()
        {
            this.Pt_Puesto_Insumos = new HashSet<Pt_Puesto_Insumos>();
            this.Pt_Pagos_Puesto = new HashSet<Pt_Pagos_Puesto>();
            this.Pt_Tmp_Propuesta_Fase_Puesto = new HashSet<Pt_Tmp_Propuesta_Fase_Puesto>();
        }
    
        public int cpue_id { get; set; }
        public string cpue_descripcion { get; set; }
        public int id_usuario_creacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
    
        public virtual ICollection<Pt_Puesto_Insumos> Pt_Puesto_Insumos { get; set; }
        public virtual ICollection<Pt_Pagos_Puesto> Pt_Pagos_Puesto { get; set; }
        public virtual ICollection<Pt_Tmp_Propuesta_Fase_Puesto> Pt_Tmp_Propuesta_Fase_Puesto { get; set; }
    }
}
