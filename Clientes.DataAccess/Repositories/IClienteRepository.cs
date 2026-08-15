using Clientes.DataAccess.Entities;

namespace Clientes.DataAccess.Repositories;

public interface IClienteRepository
{
    Task<Cliente?> ObtenerPorIdAsync(int id, bool seguimiento, CancellationToken cancellationToken = default);

    Task<bool> ExisteCorreoAsync(string correoElectronico, int? excluirClienteId = null, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Cliente> Items, int Total)> BuscarAsync(
        string? buscar,
        bool? activo,
        int pagina,
        int tamanioPagina,
        string? ordenarPor,
        bool descendente,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(Cliente cliente, CancellationToken cancellationToken = default);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
