using Módulo_de_Oferta_académica_ACD1;
using Módulo_de_Oferta_académica_ACD1.Repository;
using Módulo_de_Oferta_académica_ACD1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IInstitucionService, InstitucionService>();
builder.Services.AddScoped<InstitucionRepository>();

// Servicio para llamadas HTTP (LLamar Api de Login)

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapInstitucionEndpoints();

app.Run("http://localhost:7002");
