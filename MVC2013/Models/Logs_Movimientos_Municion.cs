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
    
    public partial class Logs_Movimientos_Municion
    {
        public int id_log_mov_municion { get; set; }
        public Nullable<int> id_bodega_inventario_municion { get; set; }
        public Nullable<int> id_empleado { get; set; }
        public Nullable<int> id_bodega_origen { get; set; }
        public Nullable<int> id_bodega_destino { get; set; }
        public Nullable<int> id_tipo_transaccion { get; set; }
        public Nullable<int> id_estado_transaccion { get; set; }
        public Nullable<int> id_cliente { get; set; }
        public Nullable<int> cantidad { get; set; }
        public Nullable<System.DateTime> fecha_asignacion { get; set; }
        public Nullable<System.DateTime> fecha_creacion { get; set; }
        public Nullable<int> id_usuario_creacion { get; set; }
        public Nullable<bool> activo { get; set; }
        public Nullable<bool> eliminado { get; set; }
    }
}
