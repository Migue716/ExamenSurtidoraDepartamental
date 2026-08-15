using Clientes.BusinessLogic.DTOs;
using Clientes.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clientes.Service.Controllers;

[ApiController]
[Route("api/clientes")]
[Produces("application/json")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _servicio;

    public ClientesController(IClienteService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginadoDto<ClienteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ResultadoPaginadoDto<ClienteDto>>> Consultar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanioPagina = 10,
        [FromQuery] string? buscar = null,
        [FromQuery] bool? activo = null,
        [FromQuery] string? ordenarPor = null,
        [FromQuery] bool descendente = false,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _servicio.ConsultarAsync(new ClienteConsultaDto
        {
            Pagina = pagina,
            TamanioPagina = tamanioPagina,
            Buscar = buscar,
            Activo = activo,
            OrdenarPor = ordenarPor,
            Descendente = descendente
        }, cancellationToken);

        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> ObtenerPorId(int id, CancellationToken cancellationToken)
    {
        return Ok(await _servicio.ObtenerPorIdAsync(id, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteDto>> Crear(
        [FromBody] ClienteEscrituraDto dto,
        CancellationToken cancellationToken)
    {
        var creado = await _servicio.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.ClienteId }, creado);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClienteDto>> Actualizar(
        int id,
        [FromBody] ClienteEscrituraDto dto,
        CancellationToken cancellationToken)
    {
        return Ok(await _servicio.ActualizarAsync(id, dto, cancellationToken));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DarDeBaja(int id, CancellationToken cancellationToken)
    {
        await _servicio.DarDeBajaAsync(id, cancellationToken);
        return NoContent();
    }
}
