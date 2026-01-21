using ChinaTown.Application.Data;
using Microsoft.AspNetCore.Mvc;

namespace ChinaTown.Web.Controllers;

[ApiController]
[Route("/api/files")]
public class FileController : ControllerBase
{
    private MongoDbContext _dbContext;

    public FileController(MongoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet("/images/{id}")]
    public async Task<IActionResult> GetImageById(Guid id)
    {
        var bytes = await _dbContext.DownloadFileAsync(id);
        return File(bytes, "image/jpeg");
    }
    
    [HttpGet("/documents/{id}")]
    public async Task<IActionResult> GetDocumentById(Guid id)
    {
        var bytes = await _dbContext.DownloadFileAsync(id);
        return File(bytes, "application/pdf");
    }
}