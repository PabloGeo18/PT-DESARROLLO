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
    
    public partial class Reporte_Resumen_Planilla_Result
    {
        public string empresa { get; set; }
        public string tipo_planilla { get; set; }
        public string periodo { get; set; }
        public decimal salario { get; set; }
        public decimal total_descuento { get; set; }
        public decimal bonificacion { get; set; }
        public decimal total_neto { get; set; }
        public Nullable<decimal> total_pagado { get; set; }
    }
}
