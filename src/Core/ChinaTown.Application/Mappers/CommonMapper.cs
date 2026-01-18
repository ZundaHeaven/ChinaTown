using AutoMapper;
using ChinaTown.Application.Dto.Common;
using ChinaTown.Domain.Entities;

namespace ChinaTown.Application.Mappers;

public class CommonMapper : Profile
{
    public CommonMapper()
    {
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.Username,
                opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.AvatarId,
                opt => opt.MapFrom(src => src.User.AvatarId))
            .ForMember(dest => dest.ContentType,
                opt => opt.MapFrom(src => src.Content.ContentType));
        
        CreateMap<CommentDto, Comment>()
            .ForMember(dest => dest.User,
                opt => opt.Ignore())
            .ForMember(dest => dest.Content,
                opt => opt.Ignore());

        CreateMap<Like, LikeDto>()
            .ForMember(dest => dest.Username,
                opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.ContentType,
                opt => opt.MapFrom(src => src.Content.ContentType));
        
        CreateMap<LikeDto, Like>()
            .ForMember(dest => dest.User,
                opt => opt.Ignore())
            .ForMember(dest => dest.Content,
                opt => opt.Ignore());
    }
}