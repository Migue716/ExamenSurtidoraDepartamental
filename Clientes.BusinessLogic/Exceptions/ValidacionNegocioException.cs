namespace Clientes.BusinessLogic.Exceptions;

public class ValidacionNegocioException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errores { get; }

    public ValidacionNegocioException(string mensaje)
        : this(mensaje, new Dictionary<string, string[]> { ["general"] = [mensaje] })
    {
    }

    public ValidacionNegocioException(string mensaje, IDictionary<string, string[]> errores)
        : base(mensaje)
    {
        Errores = new Dictionary<string, string[]>(errores);
    }
}
