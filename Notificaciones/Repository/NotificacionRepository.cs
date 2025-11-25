using System.Data;
using Dapper;
using Notificaciones.Entities;

namespace Notificaciones.Repository
{
    public class NotificacionRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public NotificacionRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task CrearNotificacionAsync(string email, string asunto, string cuerpo, string estado)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Email", email);
            parametros.Add("p_Asunto", asunto);
            parametros.Add("p_Cuerpo", cuerpo);
            parametros.Add("p_Estado", estado);

            await connection.ExecuteAsync(
                "SP_Crear_Notificacion",
                parametros,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<List<ListaNotificaciones>> ObtenerNotificacionesAsync()
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var notificaciones = await connection.QueryAsync<ListaNotificaciones>(
                "SP_Obtener_Notificaciones",
                commandType: CommandType.StoredProcedure
            );

            return notificaciones.ToList();
        }

        public async Task<List<ListaNotificaciones>> ObtenerNotificacionesPorEmailAsync(string email)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var notificaciones = await connection.QueryAsync<ListaNotificaciones>(
                "SP_Obtener_Notificaciones_PorEmail",
                new { p_Email = email },
                commandType: CommandType.StoredProcedure
            );

            return notificaciones.ToList();
        }

    }
}
