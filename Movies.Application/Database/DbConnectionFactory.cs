using Microsoft.Data.SqlClient;
using System.Data;


namespace Movies.Application.Database
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }

    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IDbConnection> CreateConnectionAsync()
        {
          var connection = new SqlConnection(_connectionString);
          await connection.OpenAsync();
          return connection;
        }
    }
}
