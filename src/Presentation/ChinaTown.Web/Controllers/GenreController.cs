using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChinaTown.Application.Dto.Genre;
using ChinaTown.Application.Services;
using ChinaTown.Domain.Enums;

namespace ChinaTown.Web.Controllers;

[ApiController]
[Route("api/genres")]
[Authorize(Roles = "Admin")]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenreController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpGet]
    public async Task<IActionResult> GetGenres()
    {
        var result = await _genreService.GetGenresAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGenre(Guid id)
    {
        var genre = await _genreService.GetGenreAsync(id);
        return Ok(genre);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGenre([FromBody] GenreCreateDto dto)
    {
        var genre = await _genreService.CreateGenreAsync(dto);
        return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genre);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGenre(Guid id, [FromBody] GenreUpdateDto dto)
    {
        var genre = await _genreService.UpdateGenreAsync(id, dto);
        return Ok(genre);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(Guid id)
    {
        await _genreService.DeleteGenreAsync(id);
        return NoContent();
    }
}