DELETE FROM dbo.Clientes
WHERE CorreoElectronico IN (
    N'laura.martinez@correo.com',
    N'carlos.ramirez@correo.com'
);
GO

INSERT INTO dbo.Clientes (
    Nombre,
    ApellidoPaterno,
    ApellidoMaterno,
    CorreoElectronico,
    Telefono,
    FechaNacimiento,
    Direccion,
    Ciudad,
    CodigoPostal,
    Activo,
    FechaRegistro
)
VALUES
(
    N'Laura',
    N'Martinez',
    N'Gomez',
    N'laura.martinez@correo.com',
    N'3312345678',
    '1990-05-18',
    N'Av. Vallarta 1500',
    N'Guadalajara',
    N'44110',
    1,
    SYSUTCDATETIME()
),
(
    N'Carlos',
    N'Ramirez',
    N'Lopez',
    N'carlos.ramirez@correo.com',
    N'3398765432',
    '1988-11-03',
    N'Av. Americas 1200',
    N'Zapopan',
    N'45050',
    1,
    SYSUTCDATETIME()
);
GO
