using System.Data;

namespace MAT02_Matricula.Repository
{
    public interface IDbConnectionFactory
    {

       IDbConnection CreateConnection();

    }
}
