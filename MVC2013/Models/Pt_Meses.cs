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
    
    public partial class Pt_Meses
    {
        public Pt_Meses()
        {
            this.Pt_Cantidad_Planilla_Mes_Anio = new HashSet<Pt_Cantidad_Planilla_Mes_Anio>();
            this.Pt_Costos_Fijos_Mes_Anio = new HashSet<Pt_Costos_Fijos_Mes_Anio>();
        }
    
        public int cmes_id { get; set; }
        public string cmes_descripcion { get; set; }
        public int id_usuario_creacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
    
        public virtual ICollection<Pt_Cantidad_Planilla_Mes_Anio> Pt_Cantidad_Planilla_Mes_Anio { get; set; }
        public virtual ICollection<Pt_Costos_Fijos_Mes_Anio> Pt_Costos_Fijos_Mes_Anio { get; set; }
    }
}
