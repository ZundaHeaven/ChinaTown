using ChinaTown.Application.Dto;
using ChinaTown.Domain.Exceptions;

namespace ChinaTown.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var statusCode = GetStatusCode(exception);
        var response = new ErrorResponseDto
        {
            StatusCode = statusCode,
            Message = exception.Message,
            Details = _env.IsDevelopment() ? exception.ToString() : null,
            Errors = GetErrors(exception)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(response.ToString());
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            BadRequestException => StatusCodes.Status400BadRequest,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

    private static Dictionary<string, string[]>? GetErrors(Exception exception)
    {
        if (exception is ValidationException validationException)
        {
            return validationException.Errors;
        }
        return null;
    }
}