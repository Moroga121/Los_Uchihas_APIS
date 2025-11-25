using System.Data;

namespace USR4_Modulos.Repository
{
    public interface IDbConnectionFactory
    {

        IDbConnection CreateConnection();

    }
}
