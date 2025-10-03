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
          .AddEndpointFilter<UserIdFilter>()
          .Produces<PublicClassMAPI>(StatusCodes.Status201Created)
          .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest);

        app.MapPost("/classes/assigned", ProfessorClasses)
          .WithName("Obtener clases por profesor")
          .RequireAuthorization("ProfessorOrAdmin")
          .AddEndpointFilter<UserIdFilter>()
          .Produces<PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI>>(StatusCodes.Status200OK)
          .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest);

        app.MapPost("/classes/enrolled", EnrolledClasses)
          .WithName("Obtener clases inscritas")
          .RequireAuthorization("RequireAuthenticated")
          .AddEndpointFilter<UserIdFilter>()
          .Produces<PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI>>(StatusCodes.Status200OK)
          .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest);

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
            if (validation.IsErr) return utils.FieldErrorToBadRequest(validation);

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
            if (validation.IsErr) return utils.FieldErrorToBadRequest(validation);

            var result = await useCase.ExecuteAsync(validation.Unwrap());
            return Results.Ok(result.FromDomain());
        });
    }

    public static Task<IResult> EnrolledClasses(
        ClassCriteriaMAPI criteria,
        HttpContext ctx,
        RoutesUtils utils,
        QueryUseCase<ClassCriteriaDTO, ClassDomain> useCase)
    {
        return utils.HandleResponseAsync(async () =>
        {
            var userId = utils.GetIdFromContext(ctx);
            criteria.WithStudent = userId;

            var validation = criteria.ToDomain();
            if (validation.IsErr) return utils.FieldErrorToBadRequest(validation);

            var result = await useCase.ExecuteAsync(validation.Unwrap());
            return Results.Ok(result.FromDomain());
        });
    }
}
