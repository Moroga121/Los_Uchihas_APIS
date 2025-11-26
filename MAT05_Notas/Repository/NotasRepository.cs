using Dapper;
using MAT05_Notas.Entities;
using System.Data;

namespace MAT05_Notas.Repository
{
    public class NotasRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public NotasRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<(string Mensaje, List<DesgloseRubro> Objetos)> Cargar_Desglose(DesgloseRubro desgloserubro)
        {
            try
            {
                using (var connection = _dbConnectionFactory.CreateConnection())
                {
                    var objetosCreados = new List<DesgloseRubro>();
                    string mensaje = "";

                    foreach (var rubro in desgloserubro.rubros)
                    {
                        var parametros = new DynamicParameters();

                        parametros.Add("p_accion", desgloserubro.Accion, DbType.String, ParameterDirection.Input);
                        parametros.Add("p_id_rubro", rubro.id_rubro, DbType.String, ParameterDirection.Input);
                        parametros.Add("p_grupo", desgloserubro.nombre_grupo, DbType.String, ParameterDirection.Input);
                        parametros.Add("p_curso", desgloserubro.nombre_curso, DbType.String, ParameterDirection.Input);
                        parametros.Add("p_nombre_rubro", rubro.nombre, DbType.String, ParameterDirection.Input);
                        parametros.Add("p_porcentaje", rubro.porcentaje, DbType.Decimal, ParameterDirection.Input);

                        parametros.Add("p_mensaje", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
                        parametros.Add("p_resultado", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        var rubrosCreados = (await connection.QueryAsync<Rubros>(
                            "SP_Cargar_Desglose",
                            parametros,
                            commandType: CommandType.StoredProcedure
                        )).ToList();

                        mensaje = parametros.Get<string>("p_mensaje");
                        int resultado = parametros.Get<int>("p_resultado");

                        // se debedetener si el resultado es 0
                        if (resultado == 0)
                        {
                            // No seguir procesando
                            return (mensaje, new List<DesgloseRubro>());
                        }

                        // ✔ Solo agregar cuando el SP devuelva objetos
                        if (rubrosCreados.Any())
                        {
                            objetosCreados.Add(new DesgloseRubro
                            {
                                nombre_curso = desgloserubro.nombre_curso,
                                nombre_grupo = desgloserubro.nombre_grupo,
                                rubros = rubrosCreados
                            });
                        }
                    }

                    return (mensaje, objetosCreados);
                }
            }

            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar el procedimiento: {ex.Message}");
            }
        }


        public async Task<(Notas? creada, string mensaje)> Asignar_Actualizar_Nota(Notas notas)
        {
            try
            {
                using (var connection = _dbConnectionFactory.CreateConnection())
                {
                    var parametros = new DynamicParameters();

                        parametros.Add("p_accion", notas.Accion, DbType.String, ParameterDirection.Input);
                        parametros.Add("p_id_nota", notas.id_nota, DbType.Int32, ParameterDirection.Input);
                        parametros.Add("p_numero_identificacion", notas.numero_identificacion, DbType.String, ParameterDirection.Input);
                        parametros.Add("p_id_rubro", notas.id_rubro, DbType.String, ParameterDirection.Input);
                        parametros.Add("p_valor", notas.valor, DbType.Decimal, ParameterDirection.Input);

                        parametros.Add("p_mensaje", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
                        parametros.Add("p_resultado", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var notaCreada = (await connection.QueryAsync<Notas>(
                          "SP_AsignarNota",
                            parametros,
                            commandType: CommandType.StoredProcedure
                        )).FirstOrDefault();

                    // Obtener los valores de salida
                    string mensaje = parametros.Get<string>("p_mensaje");
                    int resultado = parametros.Get<int>("p_resultado");

                    return (notaCreada, mensaje);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar el procedimiento: {ex.Message}");
            }

        }
        public async Task<(DesgloseRubro desgloserubro, string mensaje)> Obtener_Desglose_Por_ID(string grupo, string curso)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_grupo", grupo, DbType.String, ParameterDirection.Input);
            parametros.Add("p_curso", curso, DbType.String, ParameterDirection.Input);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var rubros = (await connection.QueryAsync<Rubros>(
                "SP_ObtenerDesglose",
                parametros,
                commandType: CommandType.StoredProcedure
            )).ToList();

            var mensaje = parametros.Get<string>("p_Mensaje");

            var desglose = new DesgloseRubro
            {
                nombre_curso = curso,
                nombre_grupo = grupo,
                rubros = rubros
            };

            return (desglose, mensaje);
        }

        public async Task<(IEnumerable<Notas> notas, string mensaje)> Obtener_Notas_By_Id(string numero_identificacion, string curso)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_numero_identificacion", numero_identificacion, DbType.String, ParameterDirection.Input);
            parametros.Add("p_curso", curso, DbType.String, ParameterDirection.Input);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            // Ejecutamos el SP y obtenemos todos los registros
            var notas_sp = await connection.QueryAsync<Notas>(
                "SP_Obtener_Notas_Estudiante",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            var mensaje = parametros.Get<string>("p_Mensaje");

            return (notas_sp, mensaje);
        }



    }
}
