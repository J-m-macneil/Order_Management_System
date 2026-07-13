using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
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

            context.Response.StatusCode = GetStatusCode(ex);
            context.Response.ContentType = "application/problem+json";

            var response = new ProblemDetails
            {
                Title = GetTitle(context.Response.StatusCode),
                Status = context.Response.StatusCode,
                Detail = GetMessage(ex)
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

    private static string GetTitle(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status403Forbidden => "Forbidden",
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status409Conflict => "Conflict",
            _ => "Internal Server Error"
        };
    }
}
