namespace Clientes.BusinessLogic.DTOs;

public class ResultadoPaginadoDto<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int Pagina { get; init; }
    public int TamanioPagina { get; init; }
    public int TotalRegistros { get; init; }
    public int TotalPaginas { get; init; }
}
