using System.Data;
using Dapper;
using Pagos.Entities;

namespace Pagos.Repository
{
    public class PagosRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public PagosRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<Pago?> CrearPagoFacturaAsync(long id_Factura, decimal montoPago, string url)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parametros = new DynamicParameters();
            parametros.Add("p_numero_factura", id_Factura, DbType.Int64, ParameterDirection.Input);
            parametros.Add("p_Monto_Pago", montoPago, DbType.Decimal, ParameterDirection.Input);
            parametros.Add("p_Ruta_Comprobante", url, DbType.String, ParameterDirection.Input);
            parametros.Add("p_NumeroPago", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_Pago_Factura", parametros, commandType: CommandType.StoredProcedure);

            int numeroPago = parametros.Get<int>("p_NumeroPago");

            var pago = await connection.QueryFirstOrDefaultAsync<Pago>(
                "SP_Consultar_Pago",
                new { p_ID_Pago = numeroPago },
                commandType: CommandType.StoredProcedure
            );

            return pago;
        }



        public async Task<Pago?> ObtenerPagoFacturaPorNumeroAsync(int numeroPago)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_ID_Pago", numeroPago, DbType.Int64);

            var pago = await connection.QueryFirstOrDefaultAsync<Pago>(
                "SP_Consultar_Pago",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return pago;
        }

        public async Task<BuscarFacturaDto?> ObtenerFacturaPorNumeroAsync(int numeroFactura)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_ID_Factura", numeroFactura, DbType.Int64);

            var pago = await connection.QueryFirstOrDefaultAsync<BuscarFacturaDto>(
                "SP_Buscar_Factura",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return pago;
        }

        public async Task<IEnumerable<Pago>> ObtenerPagoFacturaAsync()
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var pago = await connection.QueryAsync<Pago>(
                "SP_Todo_Pagos",
                commandType: CommandType.StoredProcedure
            );

            return pago;
        }

        public async Task<IEnumerable<Pago>> ObtenerPagoFacturasPorPeriodoAsync(string periodo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_Periodo", periodo, DbType.String);

            var pagos = await connection.QueryAsync<Pago>(
                "SP_Consultar_Pago_Periodo",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return pagos;
        }


        public async Task<Pago> ReversarPagoAsync(int numeroPago, string motivo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_ID_Pago", numeroPago, DbType.Int64);
            parameters.Add("p_motivo", motivo, DbType.String);

            await connection.QueryFirstOrDefaultAsync<Pago>(
                "SP_Anular_Pago",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var pago = await connection.QueryFirstOrDefaultAsync<Pago>(
                "SP_Consultar_Pago",
                new { p_ID_Pago = numeroPago },
                commandType: CommandType.StoredProcedure
            );

            return pago;
        }
    }
}
