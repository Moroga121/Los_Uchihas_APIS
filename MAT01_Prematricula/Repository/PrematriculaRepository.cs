using Dapper;
using MAT01_Prematricula.Entities;
using System;
using System.Data;

namespace MAT01_Prematricula.Repository
{
    public class PrematriculaRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public PrematriculaRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        #region Obtener Todas las Prematriculas
        public async Task<IEnumerable<Prematricula>> Obtener_Todas_Prematriculas()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var parametros = new DynamicParameters();
                parametros.Add("p_id_prematricula", null); 
                parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

                var resultado = await connection.QueryAsync<Prematricula>(
                    "SP_ObtenerPrematriculas",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                string mensaje = parametros.Get<string>("p_Mensaje");

                return resultado;
            }
        }
        #endregion

        #region Obtener Prematricula por ID
        public async Task<(Prematricula prematricula, string mensaje)> Obtener_Prematricula_Por_ID(string Id_Prematricula)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_id_prematricula", int.Parse(Id_Prematricula), dbType: DbType.Int32, direction: ParameterDirection.Input);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var prematricula = await connection.QueryFirstOrDefaultAsync<Prematricula>("SP_ObtenerPrematriculas", parametros, commandType: CommandType.StoredProcedure);

            var mensaje = parametros.Get<string>("p_Mensaje");

            return (prematricula, mensaje);
        }
        #endregion

        #region CRUD Prematricula
        public async Task<(Prematricula? creada, string mensaje)> CRUDPrematricula(Prematricula prematricula)
        {
            try
            {
                using (var connection = _dbConnectionFactory.CreateConnection())
                {
                    var parametros = new DynamicParameters();

                    // Parámetros de entrada
                    parametros.Add("p_accion", prematricula.Accion);
                    parametros.Add("p_id_prematricula", prematricula.id_prematricula);
                    parametros.Add("p_numero_identificacion", prematricula.numero_identificacion);
                    parametros.Add("p_carrera", prematricula.carrera);
                    parametros.Add("p_curso", prematricula.curso);
                    parametros.Add("p_observaciones", prematricula.observaciones);
                    parametros.Add("p_id_periodo", prematricula.Id_Periodo);

                    // Parámetros de salida
                    parametros.Add("p_mensaje", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
                    parametros.Add("p_resultado", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    // Ejecutar el procedimiento 
                    var prematriculaCreada = (await connection.QueryAsync<Prematricula>(
                        "SP_CRUD_Prematricula",
                        parametros,
                        commandType: CommandType.StoredProcedure
                    )).FirstOrDefault();

                    // Obtener los valores de salida
                    string mensaje = parametros.Get<string>("p_mensaje");
                    int resultado = parametros.Get<int>("p_resultado");

                    return (prematriculaCreada, mensaje);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar: " + ex.Message);
            }
        }

        #endregion
    }
}
