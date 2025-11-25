using Dapper;
using Módulo_de_Ofertas_académica_ACD2.Entities;
using System.Data;

namespace Módulo_de_Ofertas_académica_ACD2.Repository
{
    public class CarreraRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public CarreraRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Carrera>> Obtener_Todas_Las_Carreras()
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            return await connection.QueryAsync<Carrera>(
                "Obtener_GetAll_Carreras",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<(Carrera carrera, string mensaje)> Obtener_Carrera_Por_ID(string idCarrera)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_ID_Carrera", idCarrera);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var carrera = await connection.QueryFirstOrDefaultAsync<Carrera>(
                "Obtener_Carrera_Por_ID",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            var mensaje = parametros.Get<string>("p_Mensaje");
            return (carrera, mensaje);
        }

        public async Task<IEnumerable<Carrera>> Obtener_Carreras_Por_Institucion(string idInstitucion)
        {
            if (string.IsNullOrWhiteSpace(idInstitucion))
                throw new ArgumentException("El ID de institución no puede estar vacío.");

            if (idInstitucion.Length > 10)
                throw new ArgumentException("El ID de institución no debe superar los 10 caracteres.");

            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_ID_Institucion", idInstitucion);

            return await connection.QueryAsync<Carrera>(
                "Obtener_Carreras_Por_Institucion",
                parametros,
                commandType: CommandType.StoredProcedure
            );
        }


        public async Task<string> CRUD_CarrerasAsync(Carrera carrera)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Accion", carrera.Accion);
            parametros.Add("p_ID_Carrera", carrera.ID_Carrera);
            parametros.Add("p_Nombre", carrera.Nombre);
            parametros.Add("p_ID_Institucion", carrera.ID_Institucion);
            parametros.Add("p_ID_Director", carrera.ID_Director);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "CRUD_Carreras",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            carrera.Mensaje = parametros.Get<string>("p_Mensaje");
            return carrera.Mensaje!;
        }
    }
}
