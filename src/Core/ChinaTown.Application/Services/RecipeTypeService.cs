using AutoMapper;
using ChinaTown.Application.Data;
using ChinaTown.Application.Dto.RecipeType;
using ChinaTown.Domain.Entities;
using ChinaTown.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChinaTown.Application.Services;

public class RecipeTypeService : IRecipeTypeService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public RecipeTypeService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RecipeTypeDto>> GetAllAsync()
    {
        var query = _context.RecipeTypes
            .Include(rt => rt.RecipeTypeClaims)
            .AsQueryable();

        var recipeTypes = await query.ToListAsync();
        return _mapper.Map<IEnumerable<RecipeTypeDto>>(recipeTypes);
    }

    public async Task<RecipeTypeDto> GetByIdAsync(Guid id)
    {
        var recipeType = await _context.RecipeTypes
            .Include(rt => rt.RecipeTypeClaims)
            .FirstOrDefaultAsync(rt => rt.Id == id);

        if (recipeType == null)
            throw new NotFoundException("Recipe type not found");

        return _mapper.Map<RecipeTypeDto>(recipeType);
    }

    public async Task<RecipeTypeDto> CreateAsync(RecipeTypeCreateDto dto)
    {
        var existingRecipeType = await _context.RecipeTypes
            .FirstOrDefaultAsync(rt => rt.Name.ToLower() == dto.Name.ToLower());

        if (existingRecipeType != null)
            throw new BadRequestException($"Recipe type with name '{dto.Name}' already exists");

        var recipeType = _mapper.Map<RecipeType>(dto);
        
        _context.RecipeTypes.Add(recipeType);
        await _context.SaveChangesAsync();

        return _mapper.Map<RecipeTypeDto>(recipeType);
    }

    public async Task<RecipeTypeDto> UpdateAsync(Guid id, RecipeTypeUpdateDto dto)
    {
        var recipeType = await _context.RecipeTypes.FindAsync(id);
        if (recipeType == null)
            throw new NotFoundException("Recipe type not found");

        var existingRecipeType = await _context.RecipeTypes
            .FirstOrDefaultAsync(rt => rt.Name.ToLower() == dto.Name.ToLower() && rt.Id != id);

        if (existingRecipeType != null)
            throw new BadRequestException($"Recipe type with name '{dto.Name}' already exists");

        _mapper.Map(dto, recipeType);
        recipeType.ModifiedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<RecipeTypeDto>(recipeType);
    }

    public async Task DeleteAsync(Guid id)
    {
        var recipeType = await _context.RecipeTypes
            .Include(rt => rt.RecipeTypeClaims)
            .FirstOrDefaultAsync(rt => rt.Id == id);

        if (recipeType == null)
            throw new NotFoundException("Recipe type not found");

        if (recipeType.RecipeTypeClaims.Any())
            throw new BadRequestException("Cannot delete recipe type that has associated recipes");

        _context.RecipeTypes.Remove(recipeType);
        await _context.SaveChangesAsync();
    }
}