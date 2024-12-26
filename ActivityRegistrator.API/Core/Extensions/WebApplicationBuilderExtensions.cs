using ActivityRegistrator.Models.Dtoes;
using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.MappingProfiles;
using AutoMapper;
using Azure;
using Microsoft.Extensions.Azure;

namespace ActivityRegistrator.API.Core.Extensions;
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Injects Azure Client to the <see cref="WebApplicationBuilder.Services"/>
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    public static void AddAzureClients(this WebApplicationBuilder builder)
    {
        builder.Services.AddAzureClients(async clientBuilder =>
        {
            if (builder.Environment.IsDevelopment())
            {
                string? connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:LocalDevStorage");

                if (connectionString == null)
                {
                    throw new ArgumentNullException(nameof(connectionString));
                }

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
    }

    /// <summary>
    /// Injects AutoMapper to the <see cref="WebApplicationBuilder.Services"/>
    /// </summary>
    public static void AddAutoMapper(this WebApplicationBuilder builder)
    {
        MapperConfiguration configuration = new MapperConfiguration(cfg => {
            cfg.AddProfile<UserProfile>(); 
            } 
        );

        IMapper autoMapper = configuration.CreateMapper();
        builder.Services.AddSingleton(autoMapper);
    }
}
