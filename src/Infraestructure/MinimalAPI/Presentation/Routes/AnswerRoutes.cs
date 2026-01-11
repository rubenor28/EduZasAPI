using Application.DTOs.Answers;
using Application.UseCases.Answers;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Application.DTOs;
using MinimalAPI.Presentation.Filters;

namespace MinimalAPI.Presentation.Routes;

public static class AnswerRoutes
{
    public static RouteGroupBuilder MapAnswerRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/answers").WithTags("Answers");

        group
            .MapPost("", AddAnswer)
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<AnswerDomain>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<FieldErrorResponse>(StatusCodes.Status404NotFound);

        group
            .MapPut("", UpdateAnswer)
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<AnswerDomain>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<FieldErrorResponse>(StatusCodes.Status404NotFound);

        group
            .MapGet("/{userId:ulong}/{classId}/{testId:guid}", ReadAnswer)
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<AnswerDomain>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

        return group;
    }

    public static Task<IResult> AddAnswer(
        [FromBody] AnswerIdDTO newAnswer,
        [FromServices] AddAnswerUseCase addAnswerUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            addAnswerUseCase,
            mapRequest: () => newAnswer,
            mapResponse: a => Results.Created($"/answers/{a.UserId}/{a.ClassId}/{a.TestId}", a)
        );

    public static Task<IResult> UpdateAnswer(
        [FromBody] AnswerUpdateStudentDTO update,
        [FromServices] UpdateStudentAnswerUseCase updateAnswerUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            updateAnswerUseCase,
            mapRequest: () => update,
            mapResponse: a => Results.Ok(a)
        );

    public static Task<IResult> ReadAnswer(
        [FromRoute] ulong userId,
        [FromRoute] string classId,
        [FromRoute] Guid testId,
        [FromServices] ReadAnswerUseCase readAnswerUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            readAnswerUseCase,
            mapRequest: () =>
                new AnswerIdDTO
                {
                    UserId = userId,
                    ClassId = classId,
                    TestId = testId,
                },
            mapResponse: a => Results.Ok(a)
        );
}
