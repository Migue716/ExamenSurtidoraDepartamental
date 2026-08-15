namespace Clientes.BusinessLogic.Exceptions;

public class ConflictoNegocioException : Exception
{
    public ConflictoNegocioException(string mensaje) : base(mensaje)
    {
    }
}
