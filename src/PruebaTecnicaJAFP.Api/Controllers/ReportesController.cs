using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaJAFP.Business.Services;

namespace PruebaTecnicaJAFP.Api.Controllers;

[ApiController]
[Route("api/reportes")]
public sealed class ReportesController(ClienteService clientes) : ControllerBase
{
    [HttpGet("clientes/excel")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> ExportarClientesExcel(CancellationToken ct)
    {
        var registros = await clientes.ObtenerTodosAsync(ct);

        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add("Clientes");
        var encabezados = new[] { "Id", "Tipo identificación", "Identificación", "Razón social", "País", "Departamento", "Municipio" };

        for (var columna = 0; columna < encabezados.Length; columna++)
            hoja.Cell(1, columna + 1).Value = encabezados[columna];

        var fila = 2;
        foreach (var cliente in registros)
        {
            hoja.Cell(fila, 1).Value = cliente.ClnId;
            hoja.Cell(fila, 2).Value = TipoIdentificacion(cliente.ClnTipoId);
            hoja.Cell(fila, 3).Value = cliente.ClnNumeroIdentificacion;
            hoja.Cell(fila, 4).Value = cliente.ClnRazonSocial;
            hoja.Cell(fila, 5).Value = cliente.ClnPaisCodigo;
            hoja.Cell(fila, 6).Value = cliente.Departamento;
            hoja.Cell(fila, 7).Value = cliente.Municipio;
            fila++;
        }

        var rango = hoja.Range(1, 1, Math.Max(fila - 1, 1), encabezados.Length);
        rango.CreateTable("Clientes");
        hoja.SheetView.FreezeRows(1);
        hoja.Columns().AdjustToContents();
        hoja.Column(4).Width = Math.Min(Math.Max(hoja.Column(4).Width, 24), 45);
        hoja.Column(6).Width = Math.Min(Math.Max(hoja.Column(6).Width, 20), 35);
        hoja.Column(7).Width = Math.Min(Math.Max(hoja.Column(7).Width, 20), 35);

        await using var archivo = new MemoryStream();
        libro.SaveAs(archivo);
        return File(archivo.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"clientes-{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    private static string TipoIdentificacion(short tipoId) => tipoId switch
    {
        1 => "Cédula",
        2 => "NIT",
        3 => "Cédula de extranjería",
        _ => tipoId.ToString()
    };
}
