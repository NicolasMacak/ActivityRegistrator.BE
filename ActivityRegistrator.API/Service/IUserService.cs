using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Request;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.API.Core.DataProcessing.Model;

namespace ActivityRegistrator.API.Service;
public interface IUserService
{
    public Task<ResultListWrapper<UserEntity>> GetListAsync();

    public Task<ResultWrapper<UserEntity>> GetAsync(string email);

    public Task<ResultWrapper<UserEntity>> CreateAsync(CreateUserRequestDto requestDto);

    public Task<ResultWrapper<UserEntity>> UpdateAsync(string email, UpdateUserRequestDto request);

    public Task<ResultWrapper<UserEntity>> DeleteAsync(string email);
}
