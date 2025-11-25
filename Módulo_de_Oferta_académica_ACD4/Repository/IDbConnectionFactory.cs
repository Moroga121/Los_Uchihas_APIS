using System.Data;

namespace Módulo_de_Oferta_académica_ACD4.Repository
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
