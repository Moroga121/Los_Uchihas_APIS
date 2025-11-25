using ACA1_Promedio;
using ACA1_Promedio.Repository;
using ACA1_Promedio.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<NotaRepository>();
builder.Services.AddScoped<HistorialService>();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Registrar endpoints
app.MapHistorialAcademicoEndpoints();

app.Run("http://localhost:8000");

