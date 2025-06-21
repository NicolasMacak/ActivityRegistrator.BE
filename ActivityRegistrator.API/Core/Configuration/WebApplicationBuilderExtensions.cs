using ActivityRegistrator.Models.MappingProfiles;
using AutoMapper;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Azure;
using Microsoft.Identity.Web;
using ActivityRegistrator.API.Core.UserAccess.Enums;
using ActivityRegistrator.API.Core.UserAccess.Constants;
using ActivityRegistrator.API.Core.Configuration;

namespace ActivityRegistrator.API.Core.Configuration;
public static class WebApplicationBuilderExtensions
{
    private const string KeyVaultUri = "https://activityregistratorkv.vault.azure.net/";
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

    public static void AddAzureB2CAuthentification(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                options.TokenValidationParameters.NameClaimType = "name"; // todo.can be access in User.Identity.Name. Needed?
            },
            options => { builder.Configuration.Bind("AzureAdB2C", options); });
    }

    public static void AddLevelAccessAuthorizationPolicies(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(UserAccessLevelPolicy.RootAccess, policy =>
                policy.AddRequirements(new EndpointRequirement(UserAccessLevel.Root)));

            options.AddPolicy(UserAccessLevelPolicy.TenantAdminAccess, policy =>
                policy.AddRequirements(new EndpointRequirement(UserAccessLevel.TenantAdmin)));

            options.AddPolicy(UserAccessLevelPolicy.DelegatedTenantAdminAccess, policy =>
                policy.AddRequirements(new EndpointRequirement(UserAccessLevel.DelegatedAdmin)));

            options.AddPolicy(UserAccessLevelPolicy.UserAccess, policy =>
                policy.AddRequirements(new EndpointRequirement(UserAccessLevel.User)));

            options.AddPolicy(UserAccessLevelPolicy.GuestAccess, policy =>
                policy.AddRequirements(new EndpointRequirement(UserAccessLevel.Guest)));
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
