using Módulo_de_Ofertas_académica_ACD2;
using Módulo_de_Ofertas_académica_ACD2.Repository;
using Módulo_de_Ofertas_académica_ACD2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<ICarreraService, CarreraService>();
builder.Services.AddScoped<CarreraRepository>();

// Servicio para llamadas HTTP (LLamar Api de Login)

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarreraEndpoints();

app.Run("http://localhost:7006");

