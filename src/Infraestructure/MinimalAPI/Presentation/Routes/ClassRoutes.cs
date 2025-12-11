using Application.DTOs.ClassContent;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Application.UseCases.ClassContent;
using Application.UseCases.Classes;
using Application.UseCases.ClassProfessors;
using Application.UseCases.ClassStudents;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Application.DTOs.Common;
using MinimalAPI.Presentation.Filters;

namespace MinimalAPI.Presentation.Routes;

/// <summary>
/// Define las rutas relacionadas con la gestión de clases.
/// </summary>
public static class ClassRoutes
{
    /// <summary>
    /// Mapea los endpoints para la gestión de clases.
    /// </summary>
    /// <param name="app">La aplicación web.</param>
    /// <returns>El grupo de rutas configurado.</returns>
    public static RouteGroupBuilder MapClassRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/classes").WithTags("Clases");

        group
            .MapPost("/", AddClass)
            .WithName("Crear clases")
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ClassDomain>(StatusCodes.Status201Created)
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

        group
            .MapPut("/", UpdateClass)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ClassDomain>(StatusCodes.Status200OK)
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

        group
            .MapDelete("/{id}", DeleteClass)
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

        group
            .MapPost("/assigned", ProfessorClasses)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<PaginatedQuery<ClassDomain, ClassCriteriaDTO>>(StatusCodes.Status200OK)
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

        group
            .MapPost("/enrolled", EnrolledClasses)
            .WithName("Obtener clases inscritas")
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<PaginatedQuery<ClassDomain, ClassCriteriaDTO>>(StatusCodes.Status200OK)
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

        group
            .MapPost("/enroll", EnrollClass)
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>()
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

        group
            .MapDelete("/enroll/{classId}/{userId:ulong}", UnenrollClass)
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi(op =>
            {
                op.Summary = "Abandonar una clase";
                op.Description = "El usuario que realiza la solicitud abandona una clase";
                op.Responses["204"].Description = "Si la opreación fue exitosa";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["404"].Description =
                    "Si el usuario no está inscrito a la clase en cuestión";
                return op;
            });

        group
            .MapPut("/students/relationship", UpdateClassStudent)
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi(op =>
            {
                op.Summary = "Alternar la visibilidad de una clase";
                op.Description =
                    "El usuario que realiza la solicitud alterna la visibilidad de una clase en la que está inscrito.";
                op.Responses["204"].Description = "Si la operación fue exitosa";
                op.Responses["400"].Description = "Si el usuario o clase no son válidos";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["403"].Description =
                    "Si el usuario no tiene permisos para realizar esta acción";
                op.Responses["404"].Description =
                    "Si el usuario no está inscrito a la clase en cuestión";
                return op;
            });

        group
            .MapPost("/professor", AddProfessor)
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

        group
            .MapGet("/professors/{classId}/{userId:ulong}", ReadProfessor)
            .WithName("Leer relacion usuario professor - clase")
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<ClassProfessorDomain>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Búsqueda de relacion clase - profesor";
                op.Responses["200"].Description = "Si la búsqueda fue exitosa";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["403"].Description =
                    "Si el usuario no cuenta con los permisos adecuados";
                op.Responses["404"].Description = "Si no se encontró la relación";

                return op;
            });

        group
            .MapPut("/professors", UpdateProfessor)
            .WithName("Actualizar la relacion usuario professor - clase")
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<ClassProfessorDomain>(StatusCodes.Status200OK)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Actualizar un la relación profesor - clase.";
                op.Description = "Modifica los datos de una relación profesor - clase.";
                op.Responses["200"].Description = "Recurso actualizado exitosamente.";
                op.Responses["400"].Description = "Los datos para la actualización son inválidos.";
                op.Responses["401"].Description = "Usuario no autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permisos para modificar este recurso.";
                op.Responses["404"].Description =
                    "No se encontró un recurso con el ID proporcionado.";
                return op;
            });

        group
            .MapDelete("/professors/{classId}/{userId:ulong}", RemoveProfessorFromClass)
            .WithName("Eliminar relacion usuario professor - clase")
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Eliminacion de relacion clase - profesor";
                op.Responses["204"].Description = "Si la eliminacion fue exitosa";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["403"].Description =
                    "Si el usuario no cuenta con los permisos adecuados";
                op.Responses["404"].Description = "Si no se encontró la relación";

                return op;
            });

        group
            .MapGet("/students/{classId}/{userId:ulong}", ReadStudent)
            .WithName("Leer relacion usuario estudiante - clase")
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<ClassStudentDomain>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Búsqueda de relacion clase - estudiante";
                op.Responses["200"].Description = "Si la búsqueda fue exitosa";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["404"].Description = "Si no se encontró la relación";

                return op;
            });

        group
            .MapPost("/{classId}/professors/{userId:ulong}", ProfessorsInClass)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>();

        group
            .MapPost("/content", SearchClassContent)
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>();

        return group;
    }

    public static Task<IResult> AddClass(
        [FromBody] NewClassDTO newClass,
        [FromServices] AddClassUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => newClass,
            mapResponse: (classRecord) => Results.Created($"/users/{classRecord.Id}", classRecord)
        );
    }

    public static Task<IResult> UpdateClass(
        [FromBody] ClassUpdateDTO request,
        [FromServices] UpdateClassUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: (classRecord) => Results.Ok(classRecord)
        );
    }

    public static Task<IResult> DeleteClass(
        [FromRoute] string id,
        HttpContext ctx,
        [FromServices] RoutesUtils utils,
        [FromServices] DeleteClassUseCase useCase
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => id,
            mapResponse: (_) => Results.NoContent()
        );
    }

    public static Task<IResult> EnrolledClasses(
        [FromBody] StudentClassesSummaryCriteriaDTO request,
        [FromServices] QueryStudentClassesSummaryUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: search => Results.Ok(search)
        );
    }

    public static Task<IResult> EnrollClass(
        [FromBody] UserClassRelationId req,
        HttpContext ctx,
        [FromServices] RoutesUtils utils,
        [FromServices] AddClassStudentUseCase useCase
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => req,
            mapResponse: (_) => Results.Created()
        );
    }

    public static Task<IResult> UnenrollClass(
        [FromRoute] string classId,
        [FromRoute] ulong userId,
        HttpContext ctx,
        [FromServices] RoutesUtils utils,
        [FromServices] DeleteClassStudentUseCase useCase
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => new UserClassRelationId { ClassId = classId, UserId = userId },
            mapResponse: (_) => Results.NoContent()
        );
    }

    public static Task<IResult> UpdateClassStudent(
        [FromBody] ClassStudentUpdateDTO request,
        [FromServices] RoutesUtils utils,
        [FromServices] UpdateClassStudentUseCase useCase,
        HttpContext ctx
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: (_) => Results.NoContent()
        );
    }

    public static Task<IResult> AddProfessor(
        [FromBody] NewClassProfessorDTO data,
        HttpContext ctx,
        [FromServices] RoutesUtils utils,
        [FromServices] AddClassProfessorUseCase useCase
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => data,
            mapResponse: (_) => Results.NoContent()
        );
    }

    public static Task<IResult> UpdateProfessor(
        [FromBody] ClassProfessorUpdateDTO request,
        [FromServices] UpdateClassProfessorUseCase useCase,
        HttpContext ctx,
        [FromServices] RoutesUtils utils
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: _ => Results.NoContent()
        );
    }

    public static async Task<IResult> ProfessorClasses(
        [FromBody] ProfessorClassesSummaryCriteriaDTO criteria,
        [FromServices] QueryProfessorClassesSummaryUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return await utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => criteria,
            mapResponse: search => Results.Ok(search)
        );
    }

    public static Task<IResult> SearchClassProfessors(
        [FromBody] ClassProfessorCriteriaDTO request,
        [FromServices] SearchClassProfessorUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => request,
            mapResponse: (search) => Results.Ok(search)
        );
    }

    public static Task<IResult> ReadProfessor(
        [FromRoute] string classId,
        [FromRoute] ulong userId,
        [FromServices] ReadClassProfessorUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => new UserClassRelationId { ClassId = classId, UserId = userId },
            mapResponse: professor => Results.Ok(professor)
        );
    }

    public static Task<IResult> RemoveProfessorFromClass(
        [FromRoute] string classId,
        [FromRoute] ulong userId,
        [FromServices] DeleteClassProfessorUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    )
    {
        return utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => new UserClassRelationId { ClassId = classId, UserId = userId },
            mapResponse: _ => Results.NoContent()
        );
    }

    public static Task<IResult> ProfessorsInClass(
        [FromRoute] string classId,
        [FromRoute] ulong userId,
        [FromBody] CriteriaDTO criteria,
        [FromServices] QueryClassProfessorSummaryUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () =>
                new ClassProfessorSummaryCriteriaDTO
                {
                    ClassId = classId,
                    ProfessorId = userId,
                    Page = criteria.Page,
                    PageSize = criteria.PageSize,
                },
            mapResponse: ps => Results.Ok(ps)
        );

    public static Task<IResult> SearchClassContent(
        [FromBody] ClassContentCriteriaDTO criteria,
        [FromServices] QueryClassContentUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => criteria,
            mapResponse: cc => Results.Ok(cc)
        );

    public static Task<IResult> ReadStudent(
        [FromRoute] string classId,
        [FromRoute] ulong userId,
        [FromServices] ReadClassStudentUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => new UserClassRelationId { ClassId = classId, UserId = userId },
            mapResponse: cs => Results.Ok(cs)
        );
}
