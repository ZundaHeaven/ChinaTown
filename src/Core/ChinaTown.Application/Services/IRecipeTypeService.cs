using ChinaTown.Application.Dto.RecipeType;

namespace ChinaTown.Application.Services;

public interface IRecipeTypeService
{
    Task<IEnumerable<RecipeTypeDto>> GetAllAsync();
    Task<RecipeTypeDto> GetByIdAsync(Guid id);
    Task<RecipeTypeDto> CreateAsync(RecipeTypeCreateDto dto);
    Task<RecipeTypeDto> UpdateAsync(Guid id, RecipeTypeUpdateDto dto);
    Task DeleteAsync(Guid id);
}