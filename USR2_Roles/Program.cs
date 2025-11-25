using USR2_Roles;
using USR2_Roles.Repository;
using USR2_Roles.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inyecciones de Dependencias

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<RolesRepository>();
builder.Services.AddScoped<IRolService, RolService>();

// Servicio para llamadas HTTP (LLamar Api de Login)

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapRolEndpoints();

app.Run("http://localhost:5002");


