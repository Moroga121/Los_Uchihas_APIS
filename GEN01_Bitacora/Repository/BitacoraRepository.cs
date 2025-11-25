using Dapper;
using GEN01_Bitacora.Entities;
using System.Data;

namespace GEN01_Bitacora.Repository
{
    public class BitacoraRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        public BitacoraRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task Registrar_Bitacora(Entities.Bitacora bitacora)
        {
            try
            {
                using var connection = _dbConnectionFactory.CreateConnection();

                var parametros = new DynamicParameters();
                parametros.Add("pFecha", bitacora.Fecha, DbType.DateTime);
                parametros.Add("pUsuario", bitacora.Usuario, DbType.String);
                parametros.Add("pAccion", bitacora.Accion, DbType.String);
                parametros.Add("pDescripcion", bitacora.Descripcion, DbType.String);

                await connection.ExecuteAsync(
                    "sp_insertar_bitacora",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar el procedimiento: {ex.Message}");
            }
        }


        public async Task<IEnumerable<Bitacora>> Obtener_Todas_Las_Bitacoras()
        {
            try
            {
                using var connection = _dbConnectionFactory.CreateConnection();

                var bitacoras = await connection.QueryAsync<Bitacora>(
                    "Obtener_GetAll_Bitacoras",
                    commandType: CommandType.StoredProcedure
                );

                return bitacoras;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las bitácoras: {ex.Message}");
            }
        }
        public async Task<IEnumerable<Bitacora>> Obtener_Todas_Las_BitacorasFiltradas(DateOnly? fechaInicio, DateOnly? fechaFin, string? usuario, string? accion)
        {
            try
            {
                using var connection = _dbConnectionFactory.CreateConnection();

                var parametros = new DynamicParameters();
                parametros.Add("p_FechaInicio", fechaInicio, DbType.Date, ParameterDirection.Input);
                parametros.Add("p_FechaFinal", fechaFin, DbType.Date, ParameterDirection.Input);
                parametros.Add("p_Usuario", usuario, DbType.String, ParameterDirection.Input);
                parametros.Add("p_Accion", accion, DbType.String, ParameterDirection.Input);

                var bitacoras = await connection.QueryAsync<Bitacora>(
                    "Obtener_GetAll_BitacorasFiltrada",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                return bitacoras;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las bitácoras: {ex.Message}", ex);
            }
        }


    }
}
