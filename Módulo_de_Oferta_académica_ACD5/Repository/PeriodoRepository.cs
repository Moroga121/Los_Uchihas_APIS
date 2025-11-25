using Dapper;
using Módulo_de_Oferta_académica_ACD5.Entities;
using System.Data;

namespace Módulo_de_Oferta_académica_ACD5.Repository
{
    public class PeriodoRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public PeriodoRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Periodo>> Obtener_Todos_Los_Periodos()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryAsync<Periodo>(
                "Obtener_GetAll_Periodos",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<(Periodo periodo, string mensaje)> Obtener_Periodo_Por_ID(string idPeriodo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_ID_Periodo", idPeriodo);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var periodo = await connection.QueryFirstOrDefaultAsync<Periodo>(
                "Obtener_Periodo_Por_ID",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            var mensaje = parametros.Get<string>("p_Mensaje");

            if (periodo == null)
            {
                mensaje = string.IsNullOrEmpty(mensaje)
                    ? "No se encontró el periodo con el ID especificado."
                    : mensaje;
            }

            return (periodo, mensaje);
        }

        public async Task<string> CRUD_PeriodosAsync(Periodo periodo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Accion", periodo.Accion);
            parametros.Add("p_ID_Periodo", periodo.ID_Periodo);
            parametros.Add("p_Año", periodo.Año);
            parametros.Add("p_Numero_Periodo", periodo.Numero_Periodo);
            parametros.Add("p_Fecha_Inicio", periodo.Fecha_Inicio);
            parametros.Add("p_Fecha_Fin", periodo.Fecha_Fin);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 150, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "CRUD_Periodos",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return parametros.Get<string>("p_Mensaje");
        }
    }
}

