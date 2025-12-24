using System.Text.Json;

namespace ChinaTown.Application.Dto;

public class ErrorResponseDto
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
    
    public override string ToString() => 
        JsonSerializer.Serialize(this);
}