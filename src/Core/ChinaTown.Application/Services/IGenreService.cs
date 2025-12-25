using ChinaTown.Application.Dto.Common;
using ChinaTown.Application.Dto.Genre;

namespace ChinaTown.Application.Services;

public interface IGenreService
{
    Task<PaginatedResult<GenreDto>> GetGenresAsync(int page, int pageSize);
    Task<GenreDto> GetGenreAsync(Guid id);
    Task<GenreDto> CreateGenreAsync(GenreCreateDto dto);
    Task<GenreDto> UpdateGenreAsync(Guid id, GenreUpdateDto dto);
    Task DeleteGenreAsync(Guid id);
    Task<List<GenreDto>> GetAllGenresAsync();
}