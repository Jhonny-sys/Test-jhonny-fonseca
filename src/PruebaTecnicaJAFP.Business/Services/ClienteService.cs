using PruebaTecnicaJAFP.Business.Contracts;
using PruebaTecnicaJAFP.Business.Models;

namespace PruebaTecnicaJAFP.Business.Services;

public sealed class ClienteService(IClienteRepository clientes, IUbicacionRepository ubicaciones)
{
    public Task<IReadOnlyList<Cliente>> ObtenerTodosAsync(CancellationToken ct) => clientes.ObtenerTodosAsync(ct);
    public Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken ct) => clientes.ObtenerPorIdAsync(id, ct);

    public async Task<int> CrearAsync(ClienteInput input, CancellationToken ct)
    {
        await ValidarAsync(input, null, ct);
        return await clientes.CrearAsync(input, ct);
    }

    public async Task<bool> ActualizarAsync(int id, ClienteInput input, CancellationToken ct)
    {
        await ValidarAsync(input, id, ct);
        return await clientes.ActualizarAsync(id, input, ct);
    }

    private async Task ValidarAsync(ClienteInput input, int? excluirId, CancellationToken ct)
    {
        if (input.ClnTipoId <= 0) throw new ReglaNegocioException("El tipo de identificacion es obligatorio.");
        if (string.IsNullOrWhiteSpace(input.ClnNumeroIdentificacion) || input.ClnNumeroIdentificacion.Length > 30)
            throw new ReglaNegocioException("La identificacion es obligatoria y debe tener maximo 30 caracteres.");
        if (string.IsNullOrWhiteSpace(input.ClnRazonSocial) || input.ClnRazonSocial.Length > 150)
            throw new ReglaNegocioException("La razon social es obligatoria y debe tener maximo 150 caracteres.");
        if (await clientes.ExisteIdentificacionAsync(input.ClnNumeroIdentificacion.Trim(), excluirId, ct))
            throw new ReglaNegocioException("Ya existe un cliente con esta identificacion.");
        if (!await ubicaciones.EsUbicacionValidaAsync(input.ClnPaisCodigo, input.ClnDptColCodigoDane, input.ClnDvsPltColCodigoDane, ct))
            throw new ReglaNegocioException("La ciudad seleccionada no pertenece al departamento y pais indicados.");
    }
}

public sealed class ReglaNegocioException(string message) : Exception(message);
