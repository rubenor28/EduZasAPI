using System.Security.Claims;

namespace MinimalAPI.Presentation.Filters;

/// <summary>
/// Filtro de endpoint que extrae y valida la información del ejecutor (usuario autenticado) del contexto HTTP.
/// </summary>
public class ExecutorFilter : IEndpointFilter
{
    /// <summary>
    /// Invoca el filtro para procesar la solicitud.
    /// </summary>
    /// <param name="ctx">Contexto de invocación del filtro.</param>
    /// <param name="next">Delegado para invocar el siguiente filtro o el endpoint.</param>
    /// <returns>El resultado de la ejecución del endpoint o una respuesta de error.</returns>
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext ctx,
        EndpointFilterDelegate next
    )
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
