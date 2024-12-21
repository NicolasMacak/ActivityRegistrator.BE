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

    if (builder.Environment.IsDevelopment())
    {
        string? connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:LocalDevStorage");

        if (connectionString == null)
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        //"AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;"

        clientBuilder.AddTableServiceClient(connectionString);
    }
    else
    {
        throw new NotImplementedException();
        //const string KeyVaultName = "ActivityRegistratorKeys";
        //const string SecretName = "ActivityRegisratorConnectionString";
        //string keyVaultUri = $"https://{KeyVaultName}.vault.azure.net";

        //SecretClient kvClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

        //Response<KeyVaultSecret> secret = await kvClient.GetSecretAsync(SecretName);
    }
    
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
