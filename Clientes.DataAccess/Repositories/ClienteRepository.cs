using Clientes.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clientes.DataAccess.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly ClientesDbContext _context;

    public ClienteRepository(ClientesDbContext context)
    {
        _context = context;
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id, bool seguimiento, CancellationToken cancellationToken = default)
    {
        var consulta = _context.Clientes.AsQueryable();

        if (!seguimiento)
        {
            consulta = consulta.AsNoTracking();
        }

        return await consulta.FirstOrDefaultAsync(c => c.ClienteId == id, cancellationToken);
    }

    public Task<bool> ExisteCorreoAsync(string correoElectronico, int? excluirClienteId = null, CancellationToken cancellationToken = default)
    {
        var consulta = _context.Clientes.AsNoTracking()
            .Where(c => c.CorreoElectronico.ToLower() == correoElectronico.ToLower());

        if (excluirClienteId.HasValue)
        {
            consulta = consulta.Where(c => c.ClienteId != excluirClienteId.Value);
        }

        return consulta.AnyAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Cliente> Items, int Total)> BuscarAsync(
        string? buscar,
        bool? activo,
        int pagina,
        int tamanioPagina,
        string? ordenarPor,
        bool descendente,
        CancellationToken cancellationToken = default)
    {
        var consulta = _context.Clientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var termino = buscar.Trim();
            consulta = consulta.Where(c =>
                c.Nombre.Contains(termino) ||
                c.ApellidoPaterno.Contains(termino) ||
                (c.ApellidoMaterno != null && c.ApellidoMaterno.Contains(termino)) ||
                c.CorreoElectronico.Contains(termino) ||
                (c.Telefono != null && c.Telefono.Contains(termino)));
        }

        if (activo.HasValue)
        {
            consulta = consulta.Where(c => c.Activo == activo.Value);
        }

        var total = await consulta.CountAsync(cancellationToken);
        consulta = AplicarOrden(consulta, ordenarPor, descendente);

        var items = await consulta
            .Skip((pagina - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AgregarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await _context.Clientes.AddAsync(cliente, cancellationToken);
    }

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    private static IQueryable<Cliente> AplicarOrden(IQueryable<Cliente> consulta, string? ordenarPor, bool descendente)
    {
        return (ordenarPor?.Trim().ToLowerInvariant()) switch
        {
            "nombre" => descendente
                ? consulta.OrderByDescending(c => c.Nombre).ThenByDescending(c => c.ApellidoPaterno)
                : consulta.OrderBy(c => c.Nombre).ThenBy(c => c.ApellidoPaterno),
            "correo" => descendente
                ? consulta.OrderByDescending(c => c.CorreoElectronico)
                : consulta.OrderBy(c => c.CorreoElectronico),
            "id" => descendente
                ? consulta.OrderByDescending(c => c.ClienteId)
                : consulta.OrderBy(c => c.ClienteId),
            _ => descendente
                ? consulta.OrderByDescending(c => c.ApellidoPaterno).ThenByDescending(c => c.Nombre)
                : consulta.OrderBy(c => c.ApellidoPaterno).ThenBy(c => c.Nombre)
        };
    }
}
