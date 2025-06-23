using ActivityRegistrator.Models.Dtoes;
using ActivityRegistrator.Models.Entities;

public static class UserMappings
{
    public static UserDto ToDto(this UserEntity userEntity)
    {
        return new UserDto
        {
            TenantName = userEntity.TenantCode,
            Email = userEntity.Email,
            FullName = userEntity.FullName,
            ETag = userEntity.ETag
        };
    }
}
