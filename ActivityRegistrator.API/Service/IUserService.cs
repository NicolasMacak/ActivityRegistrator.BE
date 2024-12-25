using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Request;
using ActivityRegistrator.Models.Response;

namespace ActivityRegistrator.API.Service;
public interface IUserService
{
    public Task<ResultListWrapper<UserEntity>> GetListAsync(string tenantCode);
    public Task<ResultWrapper<UserEntity>> GetAsync(string tenantCode, string email);
    public Task<ResultWrapper<UserEntity>> CreateAsync(string tenantCode, CreateUserRequestDto requestDto);
    public Task<ResultWrapper<UserEntity>> UpdateAsync(string tenantCode, string email, UpdateUserRequestDto request);
    public Task<ResultWrapper<UserEntity>> DeleteAsync(string tenantCode, string email);
}
