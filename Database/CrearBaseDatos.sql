/* =====================================================================
   Proyecto Final - Programacion IV (UACA, II Cuatrimestre 2026)
   Sistema de Control de Activos de TI y Tiquetes de Soporte
   Script de creacion de la base de datos (SQL Server)
   ===================================================================== */

IF DB_ID('ProyectoFinalPIV') IS NULL
    CREATE DATABASE ProyectoFinalPIV;
GO

USE ProyectoFinalPIV;
GO

/* Eliminar primero las tablas hijas (tienen llaves foraneas) */
IF OBJECT_ID('dbo.SoftwarePorComputadora', 'U') IS NOT NULL DROP TABLE dbo.SoftwarePorComputadora;
IF OBJECT_ID('dbo.Tiquetes', 'U')               IS NOT NULL DROP TABLE dbo.Tiquetes;
IF OBJECT_ID('dbo.Software', 'U')               IS NOT NULL DROP TABLE dbo.Software;
IF OBJECT_ID('dbo.Computadoras', 'U')           IS NOT NULL DROP TABLE dbo.Computadoras;
IF OBJECT_ID('dbo.Credenciales', 'U')           IS NOT NULL DROP TABLE dbo.Credenciales;
IF OBJECT_ID('dbo.Usuarios', 'U')               IS NOT NULL DROP TABLE dbo.Usuarios;
GO

/* ---------- Usuarios (perfil) ---------- */
CREATE TABLE dbo.Usuarios (
    Cedula                  VARCHAR(20)  NOT NULL PRIMARY KEY,
    Nombre                  VARCHAR(50)  NOT NULL,
    Apellido1               VARCHAR(50)  NOT NULL,
    Apellido2               VARCHAR(50)  NULL,
    Rol                     VARCHAR(30)  NOT NULL
        CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN ('Empleado', 'Tecnico TI', 'Administrador TI')),
    Fecha_Creacion_Usuario  DATETIME     NOT NULL CONSTRAINT DF_Usuarios_Fecha DEFAULT (GETDATE()),
    Activo                  BIT          NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1)
);
GO

/* ---------- Credenciales (login) ---------- */
CREATE TABLE dbo.Credenciales (
    Cedula                  VARCHAR(20)  NOT NULL PRIMARY KEY
        CONSTRAINT FK_Credenciales_Usuarios REFERENCES dbo.Usuarios (Cedula) ON DELETE CASCADE,
    TipoAutenticacion       VARCHAR(10)  NOT NULL
        CONSTRAINT CK_Credenciales_Tipo CHECK (TipoAutenticacion IN ('Sistema', 'Windows')),
    Clave                   VARCHAR(100) NULL,   -- solo si TipoAutenticacion = 'Sistema'
    UsuarioWindows          VARCHAR(100) NULL,   -- solo si TipoAutenticacion = 'Windows' (DOMINIO\usuario)
    FechaUltimoCambioClave  DATETIME     NULL
);
GO

/* ---------- Control de Computadoras ---------- */
CREATE TABLE dbo.Computadoras (
    Id_Computadora   INT IDENTITY(1,1) PRIMARY KEY,
    Marca            VARCHAR(50)  NOT NULL,
    TipoEquipo       VARCHAR(20)  NOT NULL
        CONSTRAINT CK_Computadoras_Tipo CHECK (TipoEquipo IN ('Portatil', 'Escritorio', 'Servidor')),
    Serie            VARCHAR(50)  NOT NULL CONSTRAINT UQ_Computadoras_Serie UNIQUE,
    Patrimonio       VARCHAR(50)  NOT NULL,
    Departamento     VARCHAR(60)  NULL,
    Cedula_Empleado  VARCHAR(20)  NULL
        CONSTRAINT FK_Computadoras_Usuarios REFERENCES dbo.Usuarios (Cedula)
);
GO

/* ---------- Control de Software ---------- */
CREATE TABLE dbo.Software (
    Id_Software           INT IDENTITY(1,1) PRIMARY KEY,
    NombreSoftware        VARCHAR(80)  NOT NULL,
    Version               VARCHAR(30)  NULL,
    FechaInstalacion      DATE         NULL,
    Cedula_Tecnico        VARCHAR(20)  NULL
        CONSTRAINT FK_Software_Usuarios REFERENCES dbo.Usuarios (Cedula),
    TipoLicenciamiento    VARCHAR(20)  NOT NULL
        CONSTRAINT CK_Software_Tipo CHECK (TipoLicenciamiento IN ('Con Licencia', 'Freeware', 'Shareware', 'Otro')),
    LicenciasDisponibles  INT          NOT NULL CONSTRAINT DF_Software_Stock DEFAULT (0)
        CONSTRAINT CK_Software_Stock CHECK (LicenciasDisponibles >= 0),
    Observaciones         VARCHAR(300) NULL     -- como se instalo
);
GO

/* ---------- Software instalado en cada computadora (N:M) ---------- */
CREATE TABLE dbo.SoftwarePorComputadora (
    Id_Computadora  INT NOT NULL
        CONSTRAINT FK_SPC_Computadoras REFERENCES dbo.Computadoras (Id_Computadora),
    Id_Software     INT NOT NULL
        CONSTRAINT FK_SPC_Software REFERENCES dbo.Software (Id_Software),
    CONSTRAINT PK_SoftwarePorComputadora PRIMARY KEY (Id_Computadora, Id_Software)
);
GO

/* ---------- Tiquetes de Atencion ---------- */
CREATE TABLE dbo.Tiquetes (
    Id_Tiquete       INT IDENTITY(1,1) PRIMARY KEY,
    Cedula_Empleado  VARCHAR(20)  NOT NULL
        CONSTRAINT FK_Tiquetes_Empleado REFERENCES dbo.Usuarios (Cedula),
    Cedula_Tecnico   VARCHAR(20)  NULL
        CONSTRAINT FK_Tiquetes_Tecnico REFERENCES dbo.Usuarios (Cedula),
    TipoIncidencia   VARCHAR(30)  NOT NULL CONSTRAINT DF_Tiquetes_Tipo DEFAULT ('Otro'),
    Descripcion      VARCHAR(500) NOT NULL,
    Estado           VARCHAR(20)  NOT NULL CONSTRAINT DF_Tiquetes_Estado DEFAULT ('Pendiente')
        CONSTRAINT CK_Tiquetes_Estado CHECK (Estado IN ('Pendiente', 'Asignado', 'En Atencion', 'Resuelto', 'Cerrado')),
    FechaCreacion    DATETIME     NOT NULL CONSTRAINT DF_Tiquetes_Fecha DEFAULT (GETDATE()),
    FechaAtencion    DATETIME     NULL
);
GO

/* ---------- Usuario por defecto (exigido por el enunciado) ---------- */
INSERT INTO dbo.Usuarios (Cedula, Nombre, Apellido1, Apellido2, Rol)
VALUES ('0501250125', 'Administrador', 'Sistema', 'TI', 'Administrador TI');

INSERT INTO dbo.Credenciales (Cedula, TipoAutenticacion, Clave, UsuarioWindows, FechaUltimoCambioClave)
VALUES ('0501250125', 'Sistema', '12345678', NULL, GETDATE());
GO
