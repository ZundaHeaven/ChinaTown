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
                opt => opt.MapFrom(src => src.Content.ContentType.ToString()))
            .ForMember(dest => dest.AuthorId,
                opt => opt.MapFrom(l => l.Content.UserId))
            .ForMember(dest => dest.Title,
                opt => opt.MapFrom(l => l.Content.Title))
            .ForMember(dest => dest.AuthorName,
                opt => opt.MapFrom(l => l.Content.Author.Username))
            .ForMember(dest => dest.Slug,
                opt => opt.MapFrom(l => l.Content.Slug))
            .ForMember(dest => dest.Excerpt,
                opt => opt.MapFrom(l => l.Content.Excerpt))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(l => l.Content.Status.ToString()));
        
        CreateMap<LikeDto, Like>()
            .ForMember(dest => dest.User,
                opt => opt.Ignore())
            .ForMember(dest => dest.Content,
                opt => opt.Ignore());
    }
}