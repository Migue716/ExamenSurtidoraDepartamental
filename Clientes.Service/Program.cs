using Clientes.BusinessLogic.Interfaces;
using Clientes.BusinessLogic.Services;
using Clientes.DataAccess;
using Clientes.DataAccess.Repositories;
using Clientes.Service.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new()
    {
        Title = "API de Administración de Clientes",
        Version = "v1",
        Description = "API REST para el registro, consulta, modificación y baja lógica de clientes."
    });
});

builder.Services.AddExceptionHandler<ManejadorExcepcionesGlobal>();
builder.Services.AddProblemDetails();

var origenes = builder.Configuration.GetSection("Cors:Origenes").Get<string[]>()
              ?? ["http://localhost:4200"];

builder.Services.AddCors(opciones =>
{
    opciones.AddPolicy("Frontend", politica =>
        politica.WithOrigins(origenes)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var cadenaConexion = builder.Configuration.GetConnectionString("ClientesDb")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'ClientesDb'. Configure ConnectionStrings__ClientesDb o secretos de usuario.");

builder.Services.AddDbContext<ClientesDbContext>(opciones =>
    opciones.UseSqlServer(cadenaConexion));

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
