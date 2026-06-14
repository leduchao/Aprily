using System.Data;

using MySqlConnector;

namespace Aprily.Backend.Database.Connection;

public class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<IDbConnection> CreateConnection()
    {
        var connection = new MySqlConnection(_configuration.GetConnectionString("ReadConnection"));
        await connection.OpenAsync();

        return connection;
    }
}