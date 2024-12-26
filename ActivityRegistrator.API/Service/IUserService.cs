using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Request;
using ActivityRegistrator.Models.Response;
using ActivityRegistrator.API.Core.Repositories;

namespace ActivityRegistrator.API.Service;
public interface IUserService
{
    /// <inheritdoc cref="IGenericRepository{Entity}.GetListAsync(string)" />
    public Task<ResultListWrapper<UserEntity>> GetListAsync(string tenantCode);

    /// <inheritdoc cref="IGenericRepository{Entity}.GetAsync(string, string)" />
    public Task<ResultWrapper<UserEntity>> GetAsync(string tenantCode, string email);

    /// <inheritdoc cref="IGenericRepository{Entity}.CreateAsync(Entity)" />
    public Task<ResultWrapper<UserEntity>> CreateAsync(string tenantCode, CreateUserRequestDto requestDto);

    /// <summary>
    /// Updates existing entity in the database with the data in the request
    /// </summary>
    /// <returns>
    /// 200: Success <br></br>
    /// 412(PreconditionFailed): Entity was updated in the meantime
    /// </returns>
    public Task<ResultWrapper<UserEntity>> UpdateAsync(string tenantCode, string email, UpdateUserRequestDto request);

    /// <inheritdoc cref="IGenericRepository{Entity}.DeleteAsync(Entity)" />
    public Task<ResultWrapper<UserEntity>> DeleteAsync(string tenantCode, string email);
}
