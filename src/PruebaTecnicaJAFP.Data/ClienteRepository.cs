using System.Data;
using Microsoft.Data.SqlClient;
using PruebaTecnicaJAFP.Business.Contracts;
using PruebaTecnicaJAFP.Business.Models;

namespace PruebaTecnicaJAFP.Data;

public sealed class ClienteRepository(SqlConnectionFactory connectionFactory) : IClienteRepository
{
    private const string ConsultarTodos = "dbo.usp_Cliente_ConsultarTodos";
    private const string ConsultarPorId = "dbo.usp_Cliente_ConsultarPorId";

    public async Task<IReadOnlyList<Cliente>> ObtenerTodosAsync(CancellationToken ct)
    {
        var resultado = new List<Cliente>();
        await using var conexion = connectionFactory.Crear();
        await conexion.OpenAsync(ct);
        await using var comando = new SqlCommand(ConsultarTodos, conexion) { CommandType = CommandType.StoredProcedure };
        await using var lector = await comando.ExecuteReaderAsync(ct);
        while (await lector.ReadAsync(ct)) resultado.Add(Mapear(lector));
        return resultado;
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id, CancellationToken ct)
    {
        await using var conexion = connectionFactory.Crear();
        await conexion.OpenAsync(ct);
        await using var comando = new SqlCommand(ConsultarPorId, conexion) { CommandType = CommandType.StoredProcedure };
        comando.Parameters.AddWithValue("@ClnId", id);
        await using var lector = await comando.ExecuteReaderAsync(ct);
        return await lector.ReadAsync(ct) ? Mapear(lector) : null;
    }

    public async Task<bool> ExisteIdentificacionAsync(string identificacion, int? excluirId, CancellationToken ct)
    {
        await using var conexion = connectionFactory.Crear();
        await conexion.OpenAsync(ct);
        await using var comando = new SqlCommand("dbo.usp_Cliente_ExisteIdentificacion", conexion) { CommandType = CommandType.StoredProcedure };
        comando.Parameters.AddWithValue("@ClnNumeroIdentificacion", identificacion);
        comando.Parameters.AddWithValue("@ClnId", (object?)excluirId ?? DBNull.Value);
        return Convert.ToInt32(await comando.ExecuteScalarAsync(ct)) == 1;
    }

    public async Task<int> CrearAsync(ClienteInput cliente, CancellationToken ct)
    {
        await using var conexion = connectionFactory.Crear();
        await conexion.OpenAsync(ct);
        await using var comando = new SqlCommand("dbo.usp_Cliente_Crear", conexion) { CommandType = CommandType.StoredProcedure };
        comando.Parameters.AddWithValue("@ClnTipoId", cliente.ClnTipoId);
        comando.Parameters.AddWithValue("@ClnNumeroIdentificacion", cliente.ClnNumeroIdentificacion.Trim());
        comando.Parameters.AddWithValue("@ClnRazonSocial", cliente.ClnRazonSocial.Trim());
        comando.Parameters.AddWithValue("@ClnPaisCodigo", cliente.ClnPaisCodigo);
        comando.Parameters.AddWithValue("@ClnDptColCodigoDane", cliente.ClnDptColCodigoDane);
        comando.Parameters.AddWithValue("@ClnDvsPltColCodigoDane", cliente.ClnDvsPltColCodigoDane);
        var parametroId = comando.Parameters.Add("@ClnId", SqlDbType.Int);
        parametroId.Direction = ParameterDirection.Output;
        await comando.ExecuteNonQueryAsync(ct);
        return Convert.ToInt32(parametroId.Value);
    }

    public async Task<bool> ActualizarAsync(int id, ClienteInput cliente, CancellationToken ct)
    {
        await using var conexion = connectionFactory.Crear();
        await conexion.OpenAsync(ct);
        await using var comando = new SqlCommand("dbo.usp_Cliente_Actualizar", conexion) { CommandType = CommandType.StoredProcedure };
        comando.Parameters.AddWithValue("@ClnId", id);
        comando.Parameters.AddWithValue("@ClnTipoId", cliente.ClnTipoId);
        comando.Parameters.AddWithValue("@ClnNumeroIdentificacion", cliente.ClnNumeroIdentificacion.Trim());
        comando.Parameters.AddWithValue("@ClnRazonSocial", cliente.ClnRazonSocial.Trim());
        comando.Parameters.AddWithValue("@ClnPaisCodigo", cliente.ClnPaisCodigo);
        comando.Parameters.AddWithValue("@ClnDptColCodigoDane", cliente.ClnDptColCodigoDane);
        comando.Parameters.AddWithValue("@ClnDvsPltColCodigoDane", cliente.ClnDvsPltColCodigoDane);
        var parametroResultado = comando.Parameters.Add("@Resultado", SqlDbType.Int);
        parametroResultado.Direction = ParameterDirection.ReturnValue;
        await comando.ExecuteNonQueryAsync(ct);
        return Convert.ToInt32(parametroResultado.Value) == 1;
    }

    private static Cliente Mapear(SqlDataReader r) => new(r.GetInt32(0), r.GetInt16(1), r.GetString(2), r.GetString(3), r.GetInt16(4), r.GetInt32(5), r.GetInt32(6), r.GetString(7), r.GetString(8));
}
