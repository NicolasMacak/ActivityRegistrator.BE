using Microsoft.Extensions.Azure;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.API.Core.Extensions;
using ActivityRegistrator.API.Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IEnvironmentService, EnvironmentService>();
builder.Services.AddTransient<IEnvironmentRepository, EnvironmentRepository>();

builder.AddAzureClients();
builder.AddAutoMapper();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Services.GetRequiredService<IEnvironmentService>().SetupEnvironment();

app.UseAuthorization();

app.MapControllers();

app.Run();
