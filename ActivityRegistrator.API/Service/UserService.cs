using ActivityRegistrator.Models.Entities;
using ActivityRegistrator.Models.Response;
using ActivityRegistrator.API.Repositories;
using ActivityRegistrator.Models.Request;

namespace ActivityRegistrator.API.Service;
public class UserService
{
    private readonly ILogger<UserService> _logger;
    private readonly UserRepository _userRepository;

    public UserService(ILogger<UserService> logger, UserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<ResponseDtoList<UserEntity>> GetList(string tenantCode)
    {
        return await _userRepository.GetList(tenantCode);
    }

    public async Task<ResponseDto<UserEntity>> Get(string tenantCode, string email)
    {
        return await _userRepository.Get(tenantCode, email);
    }

    public async Task<ResponseDto<UserEntity>> Create(string tenantCode, CreateUserRequestDto requestDto)
    {
        ResponseDto<UserEntity> responseOfEntityToUpdate = await _userRepository.Get(tenantCode, requestDto.Email);

        if(responseOfEntityToUpdate.Status == OperationStatus.Success)
        {
            _logger.LogError("Such resource already exists. request: {requestDto}", requestDto.ToString());
            return new ResponseDto<UserEntity>().With(OperationStatus.AlreadyExists);
        }

        UserEntity newUserEntity = new()
        {
            PartitionKey = tenantCode,
            RowKey = requestDto.Email,
            FullName = requestDto.FullName
        };

        return await _userRepository.Create(newUserEntity);
    }

    public async Task<ResponseDto<UserEntity>> Update(string tenantCode, string email, UpdateUserRequestDto request)
    {
        ResponseDto<UserEntity> responseOfEntityToUpdate = await _userRepository.Get(tenantCode, email);

        if(responseOfEntityToUpdate.Status != OperationStatus.Success)
        {
            return responseOfEntityToUpdate;
        }

        UserEntity entityToUpdate = responseOfEntityToUpdate.Value!;

        entityToUpdate.FullName = request.FullName;

        return await _userRepository.Update(email, request.ETag, entityToUpdate);
    }

    public async Task<ResponseDto<UserEntity>> Delete(string tenantCode, string email)
    {
        ResponseDto<UserEntity> entityToDeleteResponse = await _userRepository.Get(tenantCode, email);
        if (entityToDeleteResponse.Status != OperationStatus.Success)
        {
            return new ResponseDto<UserEntity>().With(OperationStatus.NotFound);
        }

        return await _userRepository.Delete(entityToDeleteResponse.Value!);
    }
}
