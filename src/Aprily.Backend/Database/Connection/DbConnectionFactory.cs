using System.Data;

using Npgsql;

namespace Aprily.Backend.Database.Connection;

public class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("ReadConnection")
        ?? throw new InvalidOperationException("Connection string 'ReadConnection' not found.");

    public async Task<IDbConnection> CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}