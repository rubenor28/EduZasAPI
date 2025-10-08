using System.Security.Claims;

namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;

public class ExecutorFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
      EndpointFilterInvocationContext ctx,
      EndpointFilterDelegate next)
    {
        var httpCtx = ctx.HttpContext;

        if (httpCtx.User.Identity is null || !httpCtx.User.Identity.IsAuthenticated)
            return Results.Unauthorized();

        var userId = httpCtx.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userId is null || string.IsNullOrEmpty(userId.Value))
            return Results.Problem("El token no contiene el claim 'sub'");

        var userRole = httpCtx.User.FindFirst(ClaimTypes.Role);
        if (userRole is null || string.IsNullOrEmpty(userRole.Value))
            return Results.Problem("El token no contiene el claim 'role'");

        httpCtx.Items["UserId"] = userId.Value;
        httpCtx.Items["UserRole"] = userRole.Value;

        return await next(ctx);
    }
}
