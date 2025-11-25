using Dapper;
using System.Data;
using USR3_Parametrización.Entities;

namespace USR3_Parametrización.Repository
{
    public class ParametrizacionRepository
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public ParametrizacionRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        #region "Tasks Obtener Parametros"

        #region "Seleccionar Todos los Parametros"

        public async Task<IEnumerable<Parametrizacion>> Obtener_Todos_Los_Parametros()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<Parametrizacion>("Obtener_GetAll_Parametros", commandType: CommandType.StoredProcedure);
            }
        }

        #endregion

        #region  "Seleccionar Usuarios por Parametros"

        public async Task<(Parametrizacion parametro, string mensaje)> Obtener_Parametro_Por_ID(string identificador_parametro)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Identificador_Parametro", identificador_parametro);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var usuario = await connection.QueryFirstOrDefaultAsync<Parametrizacion>("Obtener_Parametro_por_ID", parametros, commandType: CommandType.StoredProcedure);

            var mensaje = parametros.Get<string>("p_Mensaje");

            return (usuario, mensaje);
        }



        #endregion

        #endregion

        #region "Task CRUD Parametrización"

        public async Task<string> CRUD_ParametrizacionAsync(Parametrizacion parametrizacion)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Accion", parametrizacion.Accion);
            parametros.Add("p_Identificador_Parametro", parametrizacion.Identificador_Parametro);
            parametros.Add("p_Valor_Parametro", parametrizacion.Valor_Parametro);

            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("CRUD_Parametrizacion", parametros, commandType: CommandType.StoredProcedure);

            

            parametrizacion.Mensaje = parametros.Get<string>("p_Mensaje");
            return parametrizacion.Mensaje;

        }


        #endregion

    }
}
