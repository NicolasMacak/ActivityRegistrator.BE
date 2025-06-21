using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Response;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.Models.Request;
using ActivityRegistrator.API.Core.Enums;

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

    /// <inheritdoc/>
    public async Task<ResultListWrapper<UserEntity>> GetListAsync(string tenantCode)
    {
        return await _userRepository.GetListAsync(tenantCode);
    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<UserEntity>> GetAsync(string tenantCode, string email)
    {
        return await _userRepository.GetAsync(tenantCode, email);
    }

    /// <inheritdoc/>
    public async Task<ResultWrapper<UserEntity>> CreateAsync(string tenantCode, CreateUserRequestDto requestDto)
    {
        ResultWrapper<UserEntity> responseOfEntityToUpdate = await _userRepository.GetAsync(tenantCode, requestDto.Email);

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
            AccessRole = UserRoles.User.ToString()
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

    public async Task<UserRoles> GetUserRole(string tenantCode, string email)
    {
        ResultWrapper<UserEntity> tenantUser = await GetAsync(tenantCode, email);

        if (tenantUser.Status == OperationStatus.Success)
        {
            return Enum.TryParse(tenantUser.Value!.AccessRole, out UserRoles userRole) ?
                 userRole
                 : UserRoles.Guest;
        }

        return UserRoles.Guest;
    }
}
