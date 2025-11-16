using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Application.UseCases.Classes;
using Application.UseCases.ClassProfessors;
using Application.UseCases.ClassStudents;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Classes;
using MinimalAPI.Application.DTOs.ClassProfessors;
using MinimalAPI.Application.DTOs.ClassStudents;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Presentation.Filters;

namespace MinimalAPI.Presentation.Routes;

public static class ClassRoutes
{
    public static RouteGroupBuilder MapClassRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/").WithTags("Clases");

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
                op.Responses["403"].Description =
                    "Si el usaurio no cumple con la política de acceso";
                return op;
            });

        app.MapPut("/classes", UpdateClass)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<UserIdFilter>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<PublicClassMAPI>(StatusCodes.Status200OK)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Actualización de clases";
                op.Responses["200"].Description = "Si el recurso se actualizó correctamente";
                op.Responses["400"].Description = "Si el formato de entrada no es adecuado";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["403"].Description =
                    "Si el usaurio no cumple con la política de acceso o si un profesor no dueño de una clase trata de modificar la misma";
                op.Responses["404"].Description = "Si el ID de la clase o usuario no son válidos";
                return op;
            });

        app.MapDelete("/classes/{id}", DeleteClass)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Eliminar clases";
                op.Description = "Eliminar una clase mediante su ID";
                op.Responses["204"].Description = "Si la eliminación fue exitosa";
                op.Responses["404"].Description = "Si no se encontró una clase con ese ID";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["403"].Description =
                    "Si el usuario no tiene los permisos para eliminar la clase";
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
                op.Description =
                    "Obtener las clases asesoradas de un usuario, la búsuqeda considera el usuario activo como el profesor a considerar para la búsqueda";
                op.Responses["200"].Description = "Si la búsqueda fue exitosa";
                op.Responses["400"].Description = "Si el formato de entrada no es adecuado";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["403"].Description =
                    "Si el usaurio no cumple con la política de acceso";
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
                op.Description =
                    "Obtener las clases inscritas de un usuario, la búsuqeda considera el usuario activo como el profesor a considerar para la búsqueda";
                op.Responses["200"].Description = "Si la búsqueda fue exitosa";
                op.Responses["400"].Description = "Si el formato de entrada no es adecuado";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                return op;
            });

        app.MapPost("/classes/enroll", EnrollClass)
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<UserIdFilter>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status201Created)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<MessageResponse>(StatusCodes.Status409Conflict)
            .WithOpenApi(op =>
            {
                op.Summary = "Inscribirse a una clase";
                op.Description = "Inscribe al usuario que realiza la solicitud a una clase";
                op.Responses["201"].Description = "Si la inscripción fue exitosa";
                op.Responses["400"].Description = "Si el código de la clase es inválido";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["404"].Description =
                    "Si no se encontró una clase con el código proporcionado";
                op.Responses["409"].Description = "Si el usuario ya está inscrito en la clase";
                return op;
            });

        app.MapDelete("/classes/enroll/{classId}/{userId:ulong}", UnenrollClass)
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
                op.Responses["404"].Description =
                    "Si el usuario no está inscrito a la clase en cuestión";
                return op;
            });

        app.MapPatch("/classes", UpdateClass)
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi(op =>
            {
                op.Summary = "Alternar la visibilidad de una clase";
                op.Description =
                    "El usuario que realiza la solicitud alterna la visibilidad de una clase en la que está inscrito.";
                op.Responses["200"].Description = "Si la operación fue exitosa";
                op.Responses["400"].Description = "Si el usuario o clase no son válidos";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["403"].Description =
                    "Si el usuario no tiene permisos para realizar esta acción";
                op.Responses["404"].Description =
                    "Si el usuario no está inscrito a la clase en cuestión";
                return op;
            });

        app.MapPost("/classes/professors", AddProfessor)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status201Created)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<MessageResponse>(StatusCodes.Status409Conflict)
            .WithOpenApi(op =>
            {
                op.Summary = "Asignar un profesor a una clase";
                op.Description =
                    "Asigna un profesor existente a una clase, con la opción de designarlo como propietario.";
                op.Responses["201"].Description = "Si el profesor fue asignado correctamente.";
                op.Responses["400"].Description =
                    "Si los datos de entrada (ej. ID de usuario) son inválidos.";
                op.Responses["401"].Description = "Si el usuario no está autenticado.";
                op.Responses["403"].Description =
                    "Si el usuario no tiene permisos para asignar profesores (ej. no es dueño de la clase).";
                op.Responses["404"].Description =
                    "Si no se encontró la clase o el usuario a asignar.";
                op.Responses["409"].Description = "Si el profesor ya está asignado a esa clase.";
                return op;
            });

        return group;
    }

    public static Task<IResult> AddClass(
        NewClassMAPI newClass,
        AddClassUseCase useCase,
        IMapper<NewClassMAPI, Executor, NewClassDTO> requestMapper,
        IMapper<ClassDomain, PublicClassMAPI> responseMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => requestMapper.Map(newClass, utils.GetExecutorFromContext(ctx)),
            mapResponse: (classRecord) =>
                Results.Created($"/users/{classRecord.Id}", responseMapper.Map(classRecord))
        );
    }

    public static Task<IResult> UpdateClass(
        ClassUpdateMAPI request,
        UpdateClassUseCase useCase,
        IMapper<ClassDomain, PublicClassMAPI> responseMapper,
        IMapper<ClassUpdateMAPI, Executor, ClassUpdateDTO> requestMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => requestMapper.Map(request, utils.GetExecutorFromContext(ctx)),
            mapResponse: (classRecord) => Results.Ok(responseMapper.Map(classRecord))
        );
    }

    public static Task<IResult> DeleteClass(
        string id,
        HttpContext ctx,
        RoutesUtils utils,
        DeleteClassUseCase useCase,
        IMapper<string, Executor, DeleteClassDTO> requestMapper
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => requestMapper.Map(id, utils.GetExecutorFromContext(ctx)),
            mapResponse: (_) => Results.NoContent()
        );
    }

    public static Task<IResult> EnrolledClasses(
        ClassCriteriaMAPI request,
        QueryClassUseCase useCase,
        IMapper<
            ClassCriteriaMAPI,
            Result<ClassCriteriaDTO, IEnumerable<FieldErrorDTO>>
        > requestMapper,
        IMapper<
            PaginatedQuery<ClassDomain, ClassCriteriaDTO>,
            PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI>
        > responseMapper,
        HttpContext ctx,
        RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () =>
                requestMapper.Map(
                    request with
                    {
                        WithStudent = new()
                        {
                            Id = utils.GetIdFromContext(ctx),
                            Hidden = request.WithStudent?.Hidden,
                        },
                    }
                ),
            mapResponse: (search) => Results.Ok(responseMapper.Map(search))
        );
    }

    public static Task<IResult> EnrollClass(
        EnrollClassMAPI data,
        HttpContext ctx,
        RoutesUtils utils,
        AddClassStudentUseCase useCase,
        IMapper<EnrollClassMAPI, Executor, NewClassStudentDTO> requestMapper
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => requestMapper.Map(data, utils.GetExecutorFromContext(ctx)),
            mapResponse: (_) => Results.Created()
        );
    }

    public static Task<IResult> UnenrollClass(
        string classId,
        ulong userId,
        IMapper<string, ulong, Executor, DeleteClassStudentDTO> requestMapper,
        HttpContext ctx,
        RoutesUtils utils,
        DeleteClassStudentUseCase useCase
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => requestMapper.Map(classId, userId, utils.GetExecutorFromContext(ctx)),
            mapResponse: (_) => Results.NoContent()
        );
    }

    public static Task<IResult> UpdateClassStudent(
        ClassStudentUpdateMAPI request,
        HttpContext ctx,
        RoutesUtils utils,
        UpdateClassStudentUseCase useCase,
        IMapper<ClassStudentUpdateMAPI, Executor, ClassStudentUpdateDTO> requestMapper
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => requestMapper.Map(request, utils.GetExecutorFromContext(ctx)),
            mapResponse: (_) => Results.NoContent()
        );
    }

    public static Task<IResult> AddProfessor(
        ClassProfessorMAPI data,
        HttpContext ctx,
        RoutesUtils utils,
        AddClassProfessorUseCase useCase,
        IMapper<ClassProfessorMAPI, Executor, NewClassProfessorDTO> requestMapper
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => requestMapper.Map(data, utils.GetExecutorFromContext(ctx)),
            mapResponse: (_) => Results.NoContent()
        );
    }

    public static Task<IResult> UpdateProfessor(
        ClassProfessorMAPI request,
        UpdateClassProfessorUseCase useCase,
        HttpContext ctx,
        RoutesUtils utils,
        IMapper<ClassProfessorMAPI, Executor, ClassProfessorUpdateDTO> reqMapper
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () => reqMapper.Map(request, utils.GetExecutorFromContext(ctx)),
            mapResponse: _ => Results.NoContent()
        );
    }

    public static Task<IResult> ProfessorClasses(
        ClassCriteriaMAPI criteria,
        HttpContext ctx,
        RoutesUtils utils,
        QueryClassUseCase useCase,
        IMapper<
            ClassCriteriaMAPI,
            Result<ClassCriteriaDTO, IEnumerable<FieldErrorDTO>>
        > requestMapper,
        IMapper<
            PaginatedQuery<ClassDomain, ClassCriteriaDTO>,
            PaginatedQuery<PublicClassMAPI, ClassCriteriaMAPI>
        > responseMapper
    )
    {
        return utils.HandleUseCaseAsync(
            useCase,
            mapRequest: () =>
                requestMapper.Map(
                    criteria with
                    {
                        WithProfessor = new()
                        {
                            Id = utils.GetIdFromContext(ctx),
                            IsOwner = criteria.WithProfessor?.IsOwner,
                        },
                    }
                ),
            mapResponse: (search) => Results.Ok(responseMapper.Map(search))
        );
    }
}
