using Dapper;
using Módulo_de_Oferta_académica_ACD1.Entities;
using Módulo_de_Oferta_académica_ACD1.Repository;
using MySql.Data.MySqlClient;
using System.Data;

namespace Módulo_de_Oferta_académica_ACD1.Repository
{
    public class InstitucionRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public InstitucionRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Institucion>> Obtener_Todas_Las_Instituciones()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryAsync<Institucion>(
                "Obtener_GetAll_Instituciones",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<(Institucion institucion, string mensaje)> Obtener_Institucion_Por_ID(string idInstitucion)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_ID_Institucion", idInstitucion);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var institucion = await connection.QueryFirstOrDefaultAsync<Institucion>(
                "Obtener_Institucion_Por_ID",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            var mensaje = parametros.Get<string>("p_Mensaje");
            return (institucion, mensaje);
        }

        public async Task<string> CRUD_InstitucionesAsync(Institucion institucion)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Accion", institucion.Accion);         
            parametros.Add("p_ID_Institucion", institucion.ID_Institucion);
            parametros.Add("p_Nombre", institucion.Nombre);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "CRUD_Instituciones",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            institucion.Mensaje = parametros.Get<string>("p_Mensaje");
            return institucion.Mensaje!;
        }

        public async Task<IEnumerable<Institucion>> Buscar_Instituciones_Por_Nombre(string nombre)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Nombre", nombre);

            var instituciones = await connection.QueryAsync<Institucion>(
                "Buscar_Instituciones_Por_Nombre",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return instituciones;
        }
    }
}
