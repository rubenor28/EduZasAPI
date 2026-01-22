namespace MinimalAPI.Application.Jobs;

using Domain.Entities;
using global::Application.DAOs;
using global::Application.DTOs.Answers;
using Quartz;

public class MarkAnswersTriesAsFinished(
    ILogger<MarkAnswersTriesAsFinished> logger,
    IUpdaterAsync<ClassTestDomain, ClassTestUpdateDTO> classTestUpdater,
    IUpdaterAsync<AnswerDomain, AnswerUpdateDTO> updater,
    IQuerierAsync<AnswerDomain, AnswerCriteriaDTO> querier
) : IJob
{
    private readonly ILogger<MarkAnswersTriesAsFinished> _logger = logger;
    private readonly IUpdaterAsync<AnswerDomain, AnswerUpdateDTO> _updater = updater;
    private readonly IQuerierAsync<AnswerDomain, AnswerCriteriaDTO> _querier = querier;
    private readonly IUpdaterAsync<ClassTestDomain, ClassTestUpdateDTO> _classTestUpdater =
        classTestUpdater;

    public async Task Execute(IJobExecutionContext context)
    {
        // Recuperar parámetros pasados desde el endpoint
        var dataMap = context.MergedJobDataMap;
        var testIdString = dataMap.GetString("TestId") ?? throw new InvalidOperationException();
        var classId = dataMap.GetString("ClassId") ?? throw new InvalidOperationException();
        var testId = Guid.Parse(testIdString);

        await _classTestUpdater.UpdateAsync(
            new()
            {
                ClassId = classId,
                TestId = testId,
                AllowModifyAnswers = false,
            }
        );

        var search = await _querier.GetByAsync(new() { ClassId = classId, TestId = testId });

        var answers = search.Results.Select(a => new AnswerUpdateDTO
        {
            TestId = a.TestId,
            ClassId = a.ClassId,
            UserId = a.UserId,
            TryFinished = true,
        });

        _logger.LogInformation(
            "Finalizando intentos en clase {ClsId} en test {TestId}",
            classId,
            testId
        );

        var answersUp = await _updater.BulkUpdateAsync(answers);
    }
}
