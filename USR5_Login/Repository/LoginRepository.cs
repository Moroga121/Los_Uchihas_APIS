using Dapper;
using System.Data;
using USR5_Login.Entities;

namespace USR5_Login.Repository
{
    public class LoginRepository
    {

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public LoginRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }


        #region "Task Login"

        public async Task<Login> ValidarUsuarioAsync(Login login)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_Email", login.Email);
            parametros.Add("p_Contrasena", login.Contrasena);
            parametros.Add("p_Mensaje", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_Login_Usuario", parametros, commandType: CommandType.StoredProcedure);

            login.Mensaje = parametros.Get<string>("p_Mensaje");

            return login;
        }

        #endregion

    }
}
