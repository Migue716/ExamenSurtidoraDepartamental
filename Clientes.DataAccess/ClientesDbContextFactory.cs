using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Clientes.DataAccess;

public class ClientesDbContextFactory : IDesignTimeDbContextFactory<ClientesDbContext>
{
    public ClientesDbContext CreateDbContext(string[] args)
    {
        var cadena = Environment.GetEnvironmentVariable("ConnectionStrings__ClientesDb")
            ?? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ClientesDb;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True";

        var opciones = new DbContextOptionsBuilder<ClientesDbContext>()
            .UseSqlServer(cadena);

        return new ClientesDbContext(opciones.Options);
    }
}
