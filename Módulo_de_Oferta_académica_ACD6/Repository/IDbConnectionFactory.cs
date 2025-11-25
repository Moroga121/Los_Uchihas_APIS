using System.Data;

namespace Módulo_de_Oferta_académica_ACD6.Repository
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
