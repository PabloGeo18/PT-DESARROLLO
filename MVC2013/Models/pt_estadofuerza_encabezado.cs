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
    
    public partial class pt_estadofuerza_encabezado
    {
        public pt_estadofuerza_encabezado()
        {
            this.pt_estadofuerza = new HashSet<pt_estadofuerza>();
        }
    
        public int id { get; set; }
        public System.DateTime fecha_inicio { get; set; }
        public Nullable<System.DateTime> fecha_fin { get; set; }
        public Nullable<System.DateTime> creado_el { get; set; }
        public string creado_por { get; set; }
        public Nullable<int> estado { get; set; }
        public string modificado_por { get; set; }
        public Nullable<System.DateTime> modificado_el { get; set; }
    
        public virtual ICollection<pt_estadofuerza> pt_estadofuerza { get; set; }
    }
}