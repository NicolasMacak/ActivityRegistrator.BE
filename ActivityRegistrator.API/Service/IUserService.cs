using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Request;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.API.Core.DataProcessing.Model;

namespace ActivityRegistrator.API.Service;
public interface IUserService
{
    public Task<ServiceListResult<UserEntity>> GetListAsync();

    public Task<ServiceResult<UserEntity>> GetAsync(string email);

    public Task<ServiceResult<UserEntity>> CreateAsync(CreateUserRequestDto requestDto);

    public Task<ServiceResult<UserEntity>> UpdateAsync(string email, UpdateUserRequestDto request);

    public Task<ServiceResult<UserEntity>> DeleteAsync(string email);
}
