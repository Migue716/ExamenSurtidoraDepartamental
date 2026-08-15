USE [master];
GO

IF DB_ID(N'ClientesDb') IS NULL
BEGIN
    CREATE DATABASE [ClientesDb];
END
GO

USE [ClientesDb];
GO

IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clientes
    (
        ClienteId INT IDENTITY(1, 1) NOT NULL,
        Nombre NVARCHAR(100) NOT NULL,
        ApellidoPaterno NVARCHAR(100) NOT NULL,
        ApellidoMaterno NVARCHAR(100) NULL,
        CorreoElectronico NVARCHAR(200) NOT NULL,
        Telefono NVARCHAR(20) NULL,
        FechaNacimiento DATE NULL,
        Direccion NVARCHAR(250) NULL,
        Ciudad NVARCHAR(100) NULL,
        CodigoPostal NVARCHAR(10) NULL,
        Activo BIT NOT NULL CONSTRAINT DF_Clientes_Activo DEFAULT (1),
        FechaRegistro DATETIME2 NOT NULL CONSTRAINT DF_Clientes_FechaRegistro DEFAULT (SYSUTCDATETIME()),
        FechaModificacion DATETIME2 NULL,
        CONSTRAINT PK_Clientes PRIMARY KEY CLUSTERED (ClienteId),
        CONSTRAINT UQ_Clientes_Correo UNIQUE (CorreoElectronico)
    );

    CREATE INDEX IX_Clientes_Nombre
        ON dbo.Clientes (ApellidoPaterno, Nombre);
END
GO
