using System.Net;
using ActivityRegistrator.Models.Response;
using Azure;
using Azure.Data.Tables;

namespace ActivityRegistrator.API.Core.Repositories;
public class GenericRepository<Entity> where Entity : class, ITableEntity
{
    private readonly ILogger<GenericRepository<Entity>> _logger;
    private readonly TableClient _tableClient;

    public GenericRepository(ILogger<GenericRepository<Entity>> logger, TableServiceClient tableServiceClient, string tableName)
    {
        _logger = logger;
        _tableClient = tableServiceClient.GetTableClient(tableName);
    }

    public async Task<ResultListWrapper<Entity>> GetListAsync(string tenantCode)
    {
        List<Entity> result = _tableClient //todo. Add logging also here
            .Query<Entity>(x => x.PartitionKey == tenantCode)
            .ToList();

        return new ResultListWrapper<Entity>().With(result);
    }

    public async Task<ResultWrapper<Entity>> GetAsync(string partitionKey, string rowKey)
    {
        ResultWrapper<Entity> response = new();

        try
        {
            Entity? result = _tableClient.GetEntity<Entity>(partitionKey, rowKey);
            return response.With(result);
        }
        catch (RequestFailedException requestFailedException)
        {
            if(requestFailedException.Status == (int)HttpStatusCode.NotFound)
            {
                _logger.LogError("User not found. tenantCode: {TenantCode}, email: {email}", partitionKey, rowKey);
                return response.With(OperationStatus.NotFound);
            }
            _logger.LogError(requestFailedException, "An error occurred while creating a new user");
            throw;
        }
        catch(Exception ex){
            _logger.LogError(ex, "An error occurred while getting the user");
            throw;
        }
    }

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
