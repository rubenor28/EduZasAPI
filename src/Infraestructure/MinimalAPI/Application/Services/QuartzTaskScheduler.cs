using System.Text.Json;
using Application.DTOs.Notifications;
using Application.Services;
using MinimalAPI.Application.Jobs;
using Quartz;

namespace MinimalAPI.Application.Services;

public class QuartzTaskScheduler(
    ISchedulerFactory schedulerFactory,
    ILogger<QuartzTaskScheduler> logger
) : ITaskScheduler
{
    private readonly ISchedulerFactory _schedulerFactory = schedulerFactory;
    private readonly ILogger<QuartzTaskScheduler> _logger = logger;

    public async Task BulkSendEmail(IEnumerable<EmailMessage> messages)
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var operationId = Guid.NewGuid().ToString();

        var jobDetail = JobBuilder
            .Create<SendMultipleEmailJob>()
            .WithIdentity($"send-emails-{operationId}")
            .UsingJobData("EmailsJson", JsonSerializer.Serialize(messages))
            .Build();

        var trigger = TriggerBuilder
            .Create()
            .WithIdentity($"trigger-emails-{operationId}")
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(jobDetail, trigger);
    }

    public async Task CreateNotification(
        NewNotificationDTO notification,
        IEnumerable<ulong> usersIds
    )
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey($"notify-{notification.ClassId}-{notification.Title}");

        if (await scheduler.CheckExists(jobKey))
        {
            _logger.LogWarning("Job {JobKey} already exists.", jobKey);
            return;
        }

        var jobDetail = JobBuilder
            .Create<CreateNotificationJob>()
            .WithIdentity(jobKey)
            .UsingJobData("Title", notification.Title)
            .UsingJobData("ClassId", notification.ClassId)
            .UsingJobData("UserIdsJson", JsonSerializer.Serialize(usersIds))
            .Build();

        var trigger = TriggerBuilder
            .Create()
            .WithIdentity($"trigger-finished-{notification.ClassId}-{notification.Title}")
            .Build();

        _logger.LogInformation("Scheduling job {JobKey}", jobKey);
        await scheduler.ScheduleJob(jobDetail, trigger);
    }

    public async Task ScheduleMarkAnswersAsFinished(
        string classId,
        Guid testId,
        DateTimeOffset deadline
    )
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        var jobKey = new JobKey($"mark-finished-{classId}-{testId}");

        if (await scheduler.CheckExists(jobKey))
        {
            _logger.LogWarning(
                "Job {JobKey} already exists. It will be rescheduled to {Deadline}.",
                jobKey,
                deadline
            );
            await scheduler.DeleteJob(jobKey);
        }

        var jobDetail = JobBuilder
            .Create<MarkAnswersTriesAsFinished>()
            .WithIdentity(jobKey)
            .UsingJobData("ClassId", classId)
            .UsingJobData("TestId", testId.ToString())
            .Build();

        var trigger = TriggerBuilder
            .Create()
            .WithIdentity($"trigger-finished-{classId}-{testId}")
            .StartAt(deadline)
            .Build();

        _logger.LogInformation("Scheduling job {JobKey} to run at {Deadline}", jobKey, deadline);
        await scheduler.ScheduleJob(jobDetail, trigger);
    }
}
