using EduZasAPI.Application.Classes;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Classes;
using EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;

namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Classes;

public static class ClassRoutes
{
    public static RouteGroupBuilder MapClassRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/class")
          .WithTags("Clases");

        app.MapPost("", AddClass);

        return group;
    }

    public static Task<IResult> AddClass(NewClassMAPI newClass, AddClassUseCase useCase, RoutesUtils utils)
    {
        // return utils.HandleResponseAsync(async () => {});
        return Task.FromResult(Results.Ok());
    }
}
