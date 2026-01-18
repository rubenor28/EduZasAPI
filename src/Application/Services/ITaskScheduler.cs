namespace Application.Services;

public interface ITaskScheduler
{
    Task ScheduleMarkAnswersAsFinished(string classId, Guid testId, DateTimeOffset deadline);
}
