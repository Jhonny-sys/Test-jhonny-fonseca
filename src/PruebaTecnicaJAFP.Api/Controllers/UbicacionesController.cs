using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaJAFP.Business.Contracts;
using PruebaTecnicaJAFP.Business.Models;

namespace PruebaTecnicaJAFP.Api.Controllers;

[ApiController]
[Route("api/ubicaciones")]
public sealed class UbicacionesController(IUbicacionRepository ubicaciones) : ControllerBase
{
    [HttpGet("paises")]
    public Task<IReadOnlyList<CatalogoItem>> Paises(CancellationToken ct) => ubicaciones.ObtenerPaisesAsync(ct);

    [HttpGet("paises/{paisCodigo}/departamentos")]
    public Task<IReadOnlyList<CatalogoItem>> Departamentos(short paisCodigo, CancellationToken ct) => ubicaciones.ObtenerDepartamentosAsync(paisCodigo, ct);

    [HttpGet("departamentos/{departamentoCodigo:int}/ciudades")]
    public Task<IReadOnlyList<CatalogoItem>> Ciudades(int departamentoCodigo, CancellationToken ct) => ubicaciones.ObtenerCiudadesAsync(departamentoCodigo, ct);
}
