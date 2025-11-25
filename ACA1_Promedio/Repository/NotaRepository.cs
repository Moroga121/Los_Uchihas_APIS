using System.Data;
using ACA1_Promedio.Entities;
using Dapper;

namespace ACA1_Promedio.Repository
{
    public class NotaRepository
    {
        private readonly HttpClient _httpClient;


        public NotaRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Nota>> ObtenerNotasPorEstudianteAsync(string tipoIdentificacion, string numeroIdentificacion, int? año, string? periodo)
        {
            // Construir la URL base
            var url = $"http://localhost:5125/notas/{tipoIdentificacion}/{numeroIdentificacion}";

            // Construir query string solo con parámetros que tengan valor
            var queryParams = new List<string>();

            if (año.HasValue)
                queryParams.Add($"año={año.Value}");

            if (!string.IsNullOrEmpty(periodo))
                queryParams.Add($"periodo={periodo}");

            // Agregar query string a la URL si hay parámetros
            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<Nota>();

            var notas = await response.Content.ReadFromJsonAsync<List<Nota>>();
            return notas ?? new List<Nota>();
        }

       


    }
}
