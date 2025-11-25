
using MySql.Data.MySqlClient;
using System.Data;

namespace ACA1_Promedio.Repository
{
    public class DbConnectionFactory : IDbConnectionFactory
    {

        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

    }
}
