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
    
    public partial class fn_resumen_remesas_salientes_jornada_Result
    {
        public string nombre_banco { get; set; }
        public string nombre_cliente { get; set; }
        public string nombre_punto { get; set; }
        public string id_punto_externo { get; set; }
        public string simbolo_moneda { get; set; }
        public Nullable<int> id_banco { get; set; }
        public Nullable<int> id_cliente { get; set; }
        public Nullable<int> id_punto { get; set; }
        public string moneda { get; set; }
        public Nullable<decimal> requerido { get; set; }
        public Nullable<decimal> obtenido { get; set; }
        public Nullable<int> id_moneda { get; set; }
        public Nullable<int> cantidad_paquetes { get; set; }
    }
}