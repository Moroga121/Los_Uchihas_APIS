using System.Data;

namespace Módulo_de_Ofertas_académica_ACD2.Repository
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
