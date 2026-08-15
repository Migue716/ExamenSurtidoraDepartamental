using Clientes.BusinessLogic.DTOs;

namespace Clientes.BusinessLogic.Interfaces;

public interface IClienteService
{
    Task<ResultadoPaginadoDto<ClienteDto>> ConsultarAsync(ClienteConsultaDto consulta, CancellationToken cancellationToken = default);

    Task<ClienteDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ClienteDto> CrearAsync(ClienteEscrituraDto dto, CancellationToken cancellationToken = default);

    Task<ClienteDto> ActualizarAsync(int id, ClienteEscrituraDto dto, CancellationToken cancellationToken = default);

    Task DarDeBajaAsync(int id, CancellationToken cancellationToken = default);
}
