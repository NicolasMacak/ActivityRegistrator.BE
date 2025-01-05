using ActivityRegistrator.API.Core.Repositories;
using ActivityRegistrator.API.Repositories;
using System.Reflection;

namespace ActivityRegistrator.API.Service;
public class EnvironmentService : IEnvironmentService
{
    private readonly ILogger<EnvironmentService> _logger;
    private readonly IEnvironmentRepository _environmentRepository;


    public EnvironmentService(ILogger<EnvironmentService> logger, IEnvironmentRepository environmentRepository) {
        _logger = logger;
        _environmentRepository = environmentRepository;
    }

    /// <inheritdoc/>
    public void SetupEnvironment()
    {
        CreateAzureTableStorageTablesIfNotExists();

        _logger.LogInformation("Environment setup completed");
    }

    private void CreateAzureTableStorageTablesIfNotExists()
    {
        Type tableNamesConstantsType = typeof(TablesNames);

        IEnumerable<string> tablesNames = tableNamesConstantsType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Select(x => x.GetRawConstantValue()!.ToString()!);

        _environmentRepository.CreateInitialTablesIfNotExist(tablesNames);
    }
}
