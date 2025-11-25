using Módulo_de_Oferta_académica_ACD4;
using Módulo_de_Oferta_académica_ACD4.Repository;
using Módulo_de_Oferta_académica_ACD4.Sercives;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<GrupoRepository>();
builder.Services.AddScoped<IGrupoService, GrupoService>();

// Servicio para llamadas HTTP (LLamar Api de Login)

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrupoEndpoints();

app.Run("http://localhost:7003");
