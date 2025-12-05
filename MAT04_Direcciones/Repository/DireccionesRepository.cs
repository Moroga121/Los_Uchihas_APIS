using Dapper;
using MAT04_Direcciones.Entities;
using System.Data;

namespace MAT04_Direcciones.Repository
{
    public class DireccionesRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public DireccionesRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Provincias>> Obtener_Todos_Provincias()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var parametros = new DynamicParameters();
                parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

                var provincias = await connection.QueryAsync<Provincias>("SP_Obtener_Provincias", parametros, commandType: CommandType.StoredProcedure);  
                
                string mensaje = parametros.Get<string>("p_Mensaje");

                return provincias;  
            }
        }
        //SP_Obtener_Cantones
        public async Task<(IEnumerable<cantones> _cantones, string mensaje)> Obtener_Cantones_Por_Provincia(string provincia)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_provincia", provincia.ToLower(), DbType.String, ParameterDirection.Input);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var cantones = await connection.QueryAsync<cantones>(
                "SP_Obtener_Cantones",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            var mensaje = parametros.Get<string>("p_Mensaje");

            return (cantones, mensaje);
        }
        public async Task<(IEnumerable<distritos> _distritos, string mensaje)> Obtener_Distritos_Por_Canton_Provincia(string provincia, string canton)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_provincia", provincia.ToLower(), DbType.String, ParameterDirection.Input);
            parametros.Add("p_canton", canton.ToLower(), DbType.String, ParameterDirection.Input);

            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var cantones = await connection.QueryAsync<distritos>(
                "SP_Obtener_Distritos",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            var mensaje = parametros.Get<string>("p_Mensaje");

            return (cantones, mensaje);
        }




        public async Task<IEnumerable<Direcciones>> Obtener_Direccion_Expediente(int id_distrito)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                var parametros = new DynamicParameters();
                parametros.Add("p_distrito", id_distrito, DbType.Int32, ParameterDirection.Input);

                var direccion = await connection.QueryAsync<Direcciones>("SP_Direccion_Expediente", parametros, commandType: CommandType.StoredProcedure);

                return direccion;
            }
        }

    }
}
