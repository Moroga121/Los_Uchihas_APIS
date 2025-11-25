using GEN01_Bitacora.Entities;
using System;
using System.Text.Json;

namespace GEN01_Bitacora.Services
{
    public class BitacoraService : IBitacoraService
    {
        public readonly Repository.BitacoraRepository _bitacoraRepository;

        public BitacoraService(Repository.BitacoraRepository bitacoraRepository)
        {
            _bitacoraRepository = bitacoraRepository;
        }

        public async Task Registrar_Bitacora(string usuario, string accion, object descripcion)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false // opcional: JSON en una sola línea
                };

                string descripcionJson = descripcion is string str ? str : JsonSerializer.Serialize(descripcion, options);

                var bitacora = new Bitacora
                {
                    Usuario = usuario,
                    Accion = accion,
                    Fecha = DateTime.Now,
                    Descripcion = descripcionJson
                };

                await _bitacoraRepository.Registrar_Bitacora(bitacora);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registrando bitácora: {ex.Message}");
            }
        }


        public async Task<IEnumerable<Bitacora>> Obtener_Todas_Las_Bitacoras()
        {
            return await _bitacoraRepository.Obtener_Todas_Las_Bitacoras();
        }
        public async Task<IEnumerable<Bitacora>> Obtener_Todas_Las_BitacorasFiltradas(DateOnly? fechaInicio, DateOnly? fechaFin,string? accion,string? usuario)
        {
            return await _bitacoraRepository.Obtener_Todas_Las_BitacorasFiltradas(fechaInicio, fechaFin, accion, usuario);
        }

    }
}