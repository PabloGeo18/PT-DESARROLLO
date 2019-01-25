select * from com.Pt_Armas
insert into com.Pt_Armas(carm_descripcion, carm_precio_costo, cins_precio_venta, cins_existencia, id_usuario_creacion, fecha_creacion, activo, eliminado)
values
('ESCOPETA DE BOMBEO', 2000, 0, 100, 1, getdate(), 1,0),
('ESCOPETA SEMIAUTOMATICA', 3100, 0, 100, 1, getdate(), 1,0),
('PISTOLA BERSA', 2053, 0, 100, 1, getdate(), 1,0),
('PISTOLA 9MM', 4017, 0, 100, 1, getdate(), 1,0),
('REVOLVER ', 1357, 0, 100, 1, getdate(), 1,0)