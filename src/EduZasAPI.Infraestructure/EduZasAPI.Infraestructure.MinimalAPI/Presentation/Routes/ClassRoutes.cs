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
          .Produces(StatusCodes.Status401Unauthorized)
          .Produces(StatusCodes.Status403Forbidden)
          .Produces<PublicClassMAPI>(StatusCodes.Status201Created)
          .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
          .WithOpenApi(op =>
          {
              op.Summary = "Creación de clases";
              op.Responses["201"].Description = "Si el recurso se creó correctamente";
              op.Responses["400"].Description = "Si el formato de entrada no es adecuado";
              op.Responses["401"].Description = "Si el usuario no está autenticado";
              op.Responses["403"].Description = "Si el usaurio no cumple con la política de acceso";
              return op;
          });

        app.MapPut("/classes", UpdateClass)
          .RequireAuthorization("ProfessorOrAdmin")
          .AddEndpointFilter<UserIdFilter>()
          .Produces(StatusCodes.Status401Unauthorized)
          .Produces(StatusCodes.Status403Forbidden)
          .Produces<PublicClassMAPI>(StatusCodes.Status200OK)
          .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
          .Produces(StatusCodes.Status404NotFound).WithOpenApi(op =>
          {
              op.Summary = "Actualización de clases";
              op.Responses["200"].Description = "Si el recurso se actualizó correctamente";
              op.Responses["400"].Description = "Si el formato de entrada no es adecuado";
              op.Responses["401"].Description = "Si el usuario no está autenticado";
              op.Responses["403"].Description = "Si el usaurio no cumple con la política de acceso o si un profesor no dueño de una clase trata de modificar la misma";
              op.Responses["404"].Description = "Si el ID de la clase o usuario no son válidos";
              return op;
          });

        app.MapPost("/classes/assigned", ProfessorClasses)
          .RequireAuthorization("ProfessorOrAdmin")
          .AddEndpointFilter<UserIdFilter>()
          .Produces(StatusCodes.Status401Unauthorized)
          .Produces(StatusCodes.Status403Forbidden)
          .Produces<PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI>>(StatusCodes.Status200OK)
          .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
          .WithOpenApi(op =>
          {
              op.Summary = "Búsqueda de clases asesoradas por filtros";
              op.Description = "Obtener las clases asesoradas de un usuario, la búsuqeda considera el usuario activo como el profesor a considerar para la búsqueda";
              op.Responses["200"].Description = "Si la búsqueda fue exitosa";
              op.Responses["400"].Description = "Si el formato de entrada no es adecuado";
              op.Responses["401"].Description = "Si el usuario no está autenticado";
              op.Responses["403"].Description = "Si el usaurio no cumple con la política de acceso";
              return op;
          });

        app.MapPost("/classes/enrolled", EnrolledClasses)
          .WithName("Obtener clases inscritas")
          .RequireAuthorization("RequireAuthenticated")
          .AddEndpointFilter<UserIdFilter>()
          .Produces(StatusCodes.Status401Unauthorized)
          .Produces<PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI>>(StatusCodes.Status200OK)
          .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
          .WithOpenApi(op =>
          {
              op.Summary = "Búsqueda de clases inscritas por filtros";
              op.Description = "Obtener las clases inscritas de un usuario, la búsuqeda considera el usuario activo como el profesor a considerar para la búsqueda";
              op.Responses["200"].Description = "Si la búsqueda fue exitosa";
              op.Responses["400"].Description = "Si el formato de entrada no es adecuado";
              op.Responses["401"].Description = "Si el usuario no está autenticado";
              return op;
          });

        app.MapDelete("/classes/{id}", DeleteClass)
          .RequireAuthorization("ProfessorOrAdmin")
          .AddEndpointFilter<ExecutorFilter>()
          .Produces(StatusCodes.Status401Unauthorized)
          .Produces(StatusCodes.Status403Forbidden)
          .Produces<PublicClassMAPI>(StatusCodes.Status200OK)
          .Produces(StatusCodes.Status404NotFound)
          .WithOpenApi(op =>
          {
              op.Summary = "Eliminar clases";
              op.Description = "Eliminar una clase mediante su ID";
              op.Responses["200"].Description = "Si la eliminación fue exitosa";
              op.Responses["404"].Description = "Si no se encontró una clase con ese ID";
              op.Responses["401"].Description = "Si el usuario no está autenticado";
              op.Responses["403"].Description = "Si el usuario tiene los permisos para eliminar la clase";
              return op;
          });

        app.MapPost("/classes/enroll", EnrollClass)
          .RequireAuthorization("RequireAuthenticated")
          .AddEndpointFilter<UserIdFilter>()
          .Produces(StatusCodes.Status401Unauthorized)
          .Produces(StatusCodes.Status403Forbidden)
          .Produces(StatusCodes.Status201Created)
          .WithOpenApi(op =>
          {
              op.Summary = "Inscribirse a una clase";
              op.Description = "Inscribe al usuario que realiza la solicitud a una clase";
              op.Responses["201"].Description = "Si la inscripción fue exitosa";
              op.Responses["401"].Description = "Si el usuario no está autenticado";
              return op;
          });

        app.MapDelete("/classes/enroll/{classId}", UnenrollClass)
          .RequireAuthorization("RequireAuthenticated")
          .AddEndpointFilter<ExecutorFilter>()
          .Produces(StatusCodes.Status401Unauthorized)
          .Produces(StatusCodes.Status403Forbidden)
          .Produces(StatusCodes.Status404NotFound)
          .Produces(StatusCodes.Status200OK)
          .WithOpenApi(op =>
          {
              op.Summary = "Abandonar una clase";
              op.Description = "El usuario que realiza la solicitud abandona una clase";
              op.Responses["200"].Description = "Si la opreación fue exitosa";
              op.Responses["401"].Description = "Si el usuario no está autenticado";
              op.Responses["404"].Description = "Si el usuario no está inscrito a la clase en cuestión";
              return op;
          });

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
                return validation.UnwrapErr().FromDomain();

            var newRecord = validation.Unwrap();
            var publicRecord = ClassMAPIMapper.FromDomain(newRecord);

            return Results.Created($"/users/{publicRecord.Id}", publicRecord);
        });
    }

    // TODO: Buscar por qué me agrega active: false en el criterio
    // que indica como respuesta a pesar de no colocar ese criterio
    // en la búsqueda
    public static Task<IResult> ProfessorClasses(
        ClassCriteriaMAPI criteria,
        HttpContext ctx,
        RoutesUtils utils,
        QueryUseCase<ClassCriteriaDTO, ClassDomain> useCase)
    {
        return utils.HandleResponseAsync(async () =>
        {
            var userId = utils.GetIdFromContext(ctx);
            criteria.WithProfessor = new WithProfessorMAPI { Id = userId };

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
            criteria.WithStudent = new WithStudentMAPI { Id = userId };

            var validation = criteria.ToDomain();
            if (validation.IsErr) return utils.FieldErrorToBadRequest(validation);

            var result = await useCase.ExecuteAsync(validation.Unwrap());
            return Results.Ok(result.FromDomain());
        });
    }

    public static Task<IResult> UpdateClass(
      ClassUpdateMAPI data,
      HttpContext ctx,
      RoutesUtils utils,
      UpdateClassUseCase useCase)
    {
        return utils.HandleResponseAsync(async () =>
        {
            var professorId = utils.GetIdFromContext(ctx);
            var classUpdate = ClassMAPIMapper.ToDomain(data, professorId);
            var validation = await useCase.ExecuteAsync(classUpdate);

            if (validation.IsErr)
                return validation.UnwrapErr().FromDomain();

            var updated = validation.Unwrap();
            return Results.Ok(updated.FromDomain());

        });
    }

    public static Task<IResult> DeleteClass(
      string id,
      HttpContext ctx,
      RoutesUtils utils,
      DeleteClassUseCase useCase)
    {
        return utils.HandleResponseAsync(async () =>
        {
            var executor = utils.GetExecutorFromContext(ctx);
            var validation = await useCase.ExecuteAsync(new DeleteClassDTO
            {
                Id = id,
                Executor = executor
            });

            if (validation.IsErr)
                return validation.UnwrapErr().FromDomain();

            var deleted = validation.Unwrap();
            return Results.Ok(deleted.FromDomain());
        });
    }

    public static Task<IResult> EnrollClass(
      EnrollClassMAPI data,
      HttpContext ctx,
      RoutesUtils utils,
      EnrollClassUseCase useCase)
    {
        return utils.HandleResponseAsync(async () =>
        {
            var userId = utils.GetIdFromContext(ctx);
            var validation = await useCase.ExecuteAsync(new StudentClassRelationDTO
            {
                Id = new ClassUserRelationIdDTO { ClassId = data.ClassId, UserId = userId },
                Hidden = false
            });

            if (validation.IsErr)
                return validation.UnwrapErr().FromDomain();

            var created = validation.Unwrap();
            return Results.Created();
        });
    }

    public static Task<IResult> UnenrollClass(
      string classId,
      HttpContext ctx,
      RoutesUtils utils,
      UnEnrollClassUseCase useCase)
    {
        return utils.HandleResponseAsync(async () =>
        {
            var executor = utils.GetExecutorFromContext(ctx);
            var validation = await useCase.ExecuteAsync(new UnenrollClassDTO
            {
                Id = new ClassUserRelationIdDTO { ClassId = classId, UserId = executor.Id },
                Executor = executor
            });

            if (validation.IsErr)
                return validation.UnwrapErr().FromDomain();

            var created = validation.Unwrap();
            return Results.Ok();
        });
    }
}
