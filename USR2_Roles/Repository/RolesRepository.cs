using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using USR2_Roles.Entities;

namespace USR2_Roles.Repository
{
    public class RolesRepository
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public RolesRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        #region "Tasks Obtener Roles"

        #region "Seleccionar Todos los Roles"

        public async Task<IEnumerable<Rol>> Obtener_Todos_Los_Usuarios()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<Rol>("Obtener_GetAll_Roles", commandType: CommandType.StoredProcedure);
            }
        }

        #endregion

        #region  "Seleccionar Usuarios por ID"

        public async Task<(Rol rol, string mensaje)> Obtener_Rol_Por_ID(string identificador_rol)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Identificador_Rol", identificador_rol);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var usuario = await connection.QueryFirstOrDefaultAsync<Rol>("Obtener_Rol_por_ID", parametros, commandType: CommandType.StoredProcedure);

            var mensaje = parametros.Get<string>("p_Mensaje");

            return (usuario, mensaje);
        }



        #endregion

        #endregion

        #region "Task CRUD Roles"

        public async Task<string> CRUD_RolesAsync(Rol rol)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Accion", rol.Accion);
            parametros.Add("p_Identificador_Rol", rol.Identificador_Rol);
            parametros.Add("p_Nombre_Rol", rol.Nombre_Rol);

            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("CRUD_Roles", parametros, commandType: CommandType.StoredProcedure);


            rol.Mensaje = parametros.Get<string>("p_Mensaje");
            return rol.Mensaje;

        }


        #endregion

        #region Asigna rol modulo
        public async Task<string> ActualizarPermisosRolAsync(string identificadorRol, List<string> modulos)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            // Convertir la lista de módulos en una cadena separada por comas
            var listaModulos = string.Join(",", modulos);

            var parametros = new DynamicParameters();
            parametros.Add("p_identificador_rol", identificadorRol);
            parametros.Add("p_lista_modulos", listaModulos);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            // Ejecutar el procedimiento almacenado
            await connection.ExecuteAsync("SP_ActualizarPermisosRol", parametros, commandType: CommandType.StoredProcedure);

            // Obtener el mensaje de salida
            var mensaje = parametros.Get<string>("p_Mensaje");

            return mensaje;
        }

        #endregion

    }
}
