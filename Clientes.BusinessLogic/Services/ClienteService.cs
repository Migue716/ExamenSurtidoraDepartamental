using Clientes.BusinessLogic.DTOs;
using Clientes.BusinessLogic.Exceptions;
using Clientes.BusinessLogic.Interfaces;
using Clientes.BusinessLogic.Mapping;
using Clientes.BusinessLogic.Validation;
using Clientes.DataAccess.Repositories;

namespace Clientes.BusinessLogic.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repositorio;

    public ClienteService(IClienteRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<ResultadoPaginadoDto<ClienteDto>> ConsultarAsync(
        ClienteConsultaDto consulta,
        CancellationToken cancellationToken = default)
    {
        ClienteValidador.ValidarPaginacion(consulta);

        var (items, total) = await _repositorio.BuscarAsync(
            consulta.Buscar,
            consulta.Activo,
            consulta.Pagina,
            consulta.TamanioPagina,
            consulta.OrdenarPor,
            consulta.Descendente,
            cancellationToken);

        var totalPaginas = total == 0
            ? 0
            : (int)Math.Ceiling(total / (double)consulta.TamanioPagina);

        return new ResultadoPaginadoDto<ClienteDto>
        {
            Items = items.Select(ClienteMapper.ToDto).ToList(),
            Pagina = consulta.Pagina,
            TamanioPagina = consulta.TamanioPagina,
            TotalRegistros = total,
            TotalPaginas = totalPaginas
        };
    }

    public async Task<ClienteDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cliente = await _repositorio.ObtenerPorIdAsync(id, seguimiento: false, cancellationToken);
        if (cliente is null)
        {
            throw new RecursoNoEncontradoException($"No existe un cliente con el identificador {id}.");
        }

        return ClienteMapper.ToDto(cliente);
    }

    public async Task<ClienteDto> CrearAsync(ClienteEscrituraDto dto, CancellationToken cancellationToken = default)
    {
        ClienteValidador.Validar(dto);

        var correo = ClienteMapper.NormalizarCorreo(dto.CorreoElectronico)!;
        if (await _repositorio.ExisteCorreoAsync(correo, excluirClienteId: null, cancellationToken))
        {
            throw new ConflictoNegocioException("El correo electrónico ya está registrado.");
        }

        var entidad = ClienteMapper.ToEntidad(dto);
        entidad.Activo = true;
        entidad.FechaRegistro = DateTime.UtcNow;
        entidad.FechaModificacion = null;

        await _repositorio.AgregarAsync(entidad, cancellationToken);
        await _repositorio.GuardarCambiosAsync(cancellationToken);

        return ClienteMapper.ToDto(entidad);
    }

    public async Task<ClienteDto> ActualizarAsync(int id, ClienteEscrituraDto dto, CancellationToken cancellationToken = default)
    {
        ClienteValidador.Validar(dto);

        var entidad = await _repositorio.ObtenerPorIdAsync(id, seguimiento: true, cancellationToken);
        if (entidad is null)
        {
            throw new RecursoNoEncontradoException($"No existe un cliente con el identificador {id}.");
        }

        var correo = ClienteMapper.NormalizarCorreo(dto.CorreoElectronico)!;
        if (await _repositorio.ExisteCorreoAsync(correo, id, cancellationToken))
        {
            throw new ConflictoNegocioException("El correo electrónico ya está registrado.");
        }

        ClienteMapper.Copiar(dto, entidad);
        entidad.FechaModificacion = DateTime.UtcNow;

        await _repositorio.GuardarCambiosAsync(cancellationToken);
        return ClienteMapper.ToDto(entidad);
    }

    public async Task DarDeBajaAsync(int id, CancellationToken cancellationToken = default)
    {
        var entidad = await _repositorio.ObtenerPorIdAsync(id, seguimiento: true, cancellationToken);
        if (entidad is null)
        {
            throw new RecursoNoEncontradoException($"No existe un cliente con el identificador {id}.");
        }

        if (!entidad.Activo)
        {
            throw new ConflictoNegocioException("El cliente ya se encuentra dado de baja.");
        }

        entidad.Activo = false;
        entidad.FechaModificacion = DateTime.UtcNow;
        await _repositorio.GuardarCambiosAsync(cancellationToken);
    }
}
