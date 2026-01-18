using ChinaTown.Application.Data;
using ChinaTown.Application.Dto.Article;
using ChinaTown.Domain.Entities;
using ChinaTown.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChinaTown.Application.Services;

public class ArticleTypeService : IArticleTypeService
{
    private readonly ApplicationDbContext _context;

    public ArticleTypeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ArticleTypeDto>> GetAllArticleTypesAsync()
    {
        var types = await _context.ArticleTypes
            .OrderBy(at => at.Name)
            .ToListAsync();

        return types.Select(MapToDto);
    }

    public async Task<ArticleTypeDto?> GetArticleTypeByIdAsync(Guid id)
    {
        var type = await _context.ArticleTypes.FindAsync(id);
        
        if (type == null)
            return null;
        
        return MapToDto(type);
    }

    public async Task<ArticleTypeDto> CreateArticleTypeAsync(ArticleTypeCreateDto dto)
    {
        var existingType = await _context.ArticleTypes
            .FirstOrDefaultAsync(at => at.Name == dto.Name);
        
        if (existingType != null)
            throw new BadRequestException($"Article type with name '{dto.Name}' already exists");

        var type = new ArticleType
        {
            Id = Guid.NewGuid(),
            Name = dto.Name
        };

        _context.ArticleTypes.Add(type);
        await _context.SaveChangesAsync();

        return MapToDto(type);
    }

    public async Task<ArticleTypeDto> UpdateArticleTypeAsync(Guid id, ArticleTypeUpdateDto dto)
    {
        var type = await _context.ArticleTypes.FindAsync(id);
        
        if (type == null)
            throw new NotFoundException($"Article type with id {id} not found");

        if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != type.Name)
        {
            var existingType = await _context.ArticleTypes
                .FirstOrDefaultAsync(at => at.Name == dto.Name);
            
            if (existingType != null)
                throw new BadRequestException($"Article type with name '{dto.Name}' already exists");
            
            type.Name = dto.Name;
        }

        await _context.SaveChangesAsync();

        return MapToDto(type);
    }

    public async Task DeleteArticleTypeAsync(Guid id)
    {
        var type = await _context.ArticleTypes
            .Include(at => at.Articles)
            .FirstOrDefaultAsync(at => at.Id == id);
        
        if (type == null)
            throw new NotFoundException($"Article type with id {id} not found");

        if (type.Articles.Any())
            throw new BadRequestException("Cannot delete article type that has associated articles");

        _context.ArticleTypes.Remove(type);
        await _context.SaveChangesAsync();
    }

    private ArticleTypeDto MapToDto(ArticleType type)
    {
        return new ArticleTypeDto
        {
            Id = type.Id,
            Name = type.Name,
            CreatedOn = type.CreatedOn,
            ModifiedOn = type.ModifiedOn
        };
    }
}