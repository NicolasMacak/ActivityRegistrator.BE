using ActivityRegistrator.Models.Response;
using Azure;

namespace ActivityRegistrator.API.Core.Repositories;
public interface IGenericRepository<Entity>
{
    public Task<ResultListWrapper<Entity>> GetListAsync(string tenantCode);

    public Task<ResultWrapper<Entity>> Get(string partitionKey, string rowKey);

    public Task<ResultWrapper<Entity>> Create(Entity entity);

    public Task<ResultWrapper<Entity>> Update(string partitionKey, ETag tag, Entity entity);

    public Task<ResultWrapper<Entity>> Delete(Entity entityToDelete);
}
