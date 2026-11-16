/* =====================================================================
   Datos de ejemplo para probar el sistema (opcional).
   Ejecutar DESPUES de CrearBaseDatos.sql.
   Todas las personas y cedulas son ficticias.
   ===================================================================== */
USE ProyectoFinalPIV;
GO

/* Usuarios de prueba (clave de todos: 12345678) */
INSERT INTO dbo.Usuarios (Cedula, Nombre, Apellido1, Apellido2, Rol) VALUES
('101110111', 'Laura',  'Jimenez', 'Rojas',  'Empleado'),
('202220222', 'Carlos', 'Vargas',  'Mora',   'Empleado'),
('303330333', 'Andres', 'Solano',  'Castro', 'Tecnico TI'),
('404440444', 'Daniela','Quesada', 'Brenes', 'Tecnico TI');

INSERT INTO dbo.Credenciales (Cedula, TipoAutenticacion, Clave, UsuarioWindows, FechaUltimoCambioClave) VALUES
('101110111', 'Sistema', '12345678', NULL, GETDATE()),
('202220222', 'Sistema', '12345678', NULL, GETDATE()),
('303330333', 'Sistema', '12345678', NULL, GETDATE()),
('404440444', 'Sistema', '12345678', NULL, GETDATE());
GO

/* Computadoras */
INSERT INTO dbo.Computadoras (Marca, TipoEquipo, Serie, Patrimonio, Departamento, Cedula_Empleado) VALUES
('Dell',   'Escritorio', 'SN-0001', 'PAT-100', 'Contabilidad',     '101110111'),
('HP',     'Portatil',   'SN-0002', 'PAT-101', 'Recursos Humanos', '202220222'),
('Lenovo', 'Portatil',   'SN-0003', 'PAT-102', 'Tecnologia',       '303330333'),
('Dell',   'Servidor',   'SN-0004', 'PAT-103', 'Tecnologia',       NULL);
GO

/* Software */
INSERT INTO dbo.Software (NombreSoftware, Version, FechaInstalacion, Cedula_Tecnico, TipoLicenciamiento, LicenciasDisponibles, Observaciones) VALUES
('Microsoft Office',  '2021',    '2026-07-01', '303330333', 'Con Licencia', 8,  'Instalacion remota con instalador corporativo'),
('Google Chrome',     '128',     '2026-07-02', '303330333', 'Freeware',     50, 'Instalacion manual desde el sitio oficial'),
('Antivirus ESET',    '17.0',    '2026-07-05', '404440444', 'Con Licencia', 2,  'Instalado y activado con licencia por equipo'),
('WinRAR',            '7.0',     '2026-07-10', '404440444', 'Shareware',    5,  'Instalacion manual');
GO

/* Software instalado en computadoras */
INSERT INTO dbo.SoftwarePorComputadora (Id_Computadora, Id_Software) VALUES
(1, 1), (1, 2), (2, 1), (2, 3), (3, 2), (3, 4);
GO

/* Tiquetes en todos los estados */
INSERT INTO dbo.Tiquetes (Cedula_Empleado, Cedula_Tecnico, TipoIncidencia, Descripcion, Estado, FechaCreacion, FechaAtencion) VALUES
('101110111', NULL,        'Hardware',          'La computadora no enciende despues de un corte de luz.', 'Pendiente',   DATEADD(DAY, -1, GETDATE()), NULL),
('202220222', '303330333', 'Red',               'No hay conexion a internet en la oficina de RRHH.',      'Asignado',    DATEADD(DAY, -2, GETDATE()), NULL),
('101110111', '404440444', 'Software',          'Excel se cierra al abrir archivos grandes.',             'En Atencion', DATEADD(DAY, -3, GETDATE()), NULL),
('202220222', '303330333', 'Cuenta de usuario', 'Olvido la clave del correo institucional.',              'Resuelto',    DATEADD(DAY, -5, GETDATE()), DATEADD(DAY, -4, GETDATE())),
('101110111', '404440444', 'Otro',              'Solicitud de instalacion de impresora.',                 'Cerrado',     DATEADD(DAY, -8, GETDATE()), DATEADD(DAY, -7, GETDATE()));
GO
