using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Common;
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

        app.MapPost("/classes/me", ProfessorClasses)
          .WithName("Obtener clases por profesor")
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
            var userId = utils.GetIdFromContext(ctx);

            var newC = ClassMAPIMapper.ToDomain(newClass, userId);
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

    public static Task<IResult> ProfessorClasses(
        ClassCriteriaMAPI criteria,
        HttpContext ctx,
        RoutesUtils utils,
        QueryUseCase<ClassCriteriaDTO, ClassDomain> useCase)
    {
        return utils.HandleResponseAsync(async () =>
        {
            var userId = utils.GetIdFromContext(ctx);
            criteria.WithProfessor = userId;

            var validation = criteria.ToDomain();
            if (validation.IsErr)
            {
                var errs = validation.UnwrapErr();
                var response = new FieldErrorResponse { Message = "Formato inválido", Errors = errs };
                return Results.BadRequest(response);
            }

            var result = await useCase.ExecuteAsync(validation.Unwrap());
            return Results.Ok(result.FromDomain());
        });
    }
}
