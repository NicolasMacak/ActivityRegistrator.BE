using System.Net;
using ActivityRegistrator.API.Core.DataProcessing.Enums;
using ActivityRegistrator.API.Core.DataProcessing.Model;
using ActivityRegistrator.API.Core.Extensions;
using Azure;
using Azure.Data.Tables;
using Optional;

namespace ActivityRegistrator.API.Repositories;
public class GenericRepository<Entity> where Entity : class, ITableEntity
{
    private readonly ILogger<GenericRepository<Entity>> _logger;
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
        NullableResponse<Entity> azureResponse = await _tableClient.GetEntityIfExistsAsync<Entity>(partitionKey, rowKey);

        return azureResponse.ToOption();
    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<Entity>> CreateAsync(Entity entity)
    {
        ResultWrapper<Entity> response = new();

        try
        {
            await _tableClient.AddEntityAsync(entity);
            return response.With(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a new user");
            return response.With(OperationStatus.Failure);
        }
    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<Entity>> Update(string partitionKey, ETag tag, Entity entity)
    {
        ResultWrapper<Entity> response = new();

        try
        {
            await _tableClient.UpdateEntityAsync(entity, tag);
            return response.With(entity);
        }
        catch (RequestFailedException requestFailedException)
        {
            if (requestFailedException.Status == (int)HttpStatusCode.PreconditionFailed)
            {
                _logger.LogError(requestFailedException, "Record was already updates. partitionKey: {partitionKey}, rowkey: {rowKey}, requestEtag: {requestEtag}, entityEtag: {entityEtag}", entity.PartitionKey, entity.RowKey, tag, entity.ETag);
                return response.With(entity, OperationStatus.AlreadyUpdated);
            }

            _logger.LogError(requestFailedException, "An error occurred while updating the user");
            return response.With(OperationStatus.Failure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the user");
            return response.With(OperationStatus.Failure);
        }
    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<Entity>> DeleteAsync(Entity entityToDelete)
    {
        ResultWrapper<Entity> response = new();

        try
        {
            await _tableClient.DeleteEntityAsync(entityToDelete);
            return response.With(OperationStatus.Success);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the user");
            return response.With(OperationStatus.Failure);
        }
    }
}
