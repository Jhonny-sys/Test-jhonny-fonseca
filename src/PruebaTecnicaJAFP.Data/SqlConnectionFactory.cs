using Microsoft.Data.SqlClient;

namespace PruebaTecnicaJAFP.Data;

public sealed class SqlConnectionFactory(string connectionString)
{
    public SqlConnection Crear() => new(connectionString);
}
