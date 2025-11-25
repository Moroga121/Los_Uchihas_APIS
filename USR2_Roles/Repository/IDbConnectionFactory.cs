using System.Data;

namespace USR2_Roles.Repository
{
    public interface IDbConnectionFactory
    {

       IDbConnection CreateConnection();

    }
}
