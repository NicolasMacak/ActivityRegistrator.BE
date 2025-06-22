using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.Models.Request;
using ActivityRegistrator.API.Core.Security.Enums;
using ActivityRegistrator.API.Core.DataProcessing.Enums;
using ActivityRegistrator.API.Core.DataProcessing.Model;

namespace ActivityRegistrator.API.Service;
public class UserService : IUserService // TenantAdmin service
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IActiveUserService _activeUserService;

    public UserService(ILogger<UserService> logger, IUserRepository userRepository, IActiveUserService activeUserService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _activeUserService = activeUserService;
    }

    /// <inheritdoc/>
    public async Task<ResultListWrapper<UserEntity>> GetListAsync(string tenantCode)
    {
        return await _userRepository.GetListAsync(_activeUserService.TenantCode);
    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<UserEntity>> GetAsync(string tenantCode, string email)
    {
        return await _userRepository.GetAsync(_activeUserService.TenantCode, email);
    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<UserEntity>> CreateAsync(string tenantCode, CreateUserRequestDto requestDto)
    {
        ResultWrapper<UserEntity> responseOfEntityToUpdate = await _userRepository.GetAsync(_activeUserService.TenantCode, requestDto.Email);

        if (responseOfEntityToUpdate.Status == OperationStatus.Success)
        {
            _logger.LogError("Such resource already exists. request: {requestDto}", requestDto.ToString());
            return new ResultWrapper<UserEntity>().With(OperationStatus.UniqueConstraintViolation);
        }
        else if (responseOfEntityToUpdate.Status == OperationStatus.Failure)
        {
            return responseOfEntityToUpdate;
        }

        UserEntity newUserEntity = new()
        {
            PartitionKey = tenantCode,
            RowKey = requestDto.Email,
            FullName = requestDto.FullName,
            AccessLevel = UserAccessLevel.User.ToString()
        };

        return await _userRepository.CreateAsync(newUserEntity);
    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<UserEntity>> UpdateAsync(string tenantCode, string email, UpdateUserRequestDto request)
    {
        ResultWrapper<UserEntity> responseOfEntityToUpdate = await _userRepository.GetAsync(tenantCode, email);

        if(responseOfEntityToUpdate.Status != OperationStatus.Success)
        {
            return responseOfEntityToUpdate;
        }

        UserEntity entityToUpdate = responseOfEntityToUpdate.Value!;

        entityToUpdate.FullName = request.FullName;

        return await _userRepository.Update(email, request.ETag, entityToUpdate);
    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<UserEntity>> DeleteAsync(string tenantCode, string email)
    {
        ResultWrapper<UserEntity> entityToDeleteResponse = await _userRepository.GetAsync(tenantCode, email);
        if (entityToDeleteResponse.Status != OperationStatus.Success)
        {
            return entityToDeleteResponse;
        }

        return await _userRepository.DeleteAsync(entityToDeleteResponse.Value!);
    }
}
