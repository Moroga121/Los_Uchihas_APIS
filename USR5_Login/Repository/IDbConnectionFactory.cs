using System.Data;

namespace USR5_Login.Repository
{
    public interface IDbConnectionFactory
    {

        IDbConnection CreateConnection();


    }
}
