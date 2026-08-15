using Clientes.BusinessLogic.DTOs;
using Clientes.BusinessLogic.Exceptions;
using Clientes.BusinessLogic.Services;
using Clientes.DataAccess.Entities;
using Clientes.DataAccess.Repositories;
using Moq;

namespace Clientes.BusinessLogic.Tests;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _repositorio = new();
    private readonly ClienteService _servicio;

    public ClienteServiceTests()
    {
        _servicio = new ClienteService(_repositorio.Object);
    }

    [Fact]
    public async Task CrearAsync_CuandoDatosSonValidos_RegistraClienteActivo()
    {
        var dto = ClienteValido();
        Cliente? persistido = null;

        _repositorio
            .Setup(r => r.ExisteCorreoAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositorio
            .Setup(r => r.AgregarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()))
            .Callback<Cliente, CancellationToken>((cliente, _) =>
            {
                cliente.ClienteId = 15;
                persistido = cliente;
            })
            .Returns(Task.CompletedTask);

        _repositorio
            .Setup(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resultado = await _servicio.CrearAsync(dto);

        Assert.Equal(15, resultado.ClienteId);
        Assert.Equal("laura.martinez@correo.com", resultado.CorreoElectronico);
        Assert.True(resultado.Activo);
        Assert.NotNull(persistido);
        Assert.True(persistido!.Activo);
        Assert.NotEqual(default, persistido.FechaRegistro);
        _repositorio.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CrearAsync_CuandoCorreoEstaDuplicado_LanzaConflicto()
    {
        _repositorio
            .Setup(r => r.ExisteCorreoAsync("laura.martinez@correo.com", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var excepcion = await Assert.ThrowsAsync<ConflictoNegocioException>(
            () => _servicio.CrearAsync(ClienteValido()));

        Assert.Contains("correo", excepcion.Message, StringComparison.OrdinalIgnoreCase);
        _repositorio.Verify(r => r.AgregarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CrearAsync_CuandoNombreEsVacio_LanzaValidacion()
    {
        var dto = ClienteValido();
        dto.Nombre = " ";

        await Assert.ThrowsAsync<ValidacionNegocioException>(() => _servicio.CrearAsync(dto));
    }

    [Fact]
    public async Task CrearAsync_CuandoFechaNacimientoEsFutura_LanzaValidacion()
    {
        var dto = ClienteValido();
        dto.FechaNacimiento = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1));

        await Assert.ThrowsAsync<ValidacionNegocioException>(() => _servicio.CrearAsync(dto));
    }

    [Fact]
    public async Task ActualizarAsync_CuandoClienteNoExiste_LanzaNoEncontrado()
    {
        _repositorio
            .Setup(r => r.ObtenerPorIdAsync(99, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        await Assert.ThrowsAsync<RecursoNoEncontradoException>(
            () => _servicio.ActualizarAsync(99, ClienteValido()));
    }

    [Fact]
    public async Task DarDeBajaAsync_CuandoClienteExiste_MarcaActivoEnFalso()
    {
        var cliente = new Cliente
        {
            ClienteId = 3,
            Nombre = "Carlos",
            ApellidoPaterno = "Ramírez",
            CorreoElectronico = "carlos@correo.com",
            Activo = true,
            FechaRegistro = DateTime.UtcNow.AddDays(-2)
        };

        _repositorio
            .Setup(r => r.ObtenerPorIdAsync(3, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        _repositorio
            .Setup(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _servicio.DarDeBajaAsync(3);

        Assert.False(cliente.Activo);
        Assert.NotNull(cliente.FechaModificacion);
        _repositorio.Verify(r => r.GuardarCambiosAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DarDeBajaAsync_CuandoClienteNoExiste_LanzaNoEncontrado()
    {
        _repositorio
            .Setup(r => r.ObtenerPorIdAsync(8, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        await Assert.ThrowsAsync<RecursoNoEncontradoException>(() => _servicio.DarDeBajaAsync(8));
    }

    private static ClienteEscrituraDto ClienteValido() => new()
    {
        Nombre = "Laura",
        ApellidoPaterno = "Martínez",
        ApellidoMaterno = "Gómez",
        CorreoElectronico = "laura.martinez@correo.com",
        Telefono = "3312345678",
        FechaNacimiento = new DateOnly(1990, 5, 18),
        Direccion = "Av. Vallarta 1500",
        Ciudad = "Guadalajara",
        CodigoPostal = "44110"
    };
}
