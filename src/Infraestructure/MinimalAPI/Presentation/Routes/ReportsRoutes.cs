using Application.DTOs.Answers;
using Application.Services.Graders;
using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Presentation.Filters;
using MinimalAPI.Presentation.Routes;

public static class ReportsRoutes
{
    public static RouteGroupBuilder MapReportRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/reports").WithTags("Reportes");

        group
            .MapGet("/answer/{userId:ulong}/{classId}/{testId:guid}", GetAnswerReport)
            .AddEndpointFilter<ExecutorFilter>();

        return group;
    }

    private static Task<IResult> GetAnswerReport(
        [FromRoute] ulong userId,
        [FromRoute] string classId,
        [FromRoute] Guid testId,
        [FromServices] AnswerGradeUseCase testGradeUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            testGradeUseCase,
            () =>
                new AnswerIdDTO
                {
                    ClassId = classId,
                    TestId = testId,
                    UserId = userId,
                },
            (grade) => Results.Ok(grade)
        );
}
