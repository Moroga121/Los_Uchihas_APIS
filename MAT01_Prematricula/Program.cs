using MAT01_Prematricula;
using MAT01_Prematricula.Services;
using MAT01_Prematricula.Repository;
using System;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();


builder.Services.AddScoped<PrematriculaRepository>();


builder.Services.AddScoped<IPrematriculaService, PrematriculaService>();

// Servicio para llamadas HTTP (LLamar Api de Login)

builder.Services.AddHttpClient();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPrematriculaEndpoints();
app.Run("http://localhost:6001");
