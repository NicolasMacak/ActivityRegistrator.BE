using System.Net;
using ActivityRegistrator.API.Core.DataProcessing.Enums;
using ActivityRegistrator.API.Core.DataProcessing.Model;
using ActivityRegistrator.API.Core.Extensions;
using Azure;
using Azure.Data.Tables;

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
    public async Task<ResultListWrapper<Entity>> GetListAsync(string tenantCode)
    {
        ResultListWrapper<Entity> response = new();

        try
        {
            List<Entity> result = await _tableClient
                .QueryAsync<Entity>(x => x.PartitionKey == tenantCode)
                .ToListAsync();

            return response.With(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a new user");
            return response.With(OperationStatus.Failure);
        }
    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<Entity>> GetAsync(string partitionKey, string rowKey)
    {
        ResultWrapper<Entity> response = new();

        try
        {
            NullableResponse<Entity> maybeEntity = await _tableClient.GetEntityIfExistsAsync<Entity>(partitionKey, rowKey);
            if (maybeEntity.HasValue)
            {
                return response.With(maybeEntity.Value!);
            }
            else
            {
                _logger.LogError("User not found. tenantCode: {TenantCode}, email: {email}", partitionKey, rowKey);
                return response.With(OperationStatus.NotFound);
            }
        }
        catch(Exception ex){
            _logger.LogError(ex, "An error occurred while getting the user");
            return response.With(OperationStatus.Failure);
        }
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
