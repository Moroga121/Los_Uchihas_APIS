using Dapper;
using System.Data;
using USR4_Modulos.Entities;

namespace USR4_Modulos.Repository
{
    public class ModuloRepository
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public ModuloRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        #region "Tasks Obtener Moodulos"

        #region "Seleccionar Todos los Modulos"

        public async Task<IEnumerable<Modulos>> Obtener_Todos_Los_Modulos()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<Modulos>("Obtener_GetAll_Modulos", commandType: CommandType.StoredProcedure);
            }
        }

        #endregion
        #region "Seleccionar Todos los roles y modulos relacionados"

        public async Task<IEnumerable<Rol_Modulo>> Obtener_Todos_Los_Rol_Usuario()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<Rol_Modulo>("Obtener_GetAll_Roles_Usuarios", commandType: CommandType.StoredProcedure);
            }
        }

        #endregion

        #region  "Seleccionar Usuarios por ID_Modulo"

        public async Task<(Modulos modulo, string mensaje)> Obtener_Modulo_Por_ID(string identificador_modulo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Identificador_Modulo", identificador_modulo);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var usuario = await connection.QueryFirstOrDefaultAsync<Modulos>("Obtener_Modulo_por_ID", parametros, commandType: CommandType.StoredProcedure);

            var mensaje = parametros.Get<string>("p_Mensaje");

            return (usuario, mensaje);
        }



        #endregion
        #region  "Seleccionar Usuarios por Rol"

        public async Task<(IEnumerable<Modulos>, string mensaje)> Obtener_Modulo_Por_Rol(string rol)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_ID_Rol", rol);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var usuarios = await connection.QueryAsync<Modulos>("Obtener_Modulo_por_Rol", parametros, commandType: CommandType.StoredProcedure);

            var mensaje = parametros.Get<string>("p_Mensaje");

            return (usuarios, mensaje);
        }



        #endregion
        #endregion

        #region "Task CRUD Modulos"

        public async Task<string> CRUD_ModulosAsync(Modulos modulo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Accion", modulo.Accion);
            parametros.Add("p_Identificador_Modulo", modulo.Identificador_Modulo);
            parametros.Add("p_Nombre_Modulo", modulo.Nombre_Modulo);
            parametros.Add("p_Estado", modulo.Estado);
            parametros.Add("p_Orden", modulo.Orden);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            try
            {
                await connection.ExecuteAsync("CRUD_Modulos", parametros, commandType: CommandType.StoredProcedure);
                modulo.Mensaje = parametros.Get<string>("p_Mensaje");
                return modulo.Mensaje;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error ejecutando SP: " + ex.Message);
                throw; // o devuelve ex.Message
            }

        }


        #endregion


        #region Actualizar permisos de módulo
        public async Task<string> ActualizarPermisosModuloAsync(string moduloId, List<string> rolesAsignados)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            // Convertir la lista de roles asignados en una cadena separada por comas
            var listaRoles = string.Join(",", rolesAsignados);

            var parametros = new DynamicParameters();
            parametros.Add("p_identificador_modulo", moduloId);
            parametros.Add("p_lista_roles", listaRoles);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            // Ejecutar el procedimiento almacenado
            await connection.ExecuteAsync("SP_ActualizarPermisosRoles", parametros, commandType: CommandType.StoredProcedure);

            // Obtener el mensaje de salida
            var mensaje = parametros.Get<string>("p_Mensaje");
            return mensaje;
        }
        #endregion
    }
}
