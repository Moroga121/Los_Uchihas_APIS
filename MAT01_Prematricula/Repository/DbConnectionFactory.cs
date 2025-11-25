using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAT01_Prematricula.Repository
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
            return new  MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }


    }
}
