using Dapper;
using Módulo_de_Oferta_académica_ACD6.Entities;
using System.Data;

namespace Módulo_de_Oferta_académica_ACD6.Repository
{
    public class ProfesorRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public ProfesorRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Profesor>> Obtener_Todos()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryAsync<Profesor>(
                "Obtener_Todos_Profesores",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<(Profesor? profesor, string mensaje)> Obtener_Por_ID(string idProfesor)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var parametros = new DynamicParameters();
            parametros.Add("p_ID_Profesor", idProfesor);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 150, direction: ParameterDirection.Output);

            var profesor = await connection.QueryFirstOrDefaultAsync<Profesor>(
                "Obtener_Profesor_Por_ID",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            var mensaje = parametros.Get<string>("p_Mensaje");
            return (profesor, mensaje ?? "");
        }

        public async Task<string> CRUD_ProfesoresAsync(Profesor profesor)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var parametros = new DynamicParameters();
            parametros.Add("p_Accion", profesor.Accion);
            parametros.Add("p_ID_Profesor", profesor.ID_Profesor);
            parametros.Add("p_TipoIdentificacion", profesor.TipoIdentificacion);
            parametros.Add("p_NumeroIdentificacion", profesor.NumeroIdentificacion);
            parametros.Add("p_Email", profesor.Email);
            parametros.Add("p_Nombre", profesor.Nombre);
            parametros.Add("p_FechaNacimiento", profesor.FechaNacimiento);
            parametros.Add("p_Telefono", profesor.Telefono);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 150, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "CRUD_Profesores",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return parametros.Get<string>("p_Mensaje");
        }

        public async Task<IEnumerable<Profesor>> BuscarProfesoresAsync(string busqueda, string ordenCampo, string ordenDireccion, int pagina, int tamanoPagina)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            var parametros = new DynamicParameters();
            parametros.Add("p_Busqueda", busqueda);
            parametros.Add("p_OrdenCampo", ordenCampo);
            parametros.Add("p_OrdenDireccion", ordenDireccion);
            parametros.Add("p_Pagina", pagina);
            parametros.Add("p_TamanoPagina", tamanoPagina);

            return await connection.QueryAsync<Profesor>(
                "Buscar_Profesores_Paginado",
                parametros,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}

