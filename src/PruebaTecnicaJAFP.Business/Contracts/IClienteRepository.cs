using PruebaTecnicaJAFP.Business.Models;

namespace PruebaTecnicaJAFP.Business.Contracts;

public interface IClienteRepository
{
    Task<IReadOnlyList<Cliente>> ObtenerTodosAsync(CancellationToken cancellationToken);
    Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExisteIdentificacionAsync(string identificacion, int? excluirId, CancellationToken cancellationToken);
    Task<int> CrearAsync(ClienteInput cliente, CancellationToken cancellationToken);
    Task<bool> ActualizarAsync(int id, ClienteInput cliente, CancellationToken cancellationToken);
}
