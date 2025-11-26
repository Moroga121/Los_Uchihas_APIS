using MAT04_Direcciones;
using MAT04_Direcciones.Repository;
using MAT04_Direcciones.Services;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory,DbConnectionFactory >();

builder.Services.AddScoped<DireccionesRepository>();
builder.Services.AddScoped<IDireccionesService, DireccionesService>();

// Servicio para llamadas HTTP (LLamar Api de Login)

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapProvinciasEndPoints();
app.MapCantonesEndPoints();
app.MapDistritosEndPoints();
app.Run("http://localhost:6004");
