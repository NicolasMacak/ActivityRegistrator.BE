using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Response;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.Models.Request;

namespace ActivityRegistrator.API.Service;
public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;

    public UserService(ILogger<UserService> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<ResultListWrapper<UserEntity>> GetListAsync(string tenantCode)
    {
        return await _userRepository.GetListAsync(tenantCode);
    }

    public async Task<ResultWrapper<UserEntity>> GetAsync(string tenantCode, string email)
    {
        return await _userRepository.Get(tenantCode, email);
    }

    public async Task<ResultWrapper<UserEntity>> CreateAsync(string tenantCode, CreateUserRequestDto requestDto)
    {
        ResultWrapper<UserEntity> responseOfEntityToUpdate = await _userRepository.Get(tenantCode, requestDto.Email);

        if(responseOfEntityToUpdate.Status == OperationStatus.Success)
        {
            _logger.LogError("Such resource already exists. request: {requestDto}", requestDto.ToString());
            return new ResultWrapper<UserEntity>().With(OperationStatus.UniqueConstraintViolation);
        }

        UserEntity newUserEntity = new()
        {
            PartitionKey = tenantCode,
            RowKey = requestDto.Email,
            FullName = requestDto.FullName
        };

        return await _userRepository.Create(newUserEntity);
    }

    public async Task<ResultWrapper<UserEntity>> UpdateAsync(string tenantCode, string email, UpdateUserRequestDto request)
    {
        ResultWrapper<UserEntity> responseOfEntityToUpdate = await _userRepository.Get(tenantCode, email);

        if(responseOfEntityToUpdate.Status != OperationStatus.Success)
        {
            return responseOfEntityToUpdate;
        }

        UserEntity entityToUpdate = responseOfEntityToUpdate.Value!;

        entityToUpdate.FullName = request.FullName;

        return await _userRepository.Update(email, request.ETag, entityToUpdate);
    }

    public async Task<ResultWrapper<UserEntity>> DeleteAsync(string tenantCode, string email)
    {
        ResultWrapper<UserEntity> entityToDeleteResponse = await _userRepository.Get(tenantCode, email);
        if (entityToDeleteResponse.Status != OperationStatus.Success)
        {
            return new ResultWrapper<UserEntity>().With(OperationStatus.NotFound);
        }

        return await _userRepository.Delete(entityToDeleteResponse.Value!);
    }
}
