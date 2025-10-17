namespace MinimalAPI.Presentation.Routes;

public static class UserRoutes
{
    public static RouteGroupBuilder MapUserRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/users").WithTags("Usuarios");

        return group;
    }
}
