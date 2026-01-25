using Application.DTOs.Answers;
using Application.DTOs.ClassTests;
using Application.UseCases.Reports;
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
            .RequireAuthorization("RequireAuthenticated")
            .AddEndpointFilter<ExecutorFilter>();

        group
            .MapGet("/test/{classId}/{testId:guid}", GetTestReport)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>();

        group
            .MapGet("/test/{classId}", GetClassReport)
            .RequireAuthorization("ProfessorOrAdmin")
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

    private static Task<IResult> GetTestReport(
        [FromRoute] string classId,
        [FromRoute] Guid testId,
        [FromServices] ClassTestAnswersGradeUseCase classTestAnswersGradeUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            classTestAnswersGradeUseCase,
            () => new ClassTestIdDTO { ClassId = classId, TestId = testId },
            (report) => Results.Ok(report)
        );

    private static Task<IResult> GetClassReport(
        [FromRoute] string classId,
        [FromServices] GlobalClassGradeUseCase globalClassGradeUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            globalClassGradeUseCase,
            () => classId,
            report => Results.Ok(report)
        );
}
