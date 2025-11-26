using Dapper;
using MAT03_Expedientes.Entities;
using System.Data;

namespace MAT03_Expedientes.Repository
{
    public class Expediente_EstudiantesRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public Expediente_EstudiantesRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<IEnumerable<Expediente_Estudiantes>> Obtener_Todos_Expedientes()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var parametros = new DynamicParameters();
                parametros.Add("p_numero_identificacion", null);
                parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

                var resultado = await connection.QueryAsync<Expediente_Estudiantes>(
                    "SP_ObtenerExpediente",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                string mensaje = parametros.Get<string>("p_Mensaje");

                return resultado;
            }
        }



        public async Task<(Expediente_Estudiantes _expedientes_estudiantes, string mensaje)> Obtener_Expediente_Por_ID(string Id_expediente_estudiante)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_numero_identificacion", Id_expediente_estudiante, dbType: DbType.String, direction: ParameterDirection.Input);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var expediente = await connection.QueryFirstOrDefaultAsync<Expediente_Estudiantes>("SP_ObtenerExpediente", parametros, commandType: CommandType.StoredProcedure);

            var mensaje = parametros.Get<string>("p_Mensaje");

            return (expediente, mensaje);
        }

        public async Task<(Expediente_Estudiantes? creado, string mensaje)> CRUDExpediente(Expediente_Estudiantes expediente_estudiantes)
        {
            try
            {
                using (var connection = _dbConnectionFactory.CreateConnection())
                {
                    var parametros = new DynamicParameters();

                    parametros.Add("p_accion", expediente_estudiantes.Accion, DbType.String, ParameterDirection.Input);
                    parametros.Add("p_numero_identificacion", expediente_estudiantes.numero_identificacion, DbType.String, ParameterDirection.Input);
                    parametros.Add("p_tipo_identificacion", expediente_estudiantes.tipo_identificacion, DbType.String, ParameterDirection.Input);
                    parametros.Add("p_email", expediente_estudiantes.email, DbType.String, ParameterDirection.Input);
                    parametros.Add("p_nombre", expediente_estudiantes.nombre, DbType.String, ParameterDirection.Input);
                    parametros.Add("p_fecha_nacimiento", expediente_estudiantes.fecha_nacimiento, DbType.Date, ParameterDirection.Input);
                    parametros.Add("p_id_distrito", expediente_estudiantes.id_distrito, DbType.Int32, ParameterDirection.Input);
                    parametros.Add("p_otras_senas", expediente_estudiantes.otras_senas, DbType.String, ParameterDirection.Input);
                    parametros.Add("p_telefono", expediente_estudiantes.telefono, DbType.String, ParameterDirection.Input);

                    parametros.Add("p_mensaje", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
                    parametros.Add("p_resultado", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var expedienteCreado = (await connection.QueryAsync<Expediente_Estudiantes>(
                        "SP_CRUD_Expediente",
                        parametros,
                        commandType: CommandType.StoredProcedure
                    )).FirstOrDefault();

                    // Obtener los valores de salida
                    string mensaje = parametros.Get<string>("p_mensaje");
                    int resultado = parametros.Get<int>("p_resultado");

                    return (expedienteCreado, mensaje);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar " + ex.Message);
            }

        }

    }
}