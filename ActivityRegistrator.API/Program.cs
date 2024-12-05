using Microsoft.Extensions.Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAzureClients(async clientBuilder =>
{
    const string KeyVaultName = "ActivityRegistratorKeys";
    const string SecretName = "ActivityRegisratorConnectionString";
    string keyVaultUri = $"https://{KeyVaultName}.vault.azure.net";

    SecretClient kvClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

    Response<KeyVaultSecret> secret = await kvClient.GetSecretAsync(SecretName);

    //tenantId
    //3bb5863c - 846d - 499c - b163 - 89b96e7166c9

    //clientBuilder.AddTableServiceClient();

    // store credentials niekde safe
});

//https://learn.microsoft.com/en-us/dotnet/azure/sdk/dependency-injection?tabs=web-app-builder

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
