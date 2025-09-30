namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Class;

public static class ClassRoutes
{
    public static RouteGroupBuilder MapClassRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/class")
          .WithTags("Clases");

        return group;
    }
}
