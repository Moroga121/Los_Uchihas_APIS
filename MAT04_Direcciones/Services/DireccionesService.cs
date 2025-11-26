using MAT04_Direcciones.Entities;
using MAT04_Direcciones.Repository;
using MySqlX.XDevAPI.Common;

namespace MAT04_Direcciones.Services
{
    public class DireccionesService : IDireccionesService
    {
        private readonly DireccionesRepository _direccionesRepository;
        public DireccionesService(DireccionesRepository direccionesRepository)
        {
            _direccionesRepository = direccionesRepository;
        }
        public async Task<IEnumerable<Entities.Provincias>> Obtener_Todos_Provincias()
        {
            return await _direccionesRepository.Obtener_Todos_Provincias();
        }
        public async Task<(IEnumerable<cantones> cantones, string mensaje)> Obtener_Cantones_Por_Provincia(string provincia)
        {
            if (string.IsNullOrWhiteSpace(provincia))
                return (Enumerable.Empty<cantones>(), "El nombre de la provincia es obligatorio.");

            if (provincia.Length > 50)
                return (Enumerable.Empty<cantones>(), "El nombre no puede superar los 50 caracteres.");

            var (cantones, mensajeSP) = await _direccionesRepository.Obtener_Cantones_Por_Provincia(provincia);
            return (cantones, mensajeSP);
        }
        public async Task<(IEnumerable<distritos> distritos, string mensaje)> Obtener_Distritos_Por_Canton_Provincia(string provincia , string canton)
        {
            if (string.IsNullOrWhiteSpace(provincia))
                return (Enumerable.Empty<distritos>(), "El nombre de la provincia es obligatorio.");

            if (provincia.Length > 50)
                return (Enumerable.Empty<distritos>(), "Los datos ingresados no puede superar los 50 caracteres.");

            if(string.IsNullOrWhiteSpace(canton))
                return (Enumerable.Empty<distritos>(), "El nombre del canton es obligatorio.");

            if (canton.Length > 50)
                return (Enumerable.Empty<distritos>(), "Los datos ingresados no puede superar los 50 caracteres.");

            var (distritos, mensajeSP) = await _direccionesRepository.Obtener_Distritos_Por_Canton_Provincia(provincia, canton);

            return (distritos, mensajeSP);
        }

    }
}
