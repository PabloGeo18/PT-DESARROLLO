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
    
    public partial class Reporte_Carpeta
    {
        public Reporte_Carpeta()
        {
            this.Reportes = new HashSet<Reportes>();
        }
    
        public int id_reporte_carpeta { get; set; }
        public string nombre { get; set; }
        public string carpeta { get; set; }
    
        public virtual ICollection<Reportes> Reportes { get; set; }
    }
}