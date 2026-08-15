using Clientes.BusinessLogic.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Service.Middleware;

public sealed class ManejadorExcepcionesGlobal : IExceptionHandler
{
    private readonly ILogger<ManejadorExcepcionesGlobal> _logger;

    public ManejadorExcepcionesGlobal(ILogger<ManejadorExcepcionesGlobal> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problema = exception switch
        {
            ValidacionNegocioException validacion => CrearProblema(
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
                validacion.Message,
                validacion.Errores),
            RecursoNoEncontradoException noEncontrado => CrearProblema(
                StatusCodes.Status404NotFound,
                "Recurso no encontrado",
                noEncontrado.Message),
            ConflictoNegocioException conflicto => CrearProblema(
                StatusCodes.Status409Conflict,
                "Conflicto",
                conflicto.Message),
            DbUpdateException db when EsViolacionUnica(db) => CrearProblema(
                StatusCodes.Status409Conflict,
                "Conflicto",
                "El correo electrónico ya está registrado."),
            _ => CrearProblemaInterno(exception)
        };

        if (problema.Status >= 500)
        {
            _logger.LogError(exception, "Error no controlado al procesar {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "Error de negocio al procesar {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = problema.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problema, cancellationToken);
        return true;
    }

    private ProblemDetails CrearProblemaInterno(Exception exception)
    {
        _ = exception;
        return CrearProblema(
            StatusCodes.Status500InternalServerError,
            "Error interno",
            "Ocurrió un error interno al procesar la solicitud.");
    }

    private static ProblemDetails CrearProblema(
        int status,
        string titulo,
        string detalle,
        IReadOnlyDictionary<string, string[]>? errores = null)
    {
        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = detalle
        };

        if (errores is { Count: > 0 })
        {
            problema.Extensions["errors"] = errores;
        }

        return problema;
    }

    private static bool EsViolacionUnica(DbUpdateException excepcion)
    {
        return excepcion.InnerException is SqlException sql && sql.Number is 2601 or 2627;
    }
}
