using System.Net.Mail;
using Clientes.BusinessLogic.DTOs;
using Clientes.BusinessLogic.Exceptions;

namespace Clientes.BusinessLogic.Validation;

public static class ClienteValidador
{
    public static void Validar(ClienteEscrituraDto dto)
    {
        var errores = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        Requerir(errores, nameof(dto.Nombre), dto.Nombre, 100);
        Requerir(errores, nameof(dto.ApellidoPaterno), dto.ApellidoPaterno, 100);
        LongitudMaxima(errores, nameof(dto.ApellidoMaterno), dto.ApellidoMaterno, 100);
        ValidarCorreo(errores, dto.CorreoElectronico);
        LongitudMaxima(errores, nameof(dto.Telefono), dto.Telefono, 20);
        LongitudMaxima(errores, nameof(dto.Direccion), dto.Direccion, 250);
        LongitudMaxima(errores, nameof(dto.Ciudad), dto.Ciudad, 100);
        LongitudMaxima(errores, nameof(dto.CodigoPostal), dto.CodigoPostal, 10);

        if (dto.FechaNacimiento.HasValue && dto.FechaNacimiento.Value > DateOnly.FromDateTime(DateTime.UtcNow.Date))
        {
            Agregar(errores, nameof(dto.FechaNacimiento), "La fecha de nacimiento no puede ser futura.");
        }

        if (errores.Count > 0)
        {
            var diccionario = errores.ToDictionary(k => k.Key, v => v.Value.ToArray());
            throw new ValidacionNegocioException("Los datos del cliente no son válidos.", diccionario);
        }
    }

    public static void ValidarPaginacion(ClienteConsultaDto consulta)
    {
        if (consulta.Pagina < 1)
        {
            throw new ValidacionNegocioException("El número de página debe ser mayor o igual a 1.");
        }

        if (consulta.TamanioPagina is < 1 or > 100)
        {
            throw new ValidacionNegocioException("El tamaño de página debe estar entre 1 y 100.");
        }
    }

    private static void ValidarCorreo(IDictionary<string, List<string>> errores, string? correo)
    {
        if (string.IsNullOrWhiteSpace(correo))
        {
            Agregar(errores, "correoElectronico", "El correo electrónico es obligatorio.");
            return;
        }

        var valor = correo.Trim();
        if (valor.Length > 200)
        {
            Agregar(errores, "correoElectronico", "El correo electrónico no puede superar 200 caracteres.");
        }

        if (!EsCorreoValido(valor))
        {
            Agregar(errores, "correoElectronico", "El correo electrónico no tiene un formato válido.");
        }
    }

    private static bool EsCorreoValido(string correo)
    {
        try
        {
            var direccion = new MailAddress(correo);
            return direccion.Address.Equals(correo, StringComparison.OrdinalIgnoreCase)
                   && correo.Contains('@', StringComparison.Ordinal);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static void Requerir(IDictionary<string, List<string>> errores, string campo, string? valor, int maximo)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            Agregar(errores, campo, $"El campo {campo} es obligatorio.");
            return;
        }

        LongitudMaxima(errores, campo, valor, maximo);
    }

    private static void LongitudMaxima(IDictionary<string, List<string>> errores, string campo, string? valor, int maximo)
    {
        if (!string.IsNullOrWhiteSpace(valor) && valor.Trim().Length > maximo)
        {
            Agregar(errores, campo, $"El campo {campo} no puede superar {maximo} caracteres.");
        }
    }

    private static void Agregar(IDictionary<string, List<string>> errores, string campo, string mensaje)
    {
        if (!errores.TryGetValue(campo, out var lista))
        {
            lista = [];
            errores[campo] = lista;
        }

        lista.Add(mensaje);
    }
}
