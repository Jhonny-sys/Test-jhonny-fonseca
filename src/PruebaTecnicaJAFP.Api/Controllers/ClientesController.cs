using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaJAFP.Business.Models;
using PruebaTecnicaJAFP.Business.Services;

namespace PruebaTecnicaJAFP.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public sealed class ClientesController(ClienteService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Cliente>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<Cliente>> ObtenerTodos(CancellationToken ct) => service.ObtenerTodosAsync(ct);

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Cliente), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Cliente>> ObtenerPorId(int id, CancellationToken ct)
    {
        var cliente = await service.ObtenerPorIdAsync(id, ct);
        return cliente is null ? NotFound() : Ok(cliente);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Cliente), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Cliente>> Crear(ClienteInput input, CancellationToken ct)
    {
        var id = await service.CrearAsync(input, ct);
        var cliente = await service.ObtenerPorIdAsync(id, ct);
        return CreatedAtAction(nameof(ObtenerPorId), new { id }, cliente);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(int id, ClienteInput input, CancellationToken ct)
    {
        return await service.ActualizarAsync(id, input, ct) ? NoContent() : NotFound();
    }
}
