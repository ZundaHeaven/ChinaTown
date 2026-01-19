using ChinaTown.Application.Dto.Region;

namespace ChinaTown.Application.Services;

public interface IRegionService
{
    Task<IEnumerable<RegionDto>> GetAllAsync();
    Task<RegionDto> GetByIdAsync(Guid id);
    Task<RegionDto> CreateAsync(RegionCreateDto dto);
    Task<RegionDto> UpdateAsync(Guid id, RegionUpdateDto dto);
    Task DeleteAsync(Guid id);
}