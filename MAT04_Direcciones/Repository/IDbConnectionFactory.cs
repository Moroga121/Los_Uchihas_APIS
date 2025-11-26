using System.Data;

namespace MAT04_Direcciones.Repository
{
    public interface IDbConnectionFactory
    {

       IDbConnection CreateConnection();

    }
}
