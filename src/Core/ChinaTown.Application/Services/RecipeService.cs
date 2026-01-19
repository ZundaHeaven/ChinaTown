using AutoMapper;
using ChinaTown.Application.Data;
using ChinaTown.Application.Dto.Common;
using ChinaTown.Application.Dto.Recipe;
using ChinaTown.Application.Helpers;
using ChinaTown.Domain.Entities;
using ChinaTown.Domain.Enums;
using ChinaTown.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ChinaTown.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly ApplicationDbContext _context;
    private readonly MongoDbContext _mongoDb;
    private readonly IMapper _mapper;

    public RecipeService(ApplicationDbContext context, MongoDbContext mongoDb, IMapper mapper)
    {
        _context = context;
        _mongoDb = mongoDb;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RecipeDto>> GetRecipesAsync(RecipeFilterDto filter)
    {
        var query = _context.Recipes
            .Include(r => r.RecipeTypeClaims).ThenInclude(rtc => rtc.RecipeType)
            .Include(r => r.RecipeRegions).ThenInclude(rr => rr.Region)
            .Include(r => r.Author)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Title))
            query = query.Where(r => r.Title.Contains(filter.Title));

        if (filter.Difficulty.HasValue)
            query = query.Where(r => r.Difficulty == filter.Difficulty.Value);

        if (filter.RecipeTypeIds != null && filter.RecipeTypeIds.Any())
            query = query.Where(r => r.RecipeTypeClaims.Any(rtc => filter.RecipeTypeIds.Contains(rtc.RecipeTypeId)));

        if (filter.RegionIds != null && filter.RegionIds.Any())
            query = query.Where(r => r.RecipeRegions.Any(rr => filter.RegionIds.Contains(rr.RegionId)));

        if (filter.CookTimeMin.HasValue)
            query = query.Where(r => r.CookTimeMinutes >= filter.CookTimeMin);

        if (filter.CookTimeMax.HasValue)
            query = query.Where(r => r.CookTimeMinutes <= filter.CookTimeMax);
        
        if (filter.Available.HasValue)
            query = query.Where(r => filter.Available.Value ? r.Status == ContentStatus.Published : r.Status != ContentStatus.Published);

        query = filter.Sort switch
        {
            "cooktime_asc" => query.OrderBy(r => r.CookTimeMinutes),
            "cooktime_desc" => query.OrderByDescending(r => r.CookTimeMinutes),
            "difficulty_asc" => query.OrderBy(r => r.Difficulty),
            "difficulty_desc" => query.OrderByDescending(r => r.Difficulty),
            "created_desc" => query.OrderByDescending(r => r.CreatedOn),
            _ => query.OrderByDescending(r => r.CreatedOn)
        };

        var recipes = await query.ToListAsync();
        return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
    }

    public async Task<RecipeDto> GetRecipeAsync(Guid id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.RecipeTypeClaims).ThenInclude(rtc => rtc.RecipeType)
            .Include(r => r.RecipeRegions).ThenInclude(rr => rr.Region)
            .Include(r => r.Author)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null)
            throw new NotFoundException("Recipe not found");

        return _mapper.Map<RecipeDto>(recipe);
    }

    public async Task<RecipeDto> CreateRecipeAsync(RecipeCreateDto dto, Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        var slug = SlugHelper.GenerateSlug("Recipe", dto.Title);
        
        var existedRecipe = _context.Recipes.FirstOrDefault(r => r.Slug == slug);
        if (existedRecipe != null)
            slug = SlugHelper.GenerateSlug("Recipe", "");

        var recipe = new Recipe
        {
            Title = dto.Title,
            Slug = slug,
            Difficulty = dto.Difficulty,
            Ingredients = dto.Ingredients,
            Instructions = dto.Instructions,
            CookTimeMinutes = dto.CookTimeMinutes,
            ImageId = Guid.Empty,
            UserId = userId,
            Status = ContentStatus.Draft
        };

        if (dto.RecipeTypeIds.Any())
        {
            var recipeTypes = await _context.RecipeTypes
                .Where(rt => dto.RecipeTypeIds.Contains(rt.Id))
                .ToListAsync();
            
            foreach (var recipeType in recipeTypes)
            {
                recipe.RecipeTypeClaims.Add(new RecipeTypeClaim { RecipeType = recipeType });
            }
        }

        if (dto.RegionIds.Any())
        {
            var regions = await _context.Regions
                .Where(r => dto.RegionIds.Contains(r.Id))
                .ToListAsync();
            
            foreach (var region in regions)
            {
                recipe.RecipeRegions.Add(new RecipeRegion { Region = region });
            }
        }

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return _mapper.Map<RecipeDto>(recipe);
    }

    public async Task<RecipeDto> UpdateRecipeAsync(Guid id, RecipeUpdateDto dto, Guid userId)
    {
        var recipe = await _context.Recipes
            .Include(r => r.RecipeTypeClaims).ThenInclude(rtc => rtc.RecipeType)
            .Include(r => r.RecipeRegions).ThenInclude(rr => rr.Region)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null)
            throw new NotFoundException("Recipe not found");

        if (recipe.UserId != userId)
            throw new UnauthorizedException("You can only update your own recipes");

        var slug = SlugHelper.GenerateSlug("Recipe", dto.Title);
        var existedRecipe = _context.Recipes.FirstOrDefault(r => r.Slug == slug && r.Id != id);
        if (existedRecipe != null)
            slug = SlugHelper.GenerateSlug("Recipe", "");

        recipe.Title = dto.Title;
        recipe.Slug = slug;
        recipe.Difficulty = dto.Difficulty;
        recipe.Ingredients = dto.Ingredients;
        recipe.Instructions = dto.Instructions;
        recipe.CookTimeMinutes = dto.CookTimeMinutes;
        recipe.ModifiedOn = DateTime.UtcNow;

        _context.RecipeTypeClaims.RemoveRange(recipe.RecipeTypeClaims);
        if (dto.RecipeTypeIds.Any())
        {
            var recipeTypes = await _context.RecipeTypes
                .Where(rt => dto.RecipeTypeIds.Contains(rt.Id))
                .ToListAsync();
            
            foreach (var recipeType in recipeTypes)
            {
                recipe.RecipeTypeClaims.Add(new RecipeTypeClaim { RecipeType = recipeType });
            }
        }

        _context.RecipeRegions.RemoveRange(recipe.RecipeRegions);
        if (dto.RegionIds.Any())
        {
            var regions = await _context.Regions
                .Where(r => dto.RegionIds.Contains(r.Id))
                .ToListAsync();
            
            foreach (var region in regions)
            {
                recipe.RecipeRegions.Add(new RecipeRegion { Region = region });
            }
        }

        await _context.SaveChangesAsync();
        return _mapper.Map<RecipeDto>(recipe);
    }

    public async Task DeleteRecipeAsync(Guid id, Guid userId)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
            throw new NotFoundException("Recipe not found");

        var user = await _context.Users.FindAsync(userId);
        var isAdmin = user?.Role.ToString() == "Admin";

        if (recipe.UserId != userId && !isAdmin)
            throw new UnauthorizedException("You can only delete your own recipes");

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();
    }

    public async Task UploadImageAsync(Guid recipeId, Guid fileId, string fileName, Stream stream)
    {
        var recipe = await _context.Recipes.FindAsync(recipeId);
        if (recipe == null)
            throw new NotFoundException("Recipe not found");

        await _mongoDb.UploadFileAsync(fileId, fileName, stream);
        recipe.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<byte[]> GetImageAsync(Guid recipeId)
    {
        var recipe = await _context.Recipes.FindAsync(recipeId);
        if (recipe == null || recipe.ImageId == Guid.Empty)
            throw new NotFoundException("Recipe or image not found");
        
        return await _mongoDb.DownloadFileAsync(recipe.ImageId);
    }

    public async Task<IEnumerable<RecipeDto>> GetMyRecipesAsync(Guid userId)
    {
        var query = _context.Recipes
            .Include(r => r.RecipeTypeClaims).ThenInclude(rtc => rtc.RecipeType)
            .Include(r => r.RecipeRegions).ThenInclude(rr => rr.Region)
            .Include(r => r.Author)
            .Where(r => r.UserId == userId);

        var recipes = await query
            .OrderByDescending(r => r.CreatedOn)
            .ToListAsync();

        return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
    }

    public async Task<IEnumerable<RecipeDto>> GetArchivedRecipesAsync(Guid userId)
    {
        var query = _context.Recipes
            .Include(r => r.RecipeTypeClaims).ThenInclude(rtc => rtc.RecipeType)
            .Include(r => r.RecipeRegions).ThenInclude(rr => rr.Region)
            .Include(r => r.Author)
            .Where(r => r.UserId == userId && r.Status == ContentStatus.Archived);

        var recipes = await query
            .OrderByDescending(r => r.CreatedOn)
            .ToListAsync();

        return _mapper.Map<IEnumerable<RecipeDto>>(recipes);
    }

    public async Task PublishRecipeAsync(Guid recipeId, Guid userId)
    {
        var recipe = await _context.Recipes.FindAsync(recipeId);
        if (recipe == null)
            throw new NotFoundException("Recipe not found");

        if (recipe.UserId != userId)
            throw new UnauthorizedException("You can only publish your own recipes");

        recipe.Status = ContentStatus.Published;
        recipe.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task UnpublishRecipeAsync(Guid recipeId, Guid userId)
    {
        var recipe = await _context.Recipes.FindAsync(recipeId);
        if (recipe == null)
            throw new NotFoundException("Recipe not found");

        if (recipe.UserId != userId)
            throw new UnauthorizedException("You can only unpublish your own recipes");

        recipe.Status = ContentStatus.Draft;
        recipe.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task ArchiveRecipeAsync(Guid recipeId, Guid userId)
    {
        var recipe = await _context.Recipes.FindAsync(recipeId);
        if (recipe == null)
            throw new NotFoundException("Recipe not found");

        if (recipe.UserId != userId)
            throw new UnauthorizedException("You can only archive your own recipes");

        recipe.Status = ContentStatus.Archived;
        recipe.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task RestoreRecipeAsync(Guid recipeId, Guid userId)
    {
        var recipe = await _context.Recipes.FindAsync(recipeId);
        if (recipe == null)
            throw new NotFoundException("Recipe not found");

        if (recipe.UserId != userId)
            throw new UnauthorizedException("You can only restore your own recipes");

        recipe.Status = ContentStatus.Draft;
        recipe.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CommentDto>> GetCommentsAsync(Guid recipeId)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.ContentId == recipeId)
            .OrderByDescending(c => c.CreatedOn)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task<IEnumerable<LikeDto>> GetLikesAsync(Guid recipeId)
    {
        var likes = await _context.Likes
            .Include(l => l.User)
            .Where(l => l.ContentId == recipeId)
            .OrderByDescending(l => l.CreatedOn)
            .ToListAsync();

        return _mapper.Map<IEnumerable<LikeDto>>(likes);
    }
}