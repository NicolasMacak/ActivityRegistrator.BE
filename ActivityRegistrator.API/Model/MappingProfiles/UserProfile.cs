using ActivityRegistrator.Models.Dtoes;
using ActivityRegistrator.Models.Entities;
using AutoMapper;

namespace ActivityRegistrator.Models.MappingProfiles;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserDto>()
            .ForMember(x => x.TenantName, opt => opt.MapFrom(x => x.PartitionKey))
            .ForMember(x => x.Email, opt => opt.MapFrom(x => x.RowKey))
            .ForMember(x => x.FullName, opt => opt.MapFrom(x => x.FullName))
            .ForMember(x => x.ETag, opt => opt.MapFrom(x => x.ETag));
    }
}
