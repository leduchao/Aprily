using System.Data;

namespace Aprily.Backend.Database.Connection;

public interface IDbConnectionFactory
{
    public Task<IDbConnection> CreateConnection();
}
