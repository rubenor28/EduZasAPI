using Application.DTOs.Answers;
using Application.DTOs.ClassTests;
using Application.DTOs.ResourceViewSessions;
using Application.UseCases.Reports;
using Application.UseCases.ResourceViewSessions;
using ClosedXML.Excel;
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
            .MapGet("/answer/{userId:ulong}/{classId}/{testId:guid}/detail", GetAnswerDetail)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>();

        group
            .MapGet("/test/{classId}/{testId:guid}", GetTestReport)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>();

        group
            .MapGet("/test/{classId}", GetClassReport)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>();

        group
            .MapGet("/test/{classId}/spreadsheet", GetClassReportSpreadSheet)
            .RequireAuthorization("ProfessorOrAdmin")
            .AddEndpointFilter<ExecutorFilter>();

        group
            .MapPost("/resource/session", AddResourceViewSession)
            .RequireAuthorization("RequireAuthenticated")
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
            mapRequest: () =>
                new AnswerIdDTO
                {
                    ClassId = classId,
                    TestId = testId,
                    UserId = userId,
                },
            mapResponse: (grade) => Results.Ok(grade)
        );

    private static Task<IResult> GetAnswerDetail(
        [FromRoute] ulong userId,
        [FromRoute] string classId,
        [FromRoute] Guid testId,
        [FromServices] GetAnswerDetailUseCase getAnswerDetailUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            getAnswerDetailUseCase,
            mapRequest: () =>
                new AnswerIdDTO
                {
                    ClassId = classId,
                    TestId = testId,
                    UserId = userId,
                },
            mapResponse: (grade) => Results.Ok(grade)
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
            mapRequest: () => new ClassTestIdDTO { ClassId = classId, TestId = testId },
            mapResponse: (report) => Results.Ok(report)
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
            mapRequest: () => classId,
            mapResponse: report => Results.Ok(report)
        );

    private static Task<IResult> AddResourceViewSession(
        [FromBody] NewResourceViewSession request,
        [FromServices] AddResourceViewSessionsUseCase addResourceViewSessionsUseCase,
        [FromServices] RoutesUtils utils,
        HttpContext ctx
    ) =>
        utils.HandleUseCaseAsync(
            ctx,
            addResourceViewSessionsUseCase,
            mapRequest: () => request,
            mapResponse: _ => Results.NoContent()
        );

    private static async Task<IResult> GetClassReportSpreadSheet(
        [FromRoute] string classId,
        [FromServices] GlobalClassGradeUseCase globalClassGradeUseCase,
        [FromServices] RoutesUtils utils,
        [FromServices] XLWorkbook workbook,
        HttpContext ctx
    )
    {
        var executor = utils.GetExecutorFromContext(ctx);
        var result = await globalClassGradeUseCase.ExecuteAsync(
            new() { Data = classId, Executor = executor }
        );

        if (result.IsErr)
            return RoutesUtils.MapError(result.UnwrapErr());

        var report = result.Unwrap();
        var worksheet = workbook.Worksheets.Add("Grades");

        // Headers
        var headers = new List<string> { "Correo", "Estudiante" };
        var testIds = report.TestTitles.Keys.ToList();
        headers.AddRange(testIds.Select(id => report.TestTitles[id]));

        for (int i = 0; i < headers.Count; i++)
        {
            worksheet.Cell(1, i + 1).Value = headers[i];
        }

        // Data
        var currentRow = 2;
        foreach (var studentScore in report.StudentScores)
        {
            worksheet.Cell(currentRow, 1).Value = studentScore.Email;
            worksheet.Cell(currentRow, 2).Value = studentScore.StudentName;
            for (int i = 0; i < testIds.Count; i++)
            {
                var testId = testIds[i];
                if (studentScore.Scores.TryGetValue(testId, out var score) && score.HasValue)
                {
                    worksheet.Cell(currentRow, i + 3).Value = score.Value;
                }
                else
                {
                    worksheet.Cell(currentRow, i + 3).Value = "N/A";
                }
            }
            currentRow++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        return Results.File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"{report.ClassName}-grades.xlsx"
        );
    }
}
