using System.Data;

namespace MAT03_Expedientes.Repository
{
    public interface IDbConnectionFactory
    {

       IDbConnection CreateConnection();

    }
}
