using ActivityRegistrator.Models.MappingProfiles;
using AutoMapper;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Azure;
using Microsoft.Identity.Web;

namespace ActivityRegistrator.API.Core.Extensions;
public static class WebApplicationBuilderExtensions
{
    private const string KeyVaultUri = "https://activityregistratorkv.vault.azure.net/"; // todo. to the conf?
    private const string SecretName = "ActivityRegistratorStorageAccountConnectionString";

    /// <summary>
    /// Injects Azure Client to the <see cref="WebApplicationBuilder.Services"/>
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    public static void AddAzureClients(this WebApplicationBuilder builder)
    {
        bool? useAzurite = builder.Configuration.GetValue<bool>("Environment:useAzurite");

        builder.Services.AddAzureClients(clientBuilder =>
        {
            if (builder.Environment.IsDevelopment() && useAzurite.HasValue && useAzurite.Value)
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

    public static void AddAzureB2CAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options);

                options.TokenValidationParameters.NameClaimType = "name";
            },
            options => { builder.Configuration.Bind("AzureAdB2C", options); });
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
