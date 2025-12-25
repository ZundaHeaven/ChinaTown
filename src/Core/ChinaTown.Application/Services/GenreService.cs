using ChinaTown.Application.Data;
using Microsoft.EntityFrameworkCore;
using ChinaTown.Application.Dto.Common;
using ChinaTown.Application.Dto.Genre;
using ChinaTown.Domain.Entities;
using ChinaTown.Domain.Exceptions;

namespace ChinaTown.Application.Services;

public class GenreService : IGenreService
{
    private readonly ApplicationDbContext _context;

    public GenreService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<GenreDto>> GetGenresAsync(int page, int pageSize)
    {
        var query = _context.Genres
            .Include(g => g.BookGenres)
            .OrderBy(g => g.Name);

        var totalCount = await query.CountAsync();
        var genres = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name,
                CreatedOn = g.CreatedOn,
                ModifiedOn = g.ModifiedOn,
                BooksCount = g.BookGenres.Count
            })
            .ToListAsync();

        return new PaginatedResult<GenreDto>
        {
            Items = genres,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<GenreDto> GetGenreAsync(Guid id)
    {
        var genre = await _context.Genres
            .Include(g => g.BookGenres)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (genre == null)
            throw new NotFoundException("Genre not found");

        return new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name,
            CreatedOn = genre.CreatedOn,
            ModifiedOn = genre.ModifiedOn,
            BooksCount = genre.BookGenres.Count
        };
    }

    public async Task<GenreDto> CreateGenreAsync(GenreCreateDto dto)
    {
        var existingGenre = await _context.Genres
            .FirstOrDefaultAsync(g => g.Name == dto.Name);

        if (existingGenre != null)
            throw new BadRequestException("Genre with this name already exists");

        var genre = new Genre
        {
            Name = dto.Name
        };

        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        return new GenreDto
        {
            Id = genre.Id,
            Name = genre.Name,
            CreatedOn = genre.CreatedOn,
            ModifiedOn = genre.ModifiedOn,
            BooksCount = 0
        };
    }

    public async Task<GenreDto> UpdateGenreAsync(Guid id, GenreUpdateDto dto)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null)
            throw new NotFoundException("Genre not found");

        var existingGenre = await _context.Genres
            .FirstOrDefaultAsync(g => g.Name == dto.Name && g.Id != id);

        if (existingGenre != null)
            throw new BadRequestException("Genre with this name already exists");

        genre.Name = dto.Name;
        genre.ModifiedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var updatedGenre = await _context.Genres
            .Include(g => g.BookGenres)
            .FirstOrDefaultAsync(g => g.Id == id);

        return new GenreDto
        {
            Id = updatedGenre!.Id,
            Name = updatedGenre.Name,
            CreatedOn = updatedGenre.CreatedOn,
            ModifiedOn = updatedGenre.ModifiedOn,
            BooksCount = updatedGenre.BookGenres.Count
        };
    }

    public async Task DeleteGenreAsync(Guid id)
    {
        var genre = await _context.Genres
            .Include(g => g.BookGenres)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (genre == null)
            throw new NotFoundException("Genre not found");

        if (genre.BookGenres.Any())
            throw new BadRequestException("Cannot delete genre that has books assigned");

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
    }

    public async Task<List<GenreDto>> GetAllGenresAsync()
    {
        return await _context.Genres
            .OrderBy(g => g.Name)
            .Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name,
                CreatedOn = g.CreatedOn,
                ModifiedOn = g.ModifiedOn,
                BooksCount = g.BookGenres.Count
            })
            .ToListAsync();
    }
}