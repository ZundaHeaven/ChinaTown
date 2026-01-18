using AutoMapper;
using ChinaTown.Application.Dto.Common;
using ChinaTown.Application.Dto.User;
using ChinaTown.Domain.Entities;

namespace ChinaTown.Application.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, 
                opt => opt.MapFrom(src => src.Role.ToString()));
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Comments,
                opt => opt.Ignore())
            .ForMember(dest => dest.Likes,
                opt => opt.Ignore())
            .ForMember(dest => dest.Contents,
                opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens,
                opt => opt.Ignore());
    }
}