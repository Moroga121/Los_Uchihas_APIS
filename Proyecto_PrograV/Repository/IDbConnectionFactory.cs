using System.Data;

namespace Proyecto_PrograV.Repository
{
    public interface IDbConnectionFactory
    {

       IDbConnection CreateConnection();

    }
}
