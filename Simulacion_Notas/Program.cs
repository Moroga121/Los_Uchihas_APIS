using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simulacion_Notas.Repository;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Habilitar Swagger en modo desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoints
app.MapGet("/notas/{tipoIdentificacion}/{identificacion}", (string tipoIdentificacion, string identificacion, int? año, string? periodo) =>
{
    var notas = NotaRepository.Notas
        .Where(n => n.NumeroIdentificacion == identificacion &&
                    n.TipoIdentificacion == tipoIdentificacion)
        .ToList();

    // Aplicar filtro de año si está presente
    if (año.HasValue)
    {
        notas = notas.Where(n => n.Año == año.Value).ToList();
    }

    if (!string.IsNullOrEmpty(periodo))
    {
        notas = notas.Where(n => n.Periodo == periodo).ToList();
    }

    if (!notas.Any())
        return Results.NotFound(new { Mensaje = "No se encontraron notas para este estudiante." });

    return Results.Ok(notas);
});


app.Run("http://localhost:5125");
