using Microsoft.Extensions.Azure;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.API.Core.Extensions;
using ActivityRegistrator.API.Service;
using Microsoft.AspNetCore.Authorization;
using ActivityRegistrator.API.Core.UserAccess.Handlers;
using ActivityRegistrator.API.Core.UserAccess.Middlewares;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IEnvironmentService, EnvironmentService>();
builder.Services.AddTransient<IEnvironmentRepository, EnvironmentRepository>();
builder.Services.AddScoped<IActiveUserService, ActiveUserService>();
builder.Services.AddScoped<IAuthorizationHandler, EndpointAccessHandler>();

builder.AddAzureClients();
builder.AddAzureB2CAuthentification();//todo rename to something better
builder.AddLevelAccessAuthorizationPolicies();

builder.AddAutoMapper();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<SetUserRole>();
app.MapControllers();

app.Services.GetRequiredService<IEnvironmentService>().SetupEnvironment();

app.Run();
