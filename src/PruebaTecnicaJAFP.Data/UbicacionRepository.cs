using Microsoft.Data.SqlClient;
using PruebaTecnicaJAFP.Business.Contracts;
using PruebaTecnicaJAFP.Business.Models;

namespace PruebaTecnicaJAFP.Data;

public sealed class UbicacionRepository(SqlConnectionFactory connectionFactory) : IUbicacionRepository
{
    public Task<IReadOnlyList<CatalogoItem>> ObtenerPaisesAsync(CancellationToken ct) =>
        ConsultarAsync("SELECT PaisCodigo, PaisNombre FROM dbo.Pais ORDER BY PaisNombre", null, ct);

    public Task<IReadOnlyList<CatalogoItem>> ObtenerDepartamentosAsync(short paisCodigo, CancellationToken ct) =>
        ConsultarAsync("SELECT DptColCodigoDane, DptColNombredelDepartamento FROM dbo.DepartamentosColombia WHERE DptColPaisCodigo=@codigo ORDER BY DptColNombredelDepartamento", paisCodigo, ct);

    public Task<IReadOnlyList<CatalogoItem>> ObtenerCiudadesAsync(int departamentoCodigo, CancellationToken ct) =>
        ConsultarAsync("SELECT DvsPltColCodigoDane, DvsPltColNombreMunicipio FROM dbo.DivisionPoliticaColombia WHERE DvsPltColDptColCodigoDane=@codigo ORDER BY DvsPltColNombreMunicipio", departamentoCodigo, ct);

    public async Task<bool> EsUbicacionValidaAsync(short paisCodigo, int departamentoCodigo, int ciudadCodigo, CancellationToken ct)
    {
        const string sql = "SELECT IIF(EXISTS(SELECT 1 FROM dbo.Pais p INNER JOIN dbo.DepartamentosColombia d ON d.DptColPaisCodigo=p.PaisCodigo INNER JOIN dbo.DivisionPoliticaColombia c ON c.DvsPltColDptColCodigoDane=d.DptColCodigoDane WHERE p.PaisCodigo=@pais AND d.DptColCodigoDane=@departamento AND c.DvsPltColCodigoDane=@ciudad), 1, 0)";
        await using var conexion = connectionFactory.Crear();
        await conexion.OpenAsync(ct);
        await using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@pais", paisCodigo);
        comando.Parameters.AddWithValue("@departamento", departamentoCodigo);
        comando.Parameters.AddWithValue("@ciudad", ciudadCodigo);
        return Convert.ToInt32(await comando.ExecuteScalarAsync(ct)) == 1;
    }

    private async Task<IReadOnlyList<CatalogoItem>> ConsultarAsync(string sql, object? codigo, CancellationToken ct)
    {
        var resultado = new List<CatalogoItem>();
        await using var conexion = connectionFactory.Crear();
        await conexion.OpenAsync(ct);
        await using var comando = new SqlCommand(sql, conexion);
        if (codigo is not null) comando.Parameters.AddWithValue("@codigo", codigo);
        await using var lector = await comando.ExecuteReaderAsync(ct);
        while (await lector.ReadAsync(ct))
            resultado.Add(new CatalogoItem(Convert.ToInt32(lector.GetValue(0)), lector.GetString(1)));
        return resultado;
    }
}
