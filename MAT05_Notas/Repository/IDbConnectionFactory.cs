using System.Data;

namespace MAT05_Notas.Repository
{
    public interface IDbConnectionFactory
    {

       IDbConnection CreateConnection();

    }
}
