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
    
    public partial class Inventario_Tipo
    {
        public Inventario_Tipo()
        {
            this.Marca_Tipo = new HashSet<Marca_Tipo>();
        }
    
        public int id_inventario_tipo { get; set; }
        public string descripcion { get; set; }
    
        public virtual ICollection<Marca_Tipo> Marca_Tipo { get; set; }
    }
}