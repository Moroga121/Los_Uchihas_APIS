using System.Data;

namespace USR3_Parametrización.Repository
{
    public interface IDbConnectionFactory
    {

       IDbConnection CreateConnection();

    }
}
