using System.Data;
using ACA2_Historial.Entities;
using Dapper;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACA2_Historial.Repository
{
    public class HistorialRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public HistorialRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }



        public async Task<IEnumerable<Estudiante_Matriculado>> ObtenerEstudiantesMatriculaPeriodoAsync(string Periodo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_Periodo", Periodo, DbType.String);

            var estudiantes = await connection.QueryAsync<Estudiante_Matriculado>(
                "SP_ACA2",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return estudiantes;
        }

        public async Task<List<MatriculaDto>> ObtenerTodasMatriculasAsync()
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var matriculas = await connection.QueryAsync<MatriculaDto>(
                "SP_Obtener_Matriculas",
                commandType: CommandType.StoredProcedure
            );

            return matriculas.ToList();
        }
    }
}
