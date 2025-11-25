using Dapper;
using Proyecto_PrograV.Entities;
using System.Data;

namespace Proyecto_PrograV.Repository
{
    public class UsuarioRepository
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public UsuarioRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }



        #region "Tasks Obtener Usuarios"

        #region "Obtener todos los Tipos de Identificación"

        public async Task<IEnumerable<Tipos_Identificacion>> Obtener_Tipos_De_Identificacion()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<Tipos_Identificacion>("Obtener_GetAll_TiposIdentificacion", commandType: CommandType.StoredProcedure);
            }
        }

        #endregion

        #region "Obtener Dominios"


        public async Task<IEnumerable<Usuario>> Obtener_Dominios()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<Usuario>("Obtener_GetAll_Dominios", commandType: CommandType.StoredProcedure);
            }
        }


        #endregion

        #region "Seleccionar Todos los Usuarios"

        public async Task<IEnumerable<Usuario>> Obtener_Todos_Los_Usuarios()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<Usuario>("Obtener_GetAll_Usuarios", commandType: CommandType.StoredProcedure);
            }
        }

        #endregion

        #region  "Seleccionar Usuarios por ID"

        public async Task<(Usuario usuario, string mensaje)> Obtener_Usuario_Por_Identificacion(string identificacion)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Identificacion", identificacion);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>("Obtener_Usuarios_por_Identificacion", parametros, commandType: CommandType.StoredProcedure);

            var mensaje = parametros.Get<string>("p_Mensaje");

            return (usuario, mensaje);
        }

        public async Task<(Usuario usuario, string mensaje)> Obtener_Usuario_Por_ID(string identificacion)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Identificacion", identificacion);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            var usuario = await connection.QueryFirstOrDefaultAsync<Usuario>("Obtener_Usuarios_por_ID", parametros, commandType: CommandType.StoredProcedure);

            var mensaje = parametros.Get<string>("p_Mensaje");

            return (usuario, mensaje);
        }



        #endregion

        #region "Filtrar USuarios"

        public async Task<(IEnumerable<Usuario> usuarios, string mensaje)> Obtener_Usuarios_FiltradosAsync(string identificacion, string nombre, string rol, string tipo, string dominio)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Identificacion", identificacion);
            parametros.Add("p_Nombre", nombre);
            parametros.Add("p_Rol", rol);
            parametros.Add("p_Tipo_Identificacion", tipo);
            parametros.Add("p_Dominio", dominio);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);


            var usuarios = await connection.QueryAsync<Usuario>("ObtenerUsuariosFiltrados", parametros, commandType: CommandType.StoredProcedure);

            var mensajeSP = parametros.Get<string>("p_Mensaje");

            return (usuarios, mensajeSP);
        }


        #endregion

        #endregion

        #region "Task CRUD Usuarios"

        public async Task<string> CRUD_UsuariosAsync(Usuario usuario)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Accion", usuario.Accion);
            parametros.Add("p_Identificacion", usuario.Identificacion);
            parametros.Add("p_Tipo_Identificacion", usuario.Tipo_Identificacion);
            parametros.Add("p_Nombre", usuario.Nombre);
            parametros.Add("p_Email", usuario.Email);
            parametros.Add("p_Contrasena", usuario.Contrasena);
            parametros.Add("p_Rol_Usuario", usuario.Rol_Usuario);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("CRUD_Usuarios", parametros, commandType: CommandType.StoredProcedure);



            usuario.Mensaje = parametros.Get<string>("p_Mensaje");
            return usuario.Mensaje;

        }


        #endregion

        #region "Task Cambiar Contraseña Usuario"

        public async Task<Usuario> CambiarContraAsync(Usuario usu)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Email", usu.Email);
            parametros.Add("p_NuevaContrasena", usu.Contrasena);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_ActualizarContrasena", parametros, commandType: CommandType.StoredProcedure);

            usu.Mensaje = parametros.Get<string>("p_Mensaje");

            return usu;
        }


        #endregion

    }
}
