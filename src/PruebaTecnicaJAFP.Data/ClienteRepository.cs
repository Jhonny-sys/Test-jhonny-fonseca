using Microsoft.Data.SqlClient;
using PruebaTecnicaJAFP.Business.Contracts;
using PruebaTecnicaJAFP.Business.Models;

namespace PruebaTecnicaJAFP.Data;

public sealed class ClienteRepository(SqlConnectionFactory connectionFactory) : IClienteRepository
{
    private const string SeleccionCliente = "SELECT c.ClnId, c.ClnTipoId, c.ClnNumeroIdentificacion, c.ClnRazonSocial, c.ClnPaisCodigo, c.ClnDptColCodigoDane, c.ClnDvsPltColCodigoDane, d.DptColNombredelDepartamento, m.DvsPltColNombreMunicipio FROM dbo.Cliente c INNER JOIN dbo.DepartamentosColombia d ON d.DptColCodigoDane=c.ClnDptColCodigoDane INNER JOIN dbo.DivisionPoliticaColombia m ON m.DvsPltColCodigoDane=c.ClnDvsPltColCodigoDane";

    public async Task<IReadOnlyList<Cliente>> ObtenerTodosAsync(CancellationToken ct)
    {
        var sql = $"{SeleccionCliente} ORDER BY c.ClnRazonSocial";
        var resultado = new List<Cliente>();
        await using var conexion = connectionFactory.Crear();
        await conexion.OpenAsync(ct);
        await using var comando = new SqlCommand(sql, conexion);
        await using var lector = await comando.ExecuteReaderAsync(ct);
        while (await lector.ReadAsync(ct)) resultado.Add(Mapear(lector));
        return resultado;
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken ct)
    {
        var sql = $"{SeleccionCliente} WHERE c.ClnId = @id";
        await using var conexion = connectionFactory.Crear();
        await conexion.OpenAsync(ct);
        await using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);
        await using var lector = await comando.ExecuteReaderAsync(ct);
        return await lector.ReadAsync(ct) ? Mapear(lector) : null;
    }

    public async Task<bool> ExisteIdentificacionAsync(string identificacion, int? excluirId, CancellationToken ct)
    {
        const string sql = "SELECT IIF(EXISTS(SELECT 1 FROM dbo.Cliente WHERE ClnNumeroIdentificacion = @identificacion AND (@excluirId IS NULL OR ClnId <> @excluirId)), 1, 0)";
        await using var conexion = connectionFactory.Crear();
        await conexion.OpenAsync(ct);
        await using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@identificacion", identificacion);
        comando.Parameters.AddWithValue("@excluirId", (object?)excluirId ?? DBNull.Value);
        return Convert.ToInt32(await comando.ExecuteScalarAsync(ct)) == 1;
    }

    public async Task<int> CrearAsync(ClienteInput cliente, CancellationToken ct)
    {
        const string sql = "INSERT INTO dbo.Cliente (ClnTipoId, ClnNumeroIdentificacion, ClnRazonSocial, ClnPaisCodigo, ClnDptColCodigoDane, ClnDvsPltColCodigoDane) OUTPUT INSERTED.ClnId VALUES (@tipoId, @identificacion, @razonSocial, @pais, @departamento, @ciudad)";
        await using var conexion = connectionFactory.Crear();
        await conexion.OpenAsync(ct);
        await using var comando = CrearComando(sql, conexion, cliente);
        return Convert.ToInt32(await comando.ExecuteScalarAsync(ct));
    }

    public async Task<bool> ActualizarAsync(int id, ClienteInput cliente, CancellationToken ct)
    {
        const string sql = "UPDATE dbo.Cliente SET ClnTipoId=@tipoId, ClnNumeroIdentificacion=@identificacion, ClnRazonSocial=@razonSocial, ClnPaisCodigo=@pais, ClnDptColCodigoDane=@departamento, ClnDvsPltColCodigoDane=@ciudad WHERE ClnId=@id";
        await using var conexion = connectionFactory.Crear();
        await conexion.OpenAsync(ct);
        await using var comando = CrearComando(sql, conexion, cliente);
        comando.Parameters.AddWithValue("@id", id);
        return await comando.ExecuteNonQueryAsync(ct) == 1;
    }

    private static SqlCommand CrearComando(string sql, SqlConnection conexion, ClienteInput cliente)
    {
        var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@tipoId", cliente.ClnTipoId);
        comando.Parameters.AddWithValue("@identificacion", cliente.ClnNumeroIdentificacion.Trim());
        comando.Parameters.AddWithValue("@razonSocial", cliente.ClnRazonSocial.Trim());
        comando.Parameters.AddWithValue("@pais", cliente.ClnPaisCodigo);
        comando.Parameters.AddWithValue("@departamento", cliente.ClnDptColCodigoDane);
        comando.Parameters.AddWithValue("@ciudad", cliente.ClnDvsPltColCodigoDane);
        return comando;
    }

    private static Cliente Mapear(SqlDataReader r) => new(r.GetInt32(0), r.GetInt16(1), r.GetString(2), r.GetString(3), r.GetInt16(4), r.GetInt32(5), r.GetInt32(6), r.GetString(7), r.GetString(8));
}
