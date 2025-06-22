using ActivityRegistrator.API.Core.DataProcessing.Model;
using Azure;
using Optional;

namespace ActivityRegistrator.API.Repositories;
/// <summary>
/// Adasdasdad
/// </summary>
/// <typeparam name="Entity"></typeparam>
public interface IGenericRepository<Entity>
{
    /// <summary>
    /// Returns list of records
    /// </summary>
    /// <returns>
    /// 200: Success <br></br>
    /// </returns>
    public Task<List<Entity>> GetListAsync(string tenantCode);
    /// <summary>
    /// Returns record
    /// </summary>
    /// <returns>
    /// 200: Success <br></br>
    /// 404: Entity not present in the database
    /// </returns>
    public Task<Option<Entity>> GetAsync(string partitionKey, string rowKey);
    /// <summary>
    /// Creates new entity and adds it to the database
    /// </summary>
    /// <returns>
    /// 200: Success <br></br>
    /// 409(Conflict): Entity with such PartitionKey and RowKey already exists
    /// </returns>
    public Task CreateAsync(Entity entity);
    /// <summary>
    /// Updates existing entity in the database
    /// </summary>
    /// <returns>
    /// 200: Success <br></br>
    /// 412(PreconditionFailed): Entity was updated in the meantime
    /// </returns>
    public Task UpdateAsync(string partitionKey, ETag requestEtag, Entity entity);
    /// <summary>
    /// Deletes entity from the database
    /// </summary>
    /// <returns>
    /// 203: Sucess <br></br>
    /// 404: Entity to delete not present in the database
    /// </returns>
    public Task DeleteAsync(Entity entityToDelete);
}
