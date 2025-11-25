using System.Data;
using Dapper;
using Facturacion.Entities;

namespace Facturacion.Repository
{
    public class FacturaRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public FacturaRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<Factura?> CrearFacturaAsync(string idEstudiante, decimal montoBase, decimal iva, decimal montoTotal, string detalle, string periodo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            { 
            var parametros = new DynamicParameters();
            parametros.Add("p_ID_Estudiante", idEstudiante);
            parametros.Add("p_MontoBase", montoBase);
            parametros.Add("p_IVA", iva);
            parametros.Add("p_MontoTotal", montoTotal);
            parametros.Add("p_Periodo", periodo);
            parametros.Add("p_Detalle", detalle);
            parametros.Add("p_NumeroFactura", dbType: DbType.Int64, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("SP_Crear_Factura", parametros, commandType: CommandType.StoredProcedure);

            long numeroFactura = parametros.Get<long>("p_NumeroFactura");

            // 2️⃣ Consultar la factura recién creada con todos los datos
            var factura = await connection.QueryFirstOrDefaultAsync<Factura>(
                "SP_Consultar_Factura",
                new { p_ID_Factura = numeroFactura },
                commandType: CommandType.StoredProcedure
            );

            return factura;
        }
       }

        public async Task<Factura?> ObtenerFacturaPorNumeroAsync(long numeroFactura)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_ID_Factura", numeroFactura, DbType.Int64);

            var factura = await connection.QueryFirstOrDefaultAsync<Factura>(
                "SP_Consultar_Factura",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return factura;
        }

        public async Task<IEnumerable<Factura>> ObtenerFacturasPorPeriodoAsync(string periodo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_Periodo", periodo, DbType.String);

            var facturas = await connection.QueryAsync<Factura>(
                "SP_Consultar_Factura_Periodo",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return facturas;
        }

        public async Task<IEnumerable<Detalle>> ObtenerEncabezadoDetalleAsync(long idFactura)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_idfactura", idFactura, DbType.Int64);

            var detallefacturas = await connection.QueryAsync<Detalle>(
                "SP_Encabezado_Detalle",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return detallefacturas;
        }

        public async Task<IEnumerable<Factura>> ObtenerFacturas()
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var facturas = await connection.QueryAsync<Factura>(
                "SP_Consultar_Factu",
                commandType: CommandType.StoredProcedure
            );

            return facturas;
        }

        public async Task<Factura> ReversarFacturaAsync(long numeroFactura, string motivo)
        {
            using var connection = _dbConnectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_ID_Factura", numeroFactura, DbType.Int64);
            parameters.Add("p_Motivo", motivo);

            await connection.QueryFirstOrDefaultAsync<Factura>(
                "SP_Anular_Factura",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var factura = await connection.QueryFirstOrDefaultAsync<Factura>(
               "SP_Consultar_Factura",
               new { p_ID_Factura = numeroFactura },
               commandType: CommandType.StoredProcedure
           );

            return factura;
        }
    }
}
