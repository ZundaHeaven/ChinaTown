using AutoMapper;
using ChinaTown.Application.Dto.Region;
using ChinaTown.Domain.Entities;

namespace ChinaTown.Application.Mappers;

public class RegionMapper : Profile
{
    public RegionMapper()
    {
        CreateMap<Region, RegionDto>()
            .ForMember(dest => dest.RecipeCount, 
                opt => opt.MapFrom(src => src.RecipeRegions.Count));
        
        CreateMap<RegionCreateDto, Region>();
        CreateMap<RegionUpdateDto, Region>();
    }
}