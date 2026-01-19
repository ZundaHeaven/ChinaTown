using AutoMapper;
using ChinaTown.Application.Dto.Recipe;
using ChinaTown.Domain.Entities;

namespace ChinaTown.Application.Mappers;

public class RecipeMapper : Profile
{
    public RecipeMapper()
    {
        CreateMap<Recipe, RecipeDto>()
            .ForMember(dest => dest.RecipeTypes, 
                opt => opt.MapFrom(src => src.RecipeTypeClaims.Select(rtc => rtc.RecipeType).ToList()))
            .ForMember(dest => dest.Regions, 
                opt => opt.MapFrom(src => src.RecipeRegions.Select(rr => rr.Region).ToList()))
            .ForMember(dest => dest.Username, 
                opt => opt.MapFrom(src => src.Author.Username))
            .ForMember(dest => dest.LikesCount, 
                opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.CommentsCount, 
                opt => opt.MapFrom(src => src.Comments.Count));

        CreateMap<RecipeDto, Recipe>()
            .ForMember(dest => dest.RecipeTypeClaims,
                opt => opt.Ignore())
            .ForMember(dest => dest.RecipeRegions,
                opt => opt.Ignore())
            .ForMember(dest => dest.Author,
                opt => opt.Ignore())
            .ForMember(dest => dest.Likes,
                opt => opt.Ignore())
            .ForMember(dest => dest.Comments,
                opt => opt.Ignore());
    }
}