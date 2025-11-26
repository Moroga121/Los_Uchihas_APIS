using Dapper;
using MAT02_Matricula.Entities;
using System.Data;

namespace MAT02_Matricula.Repository
{
    public class MatriculaRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public MatriculaRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<IEnumerable<MatriculaCompleta>> Obtener_Todas_Matriculas()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var sql = "SP_ObtenerMatriculas";

                var resultado = await connection.QueryAsync<Matricula, Expediente_Estudiantes, MatriculaCompleta>(
                "SP_ObtenerMatriculas",
                (matricula, estudiante) => new MatriculaCompleta
                {
                    Id_matricula = matricula.Id_matricula,
                    Curso = matricula.curso,
                    Grupo = matricula.grupo,
                    estudiante = estudiante
                },
                splitOn: "numero_identificacion",
                commandType: CommandType.StoredProcedure
            );

                return resultado;
            }
        }


        public async Task<IEnumerable<MatriculaCompleta>> Obtener_Matriculados_Por_Curso_Grupo(string curso, string grupo)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var resultado = await connection.QueryAsync<Matricula, Expediente_Estudiantes, MatriculaCompleta>("SP_ObtenerMatriculadosPorCursoGrupo", (matricula, estudiante) => new MatriculaCompleta
                {
                    Id_matricula = matricula.Id_matricula,
                    Curso = matricula.curso,
                    Grupo = matricula.grupo,
                    estudiante = estudiante
                },
                    param: new
                    {
                        p_curso = curso,
                        p_grupo = grupo
                    },
                    splitOn: "numero_identificacion",
                    commandType: CommandType.StoredProcedure
                );

                return resultado;
            }
        }


        public async Task<(Matricula? creada, string mensaje)> CRUDMatricula(Matricula matricula)
        {
            try
            {
                using (var connection = _dbConnectionFactory.CreateConnection())
                {
                    var parametros = new DynamicParameters();

                    parametros.Add("p_accion", matricula.Accion, DbType.String, ParameterDirection.Input);
                    parametros.Add("p_id_matricula", matricula.Id_matricula, DbType.Int32, ParameterDirection.Input);
                    parametros.Add("p_numero_identificacion", matricula.numero_identificacion, DbType.String, ParameterDirection.Input);
                    parametros.Add("p_curso", matricula.curso, DbType.String, ParameterDirection.Input);
                    parametros.Add("p_grupo", matricula.grupo, DbType.String, ParameterDirection.Input);
                    parametros.Add("p_id_periodo", matricula.Id_periodo, DbType.String, ParameterDirection.Input);

                    parametros.Add("p_mensaje", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
                    parametros.Add("p_resultado", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var matriculaCreada = (await connection.QueryAsync<Matricula>(
                        "SP_CRUD_Matricula",
                        parametros,
                        commandType: CommandType.StoredProcedure
                    )).FirstOrDefault();

                    // Obtener los valores de salida
                    string mensaje = parametros.Get<string>("p_mensaje");
                    int resultado = parametros.Get<int>("p_resultado");

                    return (matriculaCreada, mensaje);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar " + ex.Message);
            }

        }

    }

}
