using MAT05_Notas;
using MAT05_Notas.Repository;
using MAT05_Notas.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();


builder.Services.AddScoped<NotasRepository>();
builder.Services.AddScoped<INotasService,NotasService>();
// Servicio para llamadas HTTP (LLamar Api de Login)

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDesgloseRubroEndpoints();
app.MapRubroEndpoints();
app.MapNotaEndpoints();
app.MapObtenerNotasGroup();

app.Run("http://localhost:6005");
