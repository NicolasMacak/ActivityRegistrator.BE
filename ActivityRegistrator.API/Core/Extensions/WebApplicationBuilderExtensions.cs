using ActivityRegistrator.Models.MappingProfiles;
using AutoMapper;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Azure;

namespace ActivityRegistrator.API.Core.Extensions;
public static class WebApplicationBuilderExtensions
{
    private const string KeyVaultName = "ActivityRegistratorKeys"; 
    private const string KeyVaultUri = $"https://{KeyVaultName}.vault.azure.net";
    private const string SecretName = "ActivityRegisratorConnectionString";

    /// <summary>
    /// Injects Azure Client to the <see cref="WebApplicationBuilder.Services"/>
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    public static void AddAzureClients(this WebApplicationBuilder builder)
    {
        builder.Services.AddAzureClients(clientBuilder =>
        {
            if (builder.Environment.IsDevelopment())
            {
                string? connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:LocalDevStorage");

                if (connectionString == null)
                {
                    throw new NullReferenceException(nameof(connectionString));
                }

                clientBuilder.AddTableServiceClient(connectionString);
            }
            else
            {
                SecretClient kvClient = new SecretClient(new Uri(KeyVaultUri), new DefaultAzureCredential());
                Response<KeyVaultSecret> keyVaultConnectionStringSecret = kvClient.GetSecret(SecretName);
                string connectionString = keyVaultConnectionStringSecret.Value.Value;

                clientBuilder.AddTableServiceClient(connectionString);
            }
        });
    }

    /// <summary>
    /// Injects AutoMapper to the <see cref="WebApplicationBuilder.Services"/>
    /// </summary>
    public static void AddAutoMapper(this WebApplicationBuilder builder)
    {
        MapperConfiguration configuration = new(cfg => {
            cfg.AddProfile<UserProfile>(); 
            } 
        );

        IMapper autoMapper = configuration.CreateMapper();
        builder.Services.AddSingleton(autoMapper);
    }
}
