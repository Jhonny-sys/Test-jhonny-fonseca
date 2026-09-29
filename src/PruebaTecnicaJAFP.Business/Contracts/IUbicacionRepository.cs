using PruebaTecnicaJAFP.Business.Models;

namespace PruebaTecnicaJAFP.Business.Contracts;

public interface IUbicacionRepository
{
    Task<IReadOnlyList<CatalogoItem>> ObtenerPaisesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<CatalogoItem>> ObtenerDepartamentosAsync(short paisCodigo, CancellationToken cancellationToken);
    Task<IReadOnlyList<CatalogoItem>> ObtenerCiudadesAsync(int departamentoCodigo, CancellationToken cancellationToken);
    Task<bool> EsUbicacionValidaAsync(short paisCodigo, int departamentoCodigo, int ciudadCodigo, CancellationToken cancellationToken);
}
