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
    
    public partial class Empleado
    {
        public Empleado()
        {
            this.Estado_Fuerza_Detalle = new HashSet<Estado_Fuerza_Detalle>();
            this.Tickets_Movimiento = new HashSet<Tickets_Movimiento>();
            this.Aprendiz = new HashSet<Aprendiz>();
            this.Archivo_Empleado = new HashSet<Archivo_Empleado>();
            this.Asistencias_Extras_Empleado = new HashSet<Asistencias_Extras_Empleado>();
            this.Contratacion = new HashSet<Contratacion>();
            this.Curso_Empleado = new HashSet<Curso_Empleado>();
            this.Descuento_Periodico = new HashSet<Descuento_Periodico>();
            this.Empleado_Encabezado_Planilla = new HashSet<Empleado_Encabezado_Planilla>();
            this.Empleado_Encabezado_Planilla1 = new HashSet<Empleado_Encabezado_Planilla>();
            this.Empleado_Permisos_Ausencias = new HashSet<Empleado_Permisos_Ausencias>();
            this.Empleado_Direcciones = new HashSet<Empleado_Direcciones>();
            this.Empleado_Telefono = new HashSet<Empleado_Telefono>();
            this.Liquidaciones = new HashSet<Liquidaciones>();
            this.Bonificacion = new HashSet<Bonificacion>();
            this.Descuento = new HashSet<Descuento>();
            this.Planilla = new HashSet<Planilla>();
            this.Salario = new HashSet<Salario>();
            this.Vacacion_Contrato = new HashSet<Vacacion_Contrato>();
            this.Vacacion_Detalle = new HashSet<Vacacion_Detalle>();
            this.Empleado_Experiencia_Laboral = new HashSet<Empleado_Experiencia_Laboral>();
            this.Armas = new HashSet<Armas>();
            this.Bodega_Inventario_Municiones = new HashSet<Bodega_Inventario_Municiones>();
            this.Egreso_Detalle = new HashSet<Egreso_Detalle>();
        }
    
        public int id_empleado { get; set; }
        public string primer_nombre { get; set; }
        public string segundo_nombre { get; set; }
        public string primer_apellido { get; set; }
        public string segundo_apellido { get; set; }
        public string apellido_casada { get; set; }
        public string correo { get; set; }
        public bool servicio_militar { get; set; }
        public System.DateTime fecha_nacimiento { get; set; }
        public bool sabe_leer { get; set; }
        public bool sabe_escribir { get; set; }
        public int estatura { get; set; }
        public Nullable<int> peso { get; set; }
        public int id_estado_civil { get; set; }
        public int id_grado_estudio { get; set; }
        public int id_profesion { get; set; }
        public long dpi { get; set; }
        public int id_municipio_dpi { get; set; }
        public Nullable<int> id_tipo_licencia { get; set; }
        public Nullable<long> numero_licencia { get; set; }
        public Nullable<System.DateTime> fecha_vencimiento_licencia { get; set; }
        public string cedula { get; set; }
        public string nit { get; set; }
        public Nullable<long> afiliacion_igss { get; set; }
        public Nullable<long> afiliacion_irtra { get; set; }
        public Nullable<int> id_municipio_cedula { get; set; }
        public int id_pais_nacionalidad { get; set; }
        public string pasaporte { get; set; }
        public Nullable<int> id_tipo_cuenta { get; set; }
        public string numero_cuenta { get; set; }
        public Nullable<int> id_banco_cuenta { get; set; }
        public int id_estado_empleado { get; set; }
        public bool activo { get; set; }
        public bool eliminado { get; set; }
        public int id_usuario_creacion { get; set; }
        public Nullable<int> id_usuario_modificacion { get; set; }
        public Nullable<int> id_usuario_eliminacion { get; set; }
        public System.DateTime fecha_creacion { get; set; }
        public Nullable<System.DateTime> fecha_modificacion { get; set; }
        public Nullable<System.DateTime> fecha_eliminacion { get; set; }
        public bool acreditamiento { get; set; }
        public Nullable<int> id_genero { get; set; }
        public string referenciado_por { get; set; }
        public string comentario { get; set; }
        public Nullable<int> id_tipo_pago { get; set; }
        public Nullable<int> digeesp { get; set; }
    
        public virtual Banco Banco { get; set; }
        public virtual Estado_Civil Estado_Civil { get; set; }
        public virtual Genero Genero { get; set; }
        public virtual Municipios Municipios { get; set; }
        public virtual Municipios Municipios1 { get; set; }
        public virtual Paises Paises { get; set; }
        public virtual Profesion Profesion { get; set; }
        public virtual Tipo_Cuenta Tipo_Cuenta { get; set; }
        public virtual Tipo_Licencia Tipo_Licencia { get; set; }
        public virtual ICollection<Estado_Fuerza_Detalle> Estado_Fuerza_Detalle { get; set; }
        public virtual ICollection<Tickets_Movimiento> Tickets_Movimiento { get; set; }
        public virtual ICollection<Aprendiz> Aprendiz { get; set; }
        public virtual ICollection<Archivo_Empleado> Archivo_Empleado { get; set; }
        public virtual ICollection<Asistencias_Extras_Empleado> Asistencias_Extras_Empleado { get; set; }
        public virtual ICollection<Contratacion> Contratacion { get; set; }
        public virtual ICollection<Curso_Empleado> Curso_Empleado { get; set; }
        public virtual ICollection<Descuento_Periodico> Descuento_Periodico { get; set; }
        public virtual ICollection<Empleado_Encabezado_Planilla> Empleado_Encabezado_Planilla { get; set; }
        public virtual ICollection<Empleado_Encabezado_Planilla> Empleado_Encabezado_Planilla1 { get; set; }
        public virtual ICollection<Empleado_Permisos_Ausencias> Empleado_Permisos_Ausencias { get; set; }
        public virtual Grado_Estudio Grado_Estudio { get; set; }
        public virtual Tipo_Pago Tipo_Pago { get; set; }
        public virtual ICollection<Empleado_Direcciones> Empleado_Direcciones { get; set; }
        public virtual Estado_Empleado Estado_Empleado { get; set; }
        public virtual ICollection<Empleado_Telefono> Empleado_Telefono { get; set; }
        public virtual ICollection<Liquidaciones> Liquidaciones { get; set; }
        public virtual ICollection<Bonificacion> Bonificacion { get; set; }
        public virtual ICollection<Descuento> Descuento { get; set; }
        public virtual ICollection<Planilla> Planilla { get; set; }
        public virtual ICollection<Salario> Salario { get; set; }
        public virtual ICollection<Vacacion_Contrato> Vacacion_Contrato { get; set; }
        public virtual ICollection<Vacacion_Detalle> Vacacion_Detalle { get; set; }
        public virtual ICollection<Empleado_Experiencia_Laboral> Empleado_Experiencia_Laboral { get; set; }
        public virtual Usuarios Usuarios { get; set; }
        public virtual Usuarios Usuarios1 { get; set; }
        public virtual Usuarios Usuarios2 { get; set; }
        public virtual ICollection<Armas> Armas { get; set; }
        public virtual ICollection<Bodega_Inventario_Municiones> Bodega_Inventario_Municiones { get; set; }
        public virtual ICollection<Egreso_Detalle> Egreso_Detalle { get; set; }
    }
}
