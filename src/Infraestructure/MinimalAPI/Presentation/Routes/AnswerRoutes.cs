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
        var group = app.MapGroup("").WithTags("Answers");

        group
            .MapPost("/answers", AddAnswer)
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>()
            .Produces<AnswerDomain>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<FieldErrorResponse>(StatusCodes.Status404NotFound);

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
}
