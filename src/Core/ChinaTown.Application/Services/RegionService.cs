using AutoMapper;
using ChinaTown.Application.Data;
using ChinaTown.Application.Dto.Region;
using ChinaTown.Domain.Entities;
using ChinaTown.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChinaTown.Application.Services;

public class RegionService : IRegionService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public RegionService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RegionDto>> GetAllAsync()
    {
        var query = _context.Regions
            .Include(r => r.RecipeRegions)
            .AsQueryable();
        
        var regions = await query.ToListAsync();
        return _mapper.Map<IEnumerable<RegionDto>>(regions);
    }

    public async Task<RegionDto> GetByIdAsync(Guid id)
    {
        var region = await _context.Regions
            .Include(r => r.RecipeRegions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (region == null)
            throw new NotFoundException("Region not found");

        return _mapper.Map<RegionDto>(region);
    }

    public async Task<RegionDto> CreateAsync(RegionCreateDto dto)
    {
        var existingRegion = await _context.Regions
            .FirstOrDefaultAsync(r => r.Name.ToLower() == dto.Name.ToLower());

        if (existingRegion != null)
            throw new BadRequestException($"Region with name '{dto.Name}' already exists");

        var region = _mapper.Map<Region>(dto);
        
        _context.Regions.Add(region);
        await _context.SaveChangesAsync();

        return _mapper.Map<RegionDto>(region);
    }

    public async Task<RegionDto> UpdateAsync(Guid id, RegionUpdateDto dto)
    {
        var region = await _context.Regions.FindAsync(id);
        if (region == null)
            throw new NotFoundException("Region not found");

        var existingRegion = await _context.Regions
            .FirstOrDefaultAsync(r => r.Name.ToLower() == dto.Name.ToLower() && r.Id != id);

        if (existingRegion != null)
            throw new BadRequestException($"Region with name '{dto.Name}' already exists");

        _mapper.Map(dto, region);
        region.ModifiedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<RegionDto>(region);
    }

    public async Task DeleteAsync(Guid id)
    {
        var region = await _context.Regions
            .Include(r => r.RecipeRegions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (region == null)
            throw new NotFoundException("Region not found");

        if (region.RecipeRegions.Any())
            throw new BadRequestException("Cannot delete region that has associated recipes");

        _context.Regions.Remove(region);
        await _context.SaveChangesAsync();
    }
}