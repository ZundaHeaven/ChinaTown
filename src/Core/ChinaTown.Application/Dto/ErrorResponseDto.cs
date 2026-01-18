using System.Text.Json;

namespace ChinaTown.Application.Dto;

public class ErrorResponseDto
{
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}