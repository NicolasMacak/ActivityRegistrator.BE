using ActivityRegistrator.API.Core.Extensions;
using Azure;
using Azure.Data.Tables;
using Optional;

namespace ActivityRegistrator.API.Repositories;
public class GenericRepository<Entity> where Entity : class, ITableEntity
{
    private readonly ILogger<GenericRepository<Entity>> _logger; // todo. het
    private readonly TableClient _tableClient;

    public GenericRepository(ILogger<GenericRepository<Entity>> logger, TableServiceClient tableServiceClient, string tableName)
    {
        _logger = logger;
        _tableClient = tableServiceClient.GetTableClient(tableName);
    }

    /// <inheritdoc/>
    public async Task<List<Entity>> GetListAsync(string tenantCode)
    {
        return await _tableClient
            .QueryAsync<Entity>(x => x.PartitionKey == tenantCode)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Option<Entity>> GetAsync(string partitionKey, string rowKey)
    {
        NullableResponse<Entity> tableStorageResponse = await _tableClient.GetEntityIfExistsAsync<Entity>(partitionKey, rowKey);

        return tableStorageResponse.ToOption();
    }

    /// <inheritdoc/>
    public async Task CreateAsync(Entity entity)
    {
        await _tableClient.AddEntityAsync(entity);
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(string partitionKey, ETag tag, Entity entity)
    {
        await _tableClient.UpdateEntityAsync(entity, tag);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Entity entityToDelete)
    {
        await _tableClient.DeleteEntityAsync(entityToDelete);
    }
}
