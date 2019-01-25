Use Protal_web

--CREATE TABLES
create table com.Pt_Insumos(
	cins_id int identity(1,1) not null primary key,
	cins_descripcion varchar(100),
	cins_precio_costo money,
	cins_precio_venta money,
	cins_talla varchar(5),
	cins_es_insumo bit default 0,
	cins_es_arma bit default 0,
	cins_es_uniforme bit default 0,
	cins_depreciacion bit,
	cins_porcentaje_depreciacion decimal(7,5),
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0
)

create table com.Pt_Tipo_Pagos(
	ctpa_id int identity(1,1) not null primary key,
	ctpa_descripcion varchar(100),
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0
)

create table com.Pt_Puestos(
	cpue_id int identity(1,1) not null primary key,
	cpue_descripcion varchar(100),
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0
)

create table com.Pt_Pagos_Puesto(
	cppu_id int identity(1,1) not null primary key,
	cppu_ctpa_id int not null,
	cppu_cpue_id int not null,
	cppu_descripcion varchar(150),
	cppu_monto money,
	cppu_salario_base bit default 0,
	cppu_depende_sb bit default 0,
	cppu_porcentaje_calculo decimal(7,5),
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0,
	constraint FK_Pt_Pagos_Puesto_Pt_Tipo_Pago foreign key(cppu_ctpa_id) references com.Pt_Tipo_Pagos(ctpa_id),
	constraint FK_Pt_Pagos_Puesto_Pt_Puesto foreign key(cppu_cpue_id) references com.Pt_Puestos(cpue_id)
)

create table com.Pt_Puesto_Insumos(
	cpin_id int identity(1,1) not null primary key,
	cpin_cpue_id int not null,
	cpin_cins_id int not null,
	cpin_cantidad int,
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0,
	constraint FK_Pt_Puesto_Insumos_Pt_Puesto foreign key(cpin_cpue_id) references com.Pt_Puestos(cpue_id),
	constraint FK_Pt_Puesto_Insumos_Pt_Insumos foreign key(cpin_cins_id) references com.Pt_Insumos(cins_id)
)

create table com.Pt_Costos_Fijos(
	ccof_id int identity(1,1) not null primary key,
	ccof_descripcion varchar(100),
	ccof_precio_unitario money(7,2),
	cgcf_consumible bit,
	cgcf_depreciable bit,
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0
)

create table com.Pt_Estados_Fase(
	cefa_id int identity(1,1) not null primary key,
	cefa_descripcion varchar(100),
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0
)

create table com.Pt_Tipos_Proyecto(
	ctpo_id int identity(1,1) not null primary key,
	ctpo_descripcion varchar(100),
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0
)

create table com.Pt_Cotizaciones(
	ccot_id int identity(1,1) not null primary key,
	ccot_nombre_proyecto varchar(500),
	ccot_descripcion_general varchar(max),
	ccot_contacto varchar(100),
	ccot_telefono_contacto int,
	ccot_correo_contacto varchar(250),
	ccot_direccion_contacto varchar(500),
	ccot_fecha_entrega date,
	ccot_finaliza_cotizacion bit default 0,
	ccot_id_usuario_finalizacion int,
	ccot_fecha_finalizacion datetime,
	ccot_ctpo_id int,
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0,
	constraint FK_Pt_Fase_Cotizacion_Pt_Tipos_Proyecto foreign key(ccot_ctpo_id) references com.Pt_Tipos_Proyecto(ctpo_id),
)

create table com.Pt_Fases_Cotizacion(
	cfas_id int identity(1,1) not null primary key,
	cfas_nombre varchar(150),
	cfas_descripcion varchar(max),
	cfas_fecha_entrega date,
	cfas_cefa_id int,
	cfas_ccot_id int,
	cfas_rev_ven bit not null default 1,
	cfas_ok_ven bit not null default 0,
	cfas_id_usuario_ven int not null,
	cfas_rev_ope bit not null default 0,
	cfas_ok_ope bit not null default 0,
	cfas_id_usuario_ope int not null,
	cfas_rev_gge bit not null default 0,
	cfas_ok_gge bit not null default 0,
	cfas_rc_gge bit not null default 0,
	cfas_id_usuario_gge int,
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	historico bit not null default 0,
	activo bit not null default 1,
	eliminado bit not null default 0,
	constraint FK_Pt_Fase_Cotizacion_Pt_Cotizaciones foreign key(cfas_ccot_id) references com.Pt_Cotizaciones(ccot_id),
	constraint FK_Pt_Fases_Cotizacion_Pt_Estado_Fase foreign key(cfas_cefa_id) references com.Pt_Estados_Fase(cefa_id)
)

create table com.Pt_Tmp_Propuesta_Fase_Puesto(
	ctpf_id int identity(1,1) not null primary key,
	ctpf_cpue_id int,
	ctpf_cfas_id int,
	ctpf_personal int,
	ctpf_facConIVA money,
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0,
	constraint FK_Pt_Tmp_Propuesta_Fase_Puesto_Pt_Puestos foreign key(ctpf_cpue_id) references com.Pt_Puestos(cpue_id),
	constraint FK_Pt_Tmp_Propuesta_Fase_Puesto_Pt_Fases_Cotizacion foreign key(ctpf_cfas_id) references com.Pt_Fases_Cotizacion(cfas_id)
)

create table com.Pt_Tmp_Cotizacion_Fase_Insumos(
	ctpfi_id int identity(1,1) not null primary key,
	ctpfi_cins_id int,
	ctpfi_cantidad int,
	ctpfi_cfas_id int,
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0,
	constraint FK_Pt_Tmp_Cotizacion_Fase_Insumos_Pt_Puestos foreign key(ctpfi_cins_id) references com.Pt_Insumos(cins_id),
	constraint FK_Pt_Tmp_Cotizacion_Fase_Insumos_Pt_Fases_Cotizacion foreign key(ctpfi_cfas_id) references com.Pt_Fases_Cotizacion(cfas_id)
)

create table com.Pt_Tmp_Cotizacion_Fase_Insumos_New(
	ctpfin_id int identity(1,1) not null primary key,
	ctpfin_descripcion varchar(100),
	ctpfin_precio_costo money,
	ctpfin_precio_venta money,
	ctpfin_talla varchar(5),
	ctpfin_es_insumo bit default 0,
	ctpfin_es_arma bit default 0,
	ctpfin_es_uniforme bit default 0,
	ctpfin_depreciacion bit,
	ctpfin_porcentaje_depreciacion decimal(7,5),
	ctpfin_cantidad int,
	ctpfin_cfas_id int,
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0,
	constraint FK_Pt_Tmp_Cotizacion_Fase_Insumos_New_Pt_Fases_Cotizacion foreign key(ctpfin_cfas_id) references com.Pt_Fases_Cotizacion(cfas_id)
)

create table com.Pt_Meses(
	cmes_id int identity(1,1) not null primary key,
	cmes_descripcion varchar(200),
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0
)

create table com.Pt_Tipo_Gasto(
	ctga_id int identity(1,1) not null primary key,
	ctga_descripcion varchar(300),
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0
)

alter table com.Pt_Costos_Fijos_Mes_Anio
add ccfma_monto_gasto_operativo money
create table com.Pt_Costos_Fijos_Mes_Anio(
	ccfma_id int identity(1,1) not null primary key,
	ccfma_descripcion varchar(500),
	ccfma_mes int,
	ccfma_anio int,
	ccfma_monto_gasto_administrativo money,
	ccfma_monto_gasto_operativo money,
	--ccfma_tipo_gasto int,
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0,
	constraint FK_Pt_Costos_Fijos_Mes_Anio_Pt_Meses foreign key(ccfma_mes) references com.Pt_Meses(cmes_id)
	--constraint FK_Pt_Costos_Fijos_Mes_Anio_Pt_Tipo_Gasto foreign key(ccfma_tipo_gasto) references com.Pt_Tipo_Gasto(ctga_id)
)

create table com.Pt_Gastos_Costos_Fijos_Mes_Anio(
	cgcf_id int identity(1,1) not null primary key,
	cgcf_ccfma_id int,
	cgcf_descripcion varchar(100),
	cgcf_precio_unitario money,
	cgcf_cantidad int,
	cgcf_consumible bit,
	cgcf_depreciable bit,
	cgcf_porcentaje_depreciacion decimal(7,5),
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0,
	constraint FK_Pt_Gastos_Costos_Fijos_Mes_Anio_Pt_Costos_Fijos_Mes_Anio foreign key(cgcf_ccfma_id) references com.Pt_Costos_Fijos_Mes_Anio(ccfma_id)
)

create table com.Prueba(
precio money
)

create table com.Pt_Cantidad_Planilla_Mes_Anio(
	ccpma_id int identity(1,1) not null primary key,
	ccpma_descripcion varchar(500),
	ccpma_mes int,
	ccpma_anio int,
	ccpma_cantidad_planilla int,
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0,
	constraint FK_Pt_Cantidad_Planilla_Mes_Anio_Pt_Meses foreign key(ccpma_mes) references com.Pt_Meses(cmes_id)
)

CREATE TABLE com.Pt_Tmp_Cotizacion_Fase_Margen_Conf(
	ctcfm_id int identity(1,1) not null primary key,
	ctcfm_margen decimal (5,2),
	ctcfm_cfas_id int,
	id_usuario_creacion int not null,
	fecha_creacion datetime not null,
	id_usuario_modificacion int,
	fecha_modificacion datetime,
	id_usuario_eliminacion int,
	fecha_eliminacion datetime,
	activo bit not null default 1,
	eliminado bit not null default 0,
	constraint FK_Pt_Tmp_Cotizacion_Fase_Margen_Conf_Pt_Fases_Cotizacion foreign key(ctcfm_cfas_id) references com.Pt_Fases_Cotizacion(cfas_id)
);

--INSERTS
--Use AdmSeg_PT
--insert into adm.Aplicaciones(nombre, ruta)
--values ('Comercializacion','Comercializacion')

--insert into adm.Roles(nombre, id_aplicacion)
--values
--('Administrador Comercializacion',7),
--('Ventas - Comercializacion',7),
--('Operaciones - Comercializacion',7),
--('Finanzas - Comercializacion',7),
--('Gerente General - Comercializacion',7)

--insert into adm.Controladores(nombre,id_aplicacion)
--values
--('Home',7),
--('Insumos',7),
--('Tipo_Pagos',7),
--('Puestos',7),
--('Costos_Fijos',7),
--('Estados_Fase',7),
--('Cotizaciones',7),
--('Fases_Cotizacion',7),
--('Tipos_Proyecto',7),
--('Tmp_Propuesta_Fase_Puesto',7),
--('Costos_Fijos_Mes_Anio',7),
--('Gastos_Costos_Fijos_Mes_Anio',7),
--('Cantidad_Planilla_Mes_Anio',7)

--insert into adm.Acciones(nombre,id_controlador)
--values
----Home
--('Index',188),
----Insumos
--('Index',189),
--('Create',189),
--('Edit',189),
--('Details',189),
--('Delete',189),
--('DeleteConfirmed',189),
----Tipo_Pagos
--('Index',190),
--('Create',190),
--('Edit',190),
--('Details',190),
--('Delete',190),
--('DeleteConfirmed',190),
----Puestos
--('Index',191),
--('Create',191),
--('Edit',191),
--('Details',191),
--('Delete',191),
--('DeleteConfirmed',191),
--('CrearPuestoInsumos',191),
--('CrearPagosPuesto',191),
--('EliminarPuestoInsumos',191),
--('EliminarPagoPuesto',191),
--('DisminuirCantidadPuestoInsumos',191),
--('AumentarCantidadPuestoInsumos',191),
----Costos_Fijos
--('Index',192),
--('Create',192),
--('Edit',192),
--('Details',192),
--('Delete',192),
--('DeleteConfirmed',192),
----Estados_Fase
--('Index',193),
--('Create',193),
--('Edit',193),
--('Details',193),
--('Delete',193),
--('DeleteConfirmed',193),
----Cotizaciones
--('Index',194),
--('Create',194),
--('Edit',194),
--('Details',194),
--('Delete',194),
--('DeleteConfirmed',194),
--('FinalizarCotizacion',194),
--('CotizacionesFinalizadas',194),
--('CotizacionesEliminadas',194),
--('ClonarCotizacion',194),
----Fases_Cotizacion
--('Index',195),
--('Create',195),
--('Edit',195),
--('Details',195),
--('Delete',195),
--('DeleteConfirmed',195),
--('EnviarRevisionOperaciones',195),
--('EnviarRevisionVentas',195),
--('EnviarRevisionGerenteG',195),
--('RechazoGerente',195),
--('AprobadoGerente',195),
--('AprobadoVentas',195),
----Tipos_Proyecto
--('Index',196),
--('Create',196),
--('Edit',196),
--('Details',196),
--('Delete',196),
--('DeleteConfirmed',196),
----Tmp_Propuesta_Fase_Puesto
--('Create', 197),
--('Index', 197),
--('Delete',197),
--('DeleteConfirmed',197),
--('SumTotal',197),
--('CreateInsumosTmp',197),
--('CreateInsumosGenerales',197),
--('DeleteInsmos',197),
--('UpdateCantidadPersonal',197),
--('UpdateFacConIVA',197),
--('ManejoMargen',197),
----Costos_Fijos_Mes_Anio
--('Index',198),
--('Create',198),
--('FiltroAnio',198),
--('Edit',198),
--('Delete',198),
--('DeleteConfirmed',198),
--('Details',198),
----Gastos_Costos_Fijos_Mes_Anio
--('Index',199),
--('Create',199),
--('CreateGastos',199),
--('Edit',199),
--('Details',199),
--('Delete',199),
--('DeleteConfirmed',199)
----Cantidad_Planilla_Mes_Anio
--('Index',2197),
--('Create',2197),
--('FiltroAnio',2197), 
--('Edit',2197),
--('Details',2197),
--('Delete',2197),
--('DeleteConfirmed',2197)

--insert into com.Pt_Meses(cmes_descripcion, id_usuario_creacion, fecha_creacion, activo, eliminado)
--values
--('Enero', 1, getdate(), 1, 0),
--('Febrero', 1, getdate(), 1, 0),
--('Marzo', 1, getdate(), 1, 0),
--('Abril', 1, getdate(), 1, 0),
--('Mayo', 1, getdate(), 1, 0),
--('Junio', 1, getdate(), 1, 0),
--('Julio', 1, getdate(), 1, 0),
--('Agosto', 1, getdate(), 1, 0),
--('Septiembre', 1, getdate(), 1, 0),
--('Octubre', 1, getdate(), 1, 0),
--('Noviembre', 1, getdate(), 1, 0),
--('Diciembre', 1, getdate(), 1, 0)

--insert into com.Pt_Tipo_Gasto(ctga_descripcion, id_usuario_creacion, fecha_creacion, activo, eliminado)
--values
--('Gasto Administrativo', 1, getdate(), 1, 0),
--('Gasto Operativo', 1, getdate(), 1, 0)