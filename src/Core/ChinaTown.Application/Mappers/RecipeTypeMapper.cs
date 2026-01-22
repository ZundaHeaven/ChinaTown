using AutoMapper;
using ChinaTown.Application.Dto.RecipeType;
using ChinaTown.Domain.Entities;

namespace ChinaTown.Application.Mappers;

public class RecipeTypeMapper : Profile
{
    public RecipeTypeMapper()
    {
        CreateMap<RecipeType, RecipeTypeDto>();
        
        CreateMap<RecipeTypeCreateDto, RecipeType>();
        CreateMap<RecipeTypeUpdateDto, RecipeType>();
    }
}