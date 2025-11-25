using USR5_Login;
using USR5_Login.Repository;
using USR5_Login.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inyección de Dependencias

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<LoginRepository>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddSingleton<TokenRepository>();
builder.Services.AddHttpClient<ILoginService, LoginService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapLoginEndpoints();

app.Run();

