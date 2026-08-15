using Clientes.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clientes.DataAccess;

public class ClientesDbContext : DbContext
{
    public ClientesDbContext(DbContextOptions<ClientesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientesDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
