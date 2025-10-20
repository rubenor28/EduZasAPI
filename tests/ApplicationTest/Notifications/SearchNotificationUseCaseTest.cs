using Application.DTOs.Notifications;
using Application.UseCases.Notifications;
using EntityFramework.Application.DAOs.ClassStudents;
using EntityFramework.Application.DAOs.Notifications;
using EntityFramework.Application.DAOs.UserNotifications;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Notifications;

public class SearchNotificationUseCaseTest
{
    private readonly AddNotificationUseCase _addNotificationUseCase;
    private readonly SearchNotificationUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public SearchNotificationUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var notificationMapper = new NotificationEFMapper();
        var userNotificationMapper = new UserNotificationEFMapper();
        var studentClassMapper = new StudentClassEFMapper();

        var notificationCreator = new NotificationEFCreator(
            _ctx,
            notificationMapper,
            notificationMapper
        );

        var userNotificationCreator = new UserNotificationEFCreator(
            _ctx,
            userNotificationMapper,
            userNotificationMapper
        );

        var classStudentsQuerier = new ClassStudentEFQuerier(_ctx, studentClassMapper, 10);

        _addNotificationUseCase = new AddNotificationUseCase(
            notificationCreator,
            userNotificationCreator,
            classStudentsQuerier
        );

        var notificationQuerier = new NotificationEFQuerier(_ctx, notificationMapper, 10);
        _useCase = new SearchNotificationUseCase(notificationQuerier);
    }

    private async Task SeedClass(string classId = "TEST-CLASS")
    {
        _ctx.Classes.Add(new Class { ClassId = classId, ClassName = "Test Class" });
        await _ctx.SaveChangesAsync();
    }

    private async Task SeedStudent(string classId, ulong userId)
    {
        _ctx.Users.Add(
            new()
            {
                UserId = userId,
                Email = $"student{userId}@test.com",
                FirstName = "test",
                FatherLastname = "test",
                Password = "test",
            }
        );
        _ctx.ClassStudents.Add(
            new()
            {
                ClassId = classId,
                StudentId = userId,
                Hidden = false,
            }
        );
        await _ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task SearchUserNotification_ByStudent_ReturnsNotification()
    {
        await SeedClass("Test-Class");
        await SeedStudent("Test-Class", 1);

        var notification = new NewNotificationDTO
        {
            ClassId = "Test-Class",
            Title = "Test",
        };

        await _addNotificationUseCase.ExecuteAsync(notification);

        var criteria = new NotificationCriteriaDTO { UserId = 1 };

        var result = await _useCase.ExecuteAsync(criteria);

        Assert.NotNull(result);
        Assert.Equal(result.Criteria, criteria);
        Assert.True(result.Results.Any());

        var seekedNotification = result.Results.First();
        Assert.NotNull(seekedNotification);
        Assert.Equal(notification.Title, seekedNotification.Title);
        Assert.Equal(notification.ClassId, seekedNotification.ClassId);
    }

    [Fact]
    public async Task SearchUserNotification_ByStudent_ReturnsEmtpy()
    {
        var criteria = new NotificationCriteriaDTO { UserId = 1 };

        var result = await _useCase.ExecuteAsync(criteria);

        Assert.NotNull(result);
        Assert.Equal(result.Criteria, criteria);
        Assert.False(result.Results.Any());
    }

    [Fact]
    public async Task SearchUserNotification_ByClass_ReturnsNotification()
    {
        await SeedClass("Test-Class");
        await SeedStudent("Test-Class", 1);

        var notification = new NewNotificationDTO
        {
            ClassId = "Test-Class",
            Title = "Test",
        };

        await _addNotificationUseCase.ExecuteAsync(notification);

        var criteria = new NotificationCriteriaDTO { ClassId = "Test-Class" };
        var result = await _useCase.ExecuteAsync(criteria);

        Assert.NotNull(result);
        Assert.Equal(result.Criteria, criteria);
        Assert.True(result.Results.Any());

        var seekedNotification = result.Results.First();
        Assert.NotNull(seekedNotification);
        Assert.Equal(notification.Title, seekedNotification.Title);
        Assert.Equal(notification.ClassId, seekedNotification.ClassId);
    }

    [Fact]
    public async Task SearchUserNotification_ByClass_ReturnsEmtpy()
    {
        var criteria = new NotificationCriteriaDTO { ClassId = "Test-Class" };

        var result = await _useCase.ExecuteAsync(criteria);

        Assert.NotNull(result);
        Assert.Equal(result.Criteria, criteria);
        Assert.False(result.Results.Any());
    }
}
