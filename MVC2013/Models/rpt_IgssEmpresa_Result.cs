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
    
    public partial class rpt_IgssEmpresa_Result
    {
        public int id_empresa { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public int telefono { get; set; }
        public string correo { get; set; }
        public Nullable<int> codigo_dpi { get; set; }
        public Nullable<bool> prestaciones { get; set; }
        public string encabezado_boleta { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public string centro_trabajo { get; set; }
        public Nullable<int> zona { get; set; }
        public Nullable<int> fax { get; set; }
        public string contacto { get; set; }
        public Nullable<int> nit { get; set; }
        public Nullable<int> numero_patronal { get; set; }
        public string nombre_comercial { get; set; }
    }
}
