using EduZasAPI.Application.Classes;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Common;
using EduZasAPI.Infraestructure.MinimalAPI.Application.Classes;
using EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;

namespace EduZasAPI.Infraestructure.MinimalAPI.Presentation.Classes;

public static class ClassRoutes
{
    public static RouteGroupBuilder MapClassRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/")
          .WithTags("Clases");

        app.MapPost("/classes", AddClass)
          .WithName("Crear clases")
          .RequireAuthorization("ProfessorOrAdmin")
          .AddEndpointFilter<UserIdFilter>();

        return group;
    }

    public static Task<IResult> AddClass(
        NewClassMAPI newClass,
        AddClassUseCase useCase,
        RoutesUtils utils,
        HttpContext ctx)
    {
        return utils.HandleResponseAsync(async () =>
        {
            // Obtener ID usuario de token
            var userId = (string?)ctx.Items["UserId"];
            if (userId is null) return Results.Problem("Error al procesar el usuario");

            var newC = ClassMAPIMapper.ToDomain(newClass, ulong.Parse(userId));
            var validation = await useCase.ExecuteAsync(newC);

            if (validation.IsErr)
            {
                var errs = validation.UnwrapErr();
                var response = new FieldErrorResponse { Message = "Formato inválido", Errors = errs };
                return Results.BadRequest(response);
            }

            var newRecord = validation.Unwrap();
            var publicRecord = ClassMAPIMapper.FromDomain(newRecord);

            return Results.Created($"/users/{publicRecord.Id}", publicRecord);
        });
    }
}
