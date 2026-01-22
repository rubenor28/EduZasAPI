using Application.DTOs.ClassTests;
using Application.DTOs.Tests;
using Application.UseCases.ClassTests;
using Application.UseCases.Tests;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Application.DTOs;
using MinimalAPI.Presentation.Filters;

namespace MinimalAPI.Presentation.Routes;

/// <summary>
/// Define las rutas relacionadas con las evaluaciones (tests).
/// </summary>
public static class TestRoutes
{
    /// <summary>
    /// Mapea los endpoints para la gestión de evaluaciones.
    /// </summary>
    /// <param name="app">La aplicación web.</param>
    /// <returns>El grupo de rutas configurado.</returns>
    public static RouteGroupBuilder MapTestRoutes(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Tests");

        group
            .MapPost("/test", AddTest)
            .WithName("Agregar evaluacion")
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<TestDomain>(StatusCodes.Status201Created)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary = "Crear una nueva evaluacion.";
                op.Responses["201"].Description = "La evaluacion fue creada exitosamente.";
                op.Responses["400"].Description =
                    "Los datos proporcionados tienen un formato incorrecto.";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["403"].Description =
                    "Si el usaurio no cumple con la política de acceso";

                return op;
            });

        group
            .MapGet("/tests/{testId:guid}", GetTest)
            .WithName("Obtener evaluacion por ID")
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<TestDomain>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Obtener una evaluacion dado un ID.";
                op.Responses["200"].Description = "La evaluación enconrtada.";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["403"].Description =
                    "Si el usaurio no cumple con la política de acceso";
                op.Responses["404"].Description = "La evaluacion no existe.";

                return op;
            });

        group
            .MapDelete("/tests/{testId:guid}", DeleteTest)
            .WithName("Eliminar evaluacion")
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<TestDomain>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Eliminar una evaluacion dado un ID.";
                op.Responses["200"].Description = "La evaluacion eliminada.";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["403"].Description =
                    "Si el usaurio no cumple con la política de acceso";
                op.Responses["404"].Description = "La evaluacion no existe.";

                return op;
            });

        group
            .MapPost("/tests", SearchTests)
            .WithName("Buscar tests")
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PaginatedQuery<TestSummary, TestCriteriaDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Eliminar una evaluacion dado un ID.";
                op.Responses["200"].Description = "La evaluacion enconrtado.";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["403"].Description =
                    "Si el usaurio no cumple con la política de acceso";
                op.Responses["404"].Description = "El usuario no existe.";

                return op;
            });

        group
            .MapPut("/test", UpdateTest)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<TestDomain>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Actualizar los datos de una evaluacion";
                op.Responses["200"].Description = "Si la operación fue exitosa";
                op.Responses["400"].Description = "Si los campos no son válidos";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["403"].Description =
                    "Si el usuario no tiene permisos para realizar esta acción";
                op.Responses["404"].Description = "Si la evaluacion no existe";
                return op;
            });

        group
            .MapPost("/tests/assigned", SearchClassTestAssociations)
            .WithName("Buscar asociaciones de tests y clases")
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PaginatedQuery<ClassTestAssociationDTO, ClassTestAssociationCriteriaDTO>>(
                StatusCodes.Status200OK
            )
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithOpenApi(op =>
            {
                op.Summary =
                    "Obtener una lista paginada de todas las clases de un profesor, indicando si una evaluación específica está asociada a cada una.";
                op.Responses["200"].Description = "La lista de asociaciones.";
                op.Responses["401"].Description = "Si el usuario no está autenticado.";
                op.Responses["403"].Description =
                    "Si el usuario no tiene permisos para realizar esta acción.";
                return op;
            });

        group
            .MapPost("tests/classes", AddClassTest)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<TestDomain>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .WithOpenApi(op =>
            {
                op.Summary = "Asignar una evaluacion a una clase";
                op.Responses["200"].Description = "Si la operación fue exitosa";
                op.Responses["400"].Description = "Si los campos no son válidos";
                op.Responses["401"].Description = "Si el usuario no está autenticado";
                op.Responses["403"].Description =
                    "Si el usuario no tiene permisos para realizar esta acción";
                return op;
            });

        group
            .MapDelete("tests/{testId:guid}/{classId}", DeleteClassTest)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<FieldErrorResponse>(StatusCodes.Status400BadRequest)
            .WithOpenApi(op =>
            {
                op.Summary = "Eliminar la asignación de una evaluación a una clase.";
                op.Responses["204"].Description = "Si la operación fue exitosa.";
                op.Responses["400"].Description = "Si los campos no son válidos.";
                op.Responses["401"].Description = "Si el usuario no está autenticado.";
                op.Responses["403"].Description =
                    "Si el usuario no tiene permisos para realizar esta acción.";
                return op;
            });

        group
            .MapGet("tests/{testId:guid}/{classId}", ReadPublicTest)
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<PublicTestDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(op =>
            {
                op.Summary = "Obtener la evaluación asociada a una clase.";
                op.Description =
                    "Obtener la evaluación asociada a una clase con preguntas y respuestas mezcladas.";
                op.Responses["200"].Description = "La sesión se cerró exitosamente.";
                op.Responses["401"].Description = "El usuario no está autenticado.";
                op.Responses["403"].Description =
                    "El usuario no tiene permiso para leer la evaluación.";
                op.Responses["404"].Description = "No se encontró la evaluación.";
                return op;
            });

        return group;
    }

    public static Task<IResult> AddTest(
        [FromBody] NewTestDTO newTest,
        [FromServices] AddTestUseCase addTestUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            addTestUseCase,
            mapRequest: () => newTest,
            mapResponse: t => Results.Created($"/tests/{t.Id}", t)
        );

    public static Task<IResult> GetTest(
        [FromRoute] Guid testId,
        [FromServices] ReadTestUseCase readTestUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            readTestUseCase,
            mapRequest: () => testId,
            mapResponse: t => Results.Ok(t)
        );

    public static Task<IResult> DeleteTest(
        [FromRoute] Guid testId,
        [FromServices] DeleteTestUseCase deleteTestUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            deleteTestUseCase,
            mapRequest: () => testId,
            mapResponse: t => Results.Ok(t)
        );

    public static Task<IResult> SearchTests(
        [FromBody] TestCriteriaDTO testCriteria,
        [FromServices] QueryTestSummaryUseCase queryTestSummaryUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            queryTestSummaryUseCase,
            mapRequest: () => testCriteria,
            mapResponse: ts => Results.Ok(ts)
        );

    public static Task<IResult> UpdateTest(
        [FromBody] TestUpdateDTO testUpdate,
        [FromServices] UpdateTestUseCase updateTestUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            updateTestUseCase,
            mapRequest: () => testUpdate,
            mapResponse: t => Results.Ok(t)
        );

    public static Task<IResult> AddClassTest(
        [FromBody] ClassTestIdDTO newClassTest,
        [FromServices] AddClassTestUseCase addClassTestUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            addClassTestUseCase,
            mapRequest: () => newClassTest,
            mapResponse: _ => Results.NoContent()
        );

    public static Task<IResult> DeleteClassTest(
        [FromRoute] string classId,
        [FromRoute] Guid testId,
        [FromServices] DeleteClassTestUseCase deleteClassTestUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            deleteClassTestUseCase,
            mapRequest: () => new ClassTestIdDTO { ClassId = classId, TestId = testId },
            mapResponse: _ => Results.NoContent()
        );

    public static Task<IResult> SearchClassTestAssociations(
        [FromBody] ClassTestAssociationCriteriaDTO criteria,
        [FromServices] QueryClassTestAssociationUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(ctx, useCase, mapRequest: () => criteria, mapResponse: Results.Ok);

    public static Task<IResult> ReadPublicTest(
        [FromRoute] Guid testId,
        [FromRoute] string classId,
        [FromServices] ReadPublicTestUseCase useCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            useCase,
            mapRequest: () => new PublicTestIdDTO { TestId = testId, ClassId = classId },
            mapResponse: t => Results.Ok(t)
        );
}
