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
    
    public partial class pt_situacion
    {
        public pt_situacion()
        {
            this.pt_estadofuerza = new HashSet<pt_estadofuerza>();
        }
    
        public int id_situacion { get; set; }
        public string nombre { get; set; }
        public Nullable<int> es_pago { get; set; }
        public Nullable<int> baja_rrhh { get; set; }
    
        public virtual ICollection<pt_estadofuerza> pt_estadofuerza { get; set; }
    }
}
