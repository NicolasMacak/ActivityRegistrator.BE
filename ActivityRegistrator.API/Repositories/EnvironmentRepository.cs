using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using System.Net;

namespace ActivityRegistrator.API.Repositories;
public class EnvironmentRepository : IEnvironmentRepository
{
    ILogger<EnvironmentRepository> _logger;
    TableServiceClient _tableServiceClient;

    public EnvironmentRepository(ILogger<EnvironmentRepository> logger, TableServiceClient tableServiceClient) {
        _logger = logger;
        _tableServiceClient = tableServiceClient;
    }

    /// <inheritdoc/>
    public void CreateInitialTablesIfNotExist(IEnumerable<string> tablesNames)
    {
        foreach (var tableName in tablesNames)
        {
            Response<TableItem> response = _tableServiceClient.CreateTableIfNotExists(tableName);

            if(response.GetRawResponse().Status == (int)HttpStatusCode.Conflict)
            {
                _logger.LogInformation("Table '{tableName}' already existed", tableName);
            }
            else if(response.GetRawResponse().Status == (int)HttpStatusCode.NoContent)
            {
                _logger.LogInformation("Table '{tableName}' was created", tableName);
            }
        }

        _logger.LogInformation("All tables exists");
    }
}
