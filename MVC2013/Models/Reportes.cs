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
    
    public partial class Reportes
    {
        public Reportes()
        {
            this.Reporte_Rol = new HashSet<Reporte_Rol>();
        }
    
        public int id_reporte { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string reporte { get; set; }
        public Nullable<int> id_reporte_carpeta { get; set; }
        public Nullable<int> id_reporte_grupo { get; set; }
        public string url { get; set; }
    
        public virtual Reporte_Carpeta Reporte_Carpeta { get; set; }
        public virtual ICollection<Reporte_Rol> Reporte_Rol { get; set; }
        public virtual Reporte_Grupo Reporte_Grupo { get; set; }
    }
}
