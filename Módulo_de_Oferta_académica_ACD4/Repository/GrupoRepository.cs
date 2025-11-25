using Dapper;
using Módulo_de_Oferta_académica_ACD4.Entities;
using System.Data;

namespace Módulo_de_Oferta_académica_ACD4.Repository
{
    public class GrupoRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public GrupoRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Grupo>> Obtener_Todos_Los_Grupos()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryAsync<Grupo>(
                "Obtener_GetAll_Grupos",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<(Grupo grupo, string mensaje)> Obtener_Grupo_Por_ID(string idGrupo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_ID_Grupo", idGrupo);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var grupo = await connection.QueryFirstOrDefaultAsync<Grupo>(
                "Obtener_Grupo_Por_ID",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            var mensaje = parametros.Get<string>("p_Mensaje");
            return (grupo, mensaje);
        }

        public async Task<string> CRUD_GruposAsync(Grupo grupo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Accion", grupo.Accion);
            parametros.Add("p_ID_Grupo", grupo.ID_Grupo);
            parametros.Add("p_Numero_Grupo", grupo.Numero_Grupo);
            parametros.Add("p_ID_Curso", grupo.ID_Curso);
            parametros.Add("p_ID_Profesor", grupo.ID_Profesor);
            parametros.Add("p_Horario", grupo.Horario);
            parametros.Add("p_ID_Periodo", grupo.ID_Periodo);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "CRUD_Grupos",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return parametros.Get<string>("p_Mensaje");
        }

        public async Task<bool> CursoExisteAsync(string idCurso)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var result = await connection.ExecuteScalarAsync<int>(
                "Validar_Curso_Existe",
                new { p_ID_Curso = idCurso },
                commandType: CommandType.StoredProcedure
            );
            return result > 0;
        }

        public async Task<bool> ProfesorExisteAsync(string idProfesor)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var result = await connection.ExecuteScalarAsync<int>(
                "Validar_Profesor_Existe",
                new { p_ID_Profesor = idProfesor },
                commandType: CommandType.StoredProcedure
            );
            return result > 0;
        }

        public async Task<bool> PeriodoExisteAsync(string idPeriodo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var result = await connection.ExecuteScalarAsync<int>(
                "Validar_Periodo_Existe",
                new { p_ID_Periodo = idPeriodo },
                commandType: CommandType.StoredProcedure
            );
            return result > 0;
        }

        public async Task<bool> NumeroGrupoUnicoAsync(string idCurso, string idPeriodo, int numeroGrupo, string idGrupo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var result = await connection.ExecuteScalarAsync<int>(
                "Validar_Numero_Grupo_Unico",
                new { p_ID_Curso = idCurso, p_ID_Periodo = idPeriodo, p_Numero_Grupo = numeroGrupo, p_ID_Grupo = idGrupo },
                commandType: CommandType.StoredProcedure
            );
            return result == 0; 
        }
    }
}
