using System.Security.Claims;

namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;

/// <summary>
/// Filtro de endpoint que valida la autenticación de un usuario
/// y extrae el claim <c>sub</c> (ID de usuario) del JWT.
/// 
/// Si el usuario no está autenticado devuelve <c>401 Unauthorized</c>.
/// Si el token no contiene el claim requerido devuelve un <c>Problem</c> result.
/// 
/// En caso exitoso, el ID del usuario se almacena en <see cref="HttpContext.Items"/>
/// bajo la clave <c>"UserId"</c>, para ser usado posteriormente por el endpoint.
/// </summary>
public class UserIdFilter : IEndpointFilter
{
    /// <summary>
    /// Ejecuta la lógica del filtro.
    /// </summary>
    /// <param name="ctx">Contexto de invocación del endpoint.</param>
    /// <param name="next">Delegado al siguiente filtro o manejador.</param>
    /// <returns>
    /// <list type="bullet">
    /// <item><description><see cref="Results.Unauthorized"/> si el usuario no está autenticado.</description></item>
    /// <item><description><see cref="Results.Problem"/> si el token no incluye el claim <c>sub</c>.</description></item>
    /// <item><description>El resultado del endpoint si el filtro se completa correctamente.</description></item>
    /// </list>
    /// </returns>
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext ctx,
        EndpointFilterDelegate next)
    {
        var httpCtx = ctx.HttpContext;

        if (httpCtx.User.Identity is null || httpCtx.User.Identity.IsAuthenticated == false)
            return Results.Unauthorized();

        var userId = httpCtx.User.FindFirst(ClaimTypes.NameIdentifier);

        if (userId is null || string.IsNullOrEmpty(userId.Value))
            return Results.Problem("El token no contiene el claim 'sub'",
                statusCode: StatusCodes.Status400BadRequest);

        httpCtx.Items["UserId"] = userId.Value;

        return await next(ctx);
    }
}
