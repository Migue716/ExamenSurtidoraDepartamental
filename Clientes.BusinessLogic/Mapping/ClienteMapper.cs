using Clientes.BusinessLogic.DTOs;
using Clientes.DataAccess.Entities;

namespace Clientes.BusinessLogic.Mapping;

public static class ClienteMapper
{
    public static ClienteDto ToDto(Cliente entidad)
    {
        return new ClienteDto
        {
            ClienteId = entidad.ClienteId,
            Nombre = entidad.Nombre,
            ApellidoPaterno = entidad.ApellidoPaterno,
            ApellidoMaterno = entidad.ApellidoMaterno,
            CorreoElectronico = entidad.CorreoElectronico,
            Telefono = entidad.Telefono,
            FechaNacimiento = entidad.FechaNacimiento,
            Direccion = entidad.Direccion,
            Ciudad = entidad.Ciudad,
            CodigoPostal = entidad.CodigoPostal,
            Activo = entidad.Activo,
            FechaRegistro = entidad.FechaRegistro,
            FechaModificacion = entidad.FechaModificacion
        };
    }

    public static Cliente ToEntidad(ClienteEscrituraDto dto)
    {
        return new Cliente
        {
            Nombre = Normalizar(dto.Nombre)!,
            ApellidoPaterno = Normalizar(dto.ApellidoPaterno)!,
            ApellidoMaterno = Normalizar(dto.ApellidoMaterno),
            CorreoElectronico = NormalizarCorreo(dto.CorreoElectronico)!,
            Telefono = Normalizar(dto.Telefono),
            FechaNacimiento = dto.FechaNacimiento,
            Direccion = Normalizar(dto.Direccion),
            Ciudad = Normalizar(dto.Ciudad),
            CodigoPostal = Normalizar(dto.CodigoPostal)
        };
    }

    public static void Copiar(ClienteEscrituraDto dto, Cliente entidad)
    {
        entidad.Nombre = Normalizar(dto.Nombre)!;
        entidad.ApellidoPaterno = Normalizar(dto.ApellidoPaterno)!;
        entidad.ApellidoMaterno = Normalizar(dto.ApellidoMaterno);
        entidad.CorreoElectronico = NormalizarCorreo(dto.CorreoElectronico)!;
        entidad.Telefono = Normalizar(dto.Telefono);
        entidad.FechaNacimiento = dto.FechaNacimiento;
        entidad.Direccion = Normalizar(dto.Direccion);
        entidad.Ciudad = Normalizar(dto.Ciudad);
        entidad.CodigoPostal = Normalizar(dto.CodigoPostal);
    }

    public static string? Normalizar(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        return valor.Trim();
    }

    public static string? NormalizarCorreo(string? correo)
    {
        var normalizado = Normalizar(correo);
        return normalizado?.ToLowerInvariant();
    }
}
