using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChinaTown.Application.Dto.Region;
using ChinaTown.Application.Services;
using ChinaTown.Domain.Exceptions;

namespace ChinaTown.Web.Controllers;

[ApiController]
[Route("api/regions")]
public class RegionController : ControllerBase
{
    private readonly IRegionService _regionService;

    public RegionController(IRegionService regionService)
    {
        _regionService = regionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _regionService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var region = await _regionService.GetByIdAsync(id);
        return Ok(region);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] RegionCreateDto dto)
    {
        var region = await _regionService.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = region.Id }, region);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RegionUpdateDto dto)
    {
        var region = await _regionService.UpdateAsync(id, dto);
        return Ok(region);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _regionService.DeleteAsync(id);
        return NoContent();
    }
}