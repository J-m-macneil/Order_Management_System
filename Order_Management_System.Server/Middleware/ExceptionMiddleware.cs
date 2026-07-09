using Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (ex is not BadRequestException
                and not ConflictException
                and not ForbiddenException
                and not NotFoundException
                and not UnauthorizedException)
            {
                _logger.LogError(ex, "Unhandled exception");
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = GetStatusCode(ex);

            var response = new
            {
                message = GetMessage(ex)
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            BadRequestException => (int)HttpStatusCode.BadRequest,
            ConflictException => (int)HttpStatusCode.Conflict,
            ForbiddenException => (int)HttpStatusCode.Forbidden,
            NotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedException => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    private static string GetMessage(Exception exception)
    {
        return exception switch
        {
            BadRequestException
                or ConflictException
                or ForbiddenException
                or NotFoundException
                or UnauthorizedException => exception.Message,
            _ => "An unexpected error occurred."
        };
    }
}
