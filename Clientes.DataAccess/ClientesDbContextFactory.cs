using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Clientes.DataAccess;

public class ClientesDbContextFactory : IDesignTimeDbContextFactory<ClientesDbContext>
{
    public ClientesDbContext CreateDbContext(string[] args)
    {
        var cadena = Environment.GetEnvironmentVariable("ConnectionStrings__ClientesDb")
            ?? "Server=localhost;Database=ClientesDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True";

        var opciones = new DbContextOptionsBuilder<ClientesDbContext>()
            .UseSqlServer(cadena);

        return new ClientesDbContext(opciones.Options);
    }
}
