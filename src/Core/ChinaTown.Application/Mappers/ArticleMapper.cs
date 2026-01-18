using AutoMapper;
using ChinaTown.Application.Dto.Article;
using ChinaTown.Domain.Entities;

namespace ChinaTown.Application.Mappers;

public class ArticleMapper : Profile
{
    public ArticleMapper()
    {
        CreateMap<Article, ArticleDto>()
            .ForMember(dest => dest.ArticleType, 
                opt => opt.MapFrom(src => src.ArticleType.Name))
            .ForMember(dest => dest.AuthorName, 
                opt => opt.MapFrom(src => src.Author.Username))
            .ForMember(dest => dest.LikesCount, 
                opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.CommentsCount, 
                opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.CreatedOn, 
                opt => opt.MapFrom(src => src.CreatedOn))
            .ForMember(dest => dest.ModifiedOn, 
                opt => opt.MapFrom(src => src.ModifiedOn)); 

        CreateMap<ArticleDto, Article>()
            .ForMember(dest => dest.ArticleType, 
                opt => opt.Ignore())
            .ForMember(dest => dest.Author, 
                opt => opt.Ignore())
            .ForMember(dest => dest.Likes, 
                opt => opt.Ignore())
            .ForMember(dest => dest.Comments, 
                opt => opt.Ignore())
            .ForMember(dest => dest.Body, 
                opt => opt.MapFrom(src => src.Body))
            .ForMember(dest => dest.ReadingTimeMinutes, 
                opt => opt.MapFrom(src => src.ReadingTimeMinutes))
            .ForMember(dest => dest.ArticleTypeId, 
                opt => opt.MapFrom(src => src.ArticleTypeId));

        CreateMap<ArticleType, ArticleTypeDto>();
        CreateMap<ArticleTypeDto, ArticleType>();
    }
}