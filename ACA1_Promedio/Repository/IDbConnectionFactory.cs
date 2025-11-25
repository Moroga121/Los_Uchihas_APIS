using System.Data;

namespace ACA1_Promedio.Repository
{
    public interface IDbConnectionFactory
    {

       IDbConnection CreateConnection();

    }
}
