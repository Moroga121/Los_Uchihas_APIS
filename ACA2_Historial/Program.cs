using ACA2_Historial;
using ACA2_Historial.Repository;
using ACA2_Historial.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Se agregaron los servicios que se crearon
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<HistorialRepository>();
builder.Services.AddScoped<IHistorialService, HistorialService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapEstudiantesMatriculaPeriodoEndpoints();

app.Run("http://localhost:8001");
