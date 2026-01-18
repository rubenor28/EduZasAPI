using Application.Services;
using MinimalAPI.Application.Jobs;
using Quartz;

namespace MinimalAPI.Application.Services;

public class QuartzTaskScheduler(ISchedulerFactory schedulerFactory, ILogger<QuartzTaskScheduler> logger) : ITaskScheduler
{
    private readonly ISchedulerFactory _schedulerFactory = schedulerFactory;
    private readonly ILogger<QuartzTaskScheduler> _logger = logger;

    public async Task ScheduleMarkAnswersAsFinished(string classId, Guid testId, DateTimeOffset deadline)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey($"mark-finished-{classId}-{testId}");

        if(await scheduler.CheckExists(jobKey))
        {
            _logger.LogWarning("Job {JobKey} already exists. It will be rescheduled to {Deadline}.", jobKey, deadline);
            await scheduler.DeleteJob(jobKey);
        }
        
        var jobDetail = JobBuilder.Create<MarkAnswersTriesAsFinished>()
            .WithIdentity(jobKey)
            .UsingJobData("ClassId", classId)
            .UsingJobData("TestId", testId.ToString())
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"trigger-finished-{classId}-{testId}")
            .StartAt(deadline)
            .Build();
        
        _logger.LogInformation("Scheduling job {JobKey} to run at {Deadline}", jobKey, deadline);
        await scheduler.ScheduleJob(jobDetail, trigger);
    }
}
