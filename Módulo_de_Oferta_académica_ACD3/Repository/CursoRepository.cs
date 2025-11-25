using Dapper;
using Módulo_de_Oferta_académica_ACD3.Entities;
using System.Data;

namespace Módulo_de_Oferta_académica_ACD3.Repository
{
    public class CursoRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public CursoRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Curso>> Obtener_Todos_Los_Cursos()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryAsync<Curso>(
                "Obtener_GetAll_Cursos",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<(Curso curso, string mensaje)> Obtener_Curso_Por_ID(string idCurso)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_ID_Curso", idCurso);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var curso = await connection.QueryFirstOrDefaultAsync<Curso>(
                "Obtener_Curso_Por_ID",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            var mensaje = parametros.Get<string>("p_Mensaje");
            return (curso, mensaje);
        }

        public async Task<IEnumerable<Curso>> Obtener_Cursos_Por_Carrera(string idCarrera)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_ID_Carrera", idCarrera);

            return await connection.QueryAsync<Curso>(
                "Obtener_Cursos_Por_Carrera",
                parametros,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<string> CRUD_CursosAsync(Curso curso)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Accion", curso.Accion);
            parametros.Add("p_ID_Curso", curso.ID_Curso);
            parametros.Add("p_ID_Carrera", curso.ID_Carrera);
            parametros.Add("p_Nivel", curso.Nivel);
            parametros.Add("p_Nombre", curso.Nombre);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "CRUD_Cursos",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            curso.Mensaje = parametros.Get<string>("p_Mensaje");
            return curso.Mensaje!;
        }
    }
}

