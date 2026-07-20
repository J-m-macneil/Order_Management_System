using Microsoft.AspNetCore.Mvc;

namespace Server.Middleware;

public sealed class DemoReadOnlyMiddleware
{
    private const string DemoRole = "Demo";
    private readonly RequestDelegate _next;

    public DemoReadOnlyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.IsInRole(DemoRole) &&
            IsMutationRequest(context.Request.Method) &&
            !IsSessionEndpoint(context.Request.Path))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Demo access is read-only.",
                Status = StatusCodes.Status403Forbidden,
                Detail = "Demo users cannot change application data."
            });

            return;
        }

        await _next(context);
    }

    private static bool IsMutationRequest(string method) =>
        HttpMethods.IsPost(method) ||
        HttpMethods.IsPut(method) ||
        HttpMethods.IsPatch(method) ||
        HttpMethods.IsDelete(method);

    private static bool IsSessionEndpoint(PathString path) =>
        path.Equals("/api/auth/refresh", StringComparison.OrdinalIgnoreCase) ||
        path.Equals("/api/auth/logout", StringComparison.OrdinalIgnoreCase);
}
