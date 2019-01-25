using MVC2013.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Comun.Util
{
    public class Catalogos
    {

        #region Administracion

        public enum Tipo_Archivo
        {
            Foto_Perfil = 1,
            Notas = 14
        }

        public enum Estado_Contrato_Servicio
        {
            creada = 1,
            Activo = 2,
            Facturado = 3,
            Finalizado = 4,
            Cancelado = 5
        }

        public enum Estado_Factura
        {
            creada = 1,
            En_Cobro = 2,
            Pagada = 3,
            Anulada = 4,
            Eliminada = 5
        }

        public enum Dias_Semana
        {
            Domingo = 1,
            Lunes = 2,
            Martes = 3,
            Miercoles = 4,
            Jueves = 5,
            Viernes = 6,
            Sabado = 7
        };

        public enum Estado_Proceso_Facturacion_Detalle
        {
            Activo = 1,
            Facturado = 2,
            Pagado = 3
        }
        #endregion

        #region Planilla
        public static double Porcentaje_Igss = 0.0483;

        public enum Tipo_Planilla
        {
            Primera_Quincena = 1,
            Segunda_Quincena = 2,
            Bono_14 = 3,
            Aguinaldo = 4
        }

        public enum Banco
        {
            GyT = 1
        }

        public enum Tipo_Pago
        {
            Acreditacion = 3,
            Cheque = 1,
            Efectivo = 2
        }

        public enum Tipo_Bono
        {
            Hora_Extra = 1,
            Bono_Decreto = 2,
            Bono_Extra = 3
        }

        public enum Tipo_Descuento
        {
            Igss = 1,
            Prestamo = 3,
        }

        public enum Estado_Empleado
        {
            Alta = 1,
            Baja = 2
        }
        #endregion

        #region Estado de Fuerza

        public enum Motivo_Baja
        {
            Renuncia = 1,
            Despido = 2,
            Abandono = 3,
            PeticionCliente = 4,
            Fallecimiento = 5,
            Ausencias = 6,
        }

        public enum Ubicacion
        {
            Central = 1155
        }

        public enum Situacion
        {
            Presente = 1,
            Descanso = 2,
            Vacaciones = 3,
            Permiso_con_pago = 4,
            Permiso_sin_pago = 5,
            Ausente = 6,
            Suspendido = 7,
            Autorizacion_Especial = 8,
            Imposibilitado_Asistencia = 9
        }

        #endregion

        public enum Traslado_Estado
        {
            Configurando = 1,
            Pendiente = 2,
            Procesado = 3,
            Rechazado = 4,
            Aceptado = 5
        }

        public enum Estados_Solicitud
        {
            Nueva = 1,
            Recibir = 2,
            Entregar = 3,
            Entregada = 4,
            Finalizada = 5,
            Anulada = 6,
            AnuladaOPS = 7
        };

        public enum Estados_Asignacion
        {
            Asignada = 1,
            Reasignando = 2,
            Reasignada = 3
        };

        public enum Tipos_Paquete
        {
            Detallado = 1,
            Agrupado = 2
        };

        public enum Tipos_Contenido
        {
            Monetario = 1,
            Documento = 2,
            Bin = 3
        };

        public enum Tipos_Cliente
        {
            Banco = 1
        };

        public enum Sdc_Paquete_Contenidos_Tipo
        {
            Efectivo = 1,
            Cheque_Ajeno = 2,
            Cheque_Propio = 3,
            Boleta_Deposito = 4,
            Documentos_Monetarios = 5
        };

        public enum Sdc_Cuenta_Tipo
        {
            Efectivo = 1,
            Cheque = 2,
            Boleta = 3,
            Insumos = 4,
            Paquete = 5,
            Bines = 6,
            Descuadre = 7,
            Remesas = 8,
            Documentos_Monetarios = 9
        }

        public enum Sdc_Paquete_Procesamiento_Estado
        {
            En_Proceso = 1,
            Revision_Descuadre = 2,
            Procesado = 3
        };

        public enum Sdc_Paquete_Detalle_Estado
        {
            Procesar = 1,
            Procesado = 2,
            Disponible = 3,
            En_Traslado = 4
        };

        public enum Sdc_Cat_Operacion_Tipo
        {
            Recoleccion_entrante = 1,
            Remesa_entrante = 2,
            Remanente_entrante = 3,
            Resguardo_entrante = 4,
            Remesa_saliente = 5,
            Planilla_sencillo_saliente = 6,
            Abastecimiento_saliente = 7
        };

        public enum Sdc_Paquete_Estado
        {
            Procesar = 1,
            Resguardo = 2,
            Transito = 3,
            EnTraslado = 4,
            Procesado = 5,
            Disponible = 6,
            EnProcesamiento = 7,
            EnCreacion = 8,
            EnRemesaSaliente = 9,
            Entregado = 10,
            EnCreacionGrupo = 11
        };

        public enum Sdc_Paquete_Tipo
        {
            Bolsa = 1,
            Bin = 2,
            GrupoBolsas = 3,
            GrupoBines = 4,
            ConfiguracionRemesa = 5,
            Recepcion = 6
        };

        public enum Sdc_Bin_Estado
        {
            Vacio = 1,
            ConValores = 2,
            Desechado = 3,
            EnTraslado = 4,
            EnReparacion = 5,
            UbicacionExterna = 6
        };

        public enum Sdc_Insumo_Clasificacion
        {
            Bolsa = 1,
            Marchamo = 2,
            Sticker = 3
        };

        public enum Sdc_Traslado_Grupo_Elementos
        {
            Insumos = 1,
            Bines = 2,
            Paquetes = 3,
            Valores_Monetarios = 4
        }

        public enum Sdc_Traslado_Tipos_Elemento
        {
            Insumos = 1,
            Bines = 2,
            Paquetes = 3,
            Efectivo = 4,
            Cheque = 5,
            Boleta = 6,
            Valor_Monetario = 7
        }

        public enum Sdc_Cat_Insumo_Estado
        {
            Vacia = 1,
            ConValores = 2,
            Procesada = 3,
            EnTraslado = 4
        };

        public enum Sdc_Cat_Paquete_Generacion_Estado
        {
            EnProceso = 1,
            EnConfirmacion = 2,
            EnRevision = 3,
            Procesada = 4,
            Rechazada = 5
        };

        public enum Sdc_Transaccion_Tipo
        {
            Recepcion = 1,
            Traslado = 2,
            ProcesamientoPaquetes = 3,
            CorteParcial = 4,
            SalidaRemesa = 5,
            CreacionInsumo = 6,
            CreacionBin = 7,
            ConfiguracionRemesa = 8,
            LlenadoBolsa = 9,
            LlenadoBin = 10,
            SalidaInsumo = 11,
            SalidaBin = 12
        };



        public enum Sdc_Descuadre_Tipo
        {
            AbsMayorProc = 1,
            AbsMenorProc = 2,
            AbsIgualProYAbsMayorBol = 3,
            AbsIgualProYAbsMenorBol = 4,
            AbsIgualBolYProMayorBol = 5,
            AbsIgualBolYProMenorBol = 6,
            ProIgualBolYAbsMayorBol = 7,
            ProIgualBolYAbsMenorBol = 8,
            AbsDifBolYProDifAbsYProMayorBol = 9,
            AbsDifBolYProDifAbsYProMenorBol = 10
        }

        public enum Sdc_Descuadre_Estado
        {
            EnRevision = 1,
            Aprobado = 2,
            Rechazado = 3,
            Procesado = 4,
            Eliminado = 5
        }

        public enum ComercializacionRoles
        {
            Administrador = 25,
            Ventas = 26,
            Operaciones = 27,
            Finanzas = 28,
            GerenteG = 29
        }

        public enum InventarioRoles
        {
            Bodeguero = 8,
            Armero = 31
        }

        public enum InventarioBodegas
        {
            BCentral = 1,
            Armeria = 4
        }

        public enum InventarioTrasladosTipo
        {
            Armeria_BodCentral = 1,
            Cliente_Armeria = 2
        }

        public enum InventarioTransacciones
        {
            Ingreso = 1,
            Egreso = 2,
            Retorno = 3
        }

        public enum InventarioTransaccionEstado
        {
            Creado = 1,
            Actualizado = 2,
            ToArmeria = 3,
            ToBodegaCentral = 4,
            ToCliente = 5,
            Finalizado = 6,
            Rechazado = 7
        }

        public enum ModulosPT
        {
            rrhh = 1,
            EstadoFuerza = 2,
            Inventario = 3,
            Administracion = 4,
            Tickets = 5,
            Customers = 6,
            Comercializacion = 7
        }


    }
}