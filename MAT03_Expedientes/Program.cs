using MAT03_Expedientes;
using MAT03_Expedientes.Repository;
using MAT03_Expedientes.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddScoped<Expediente_EstudiantesRepository>();
builder.Services.AddScoped<IExpediente_EstudianteService, Expediente_EstudianteService>();

// Servicio para llamadas HTTP (LLamar Api de Login)

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapExpedientesEndpoints();

app.Run("http://localhost:6003");


