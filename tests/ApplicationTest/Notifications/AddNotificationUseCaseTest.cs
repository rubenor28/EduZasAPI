using Application.DTOs.Notifications;
using Application.UseCases.Notifications;
using Domain.Enums;
using EntityFramework.Application.DAOs.Notifications;
using EntityFramework.Application.DAOs.UserNotifications;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Notifications;
using EntityFramework.InterfaceAdapters.Mappers.UserNotifications;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Notifications;

public class AddNotificationUseCaseTest : IDisposable
{
    private readonly AddNotificationUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public AddNotificationUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var notificationMapper = new NotificationMapper();
        var userNotificationMapper = new UserNotificationMapper();

        var notificationCreator = new NotificationEFCreator(
            _ctx,
            notificationMapper,
            new NewNotificationEFMapper()
        );

        var userNotificationCreator = new UserNotificationEFCreator(
            _ctx,
            userNotificationMapper,
            new NewUserNotificationEFMapper()
        );

        var classStudentsQuerier = new UserEFQuerier(_ctx, new UserProjector(), 10);

        _useCase = new AddNotificationUseCase(
            notificationCreator,
            userNotificationCreator,
            classStudentsQuerier
        );
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
    public async Task ExecuteAsync_WhenClassHasStudents_CreatesUserNotifications()
    {
        await SeedClass();
        await SeedStudent("TEST-CLASS", 1);
        await SeedStudent("TEST-CLASS", 2);

        var newNotificationDto = new NewNotificationDTO
        {
            ClassId = "TEST-CLASS",
            Title = "Test Notification",
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newNotificationDto,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsOk);
        var createdNotification = result.Unwrap();
        Assert.Equal(
            2,
            await _ctx.NotificationPerUsers.CountAsync(n =>
                n.NotificationId == createdNotification.Id
            )
        );
    }

    [Fact]
    public async Task ExecuteAsync_WhenClassHasNoStudents_CreatesNoUserNotifications()
    {
        await SeedClass();

        var newNotificationDto = new NewNotificationDTO
        {
            ClassId = "TEST-CLASS",
            Title = "Test Notification",
        };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newNotificationDto,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );

        Assert.True(result.IsOk);
        var createdNotification = result.Unwrap();
        Assert.Equal(
            0,
            await _ctx.NotificationPerUsers.CountAsync(n =>
                n.NotificationId == createdNotification.Id
            )
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidData_ReturnsError()
    {
        await SeedClass();
        var newNotificationDto = new NewNotificationDTO { ClassId = "TEST-CLASS", Title = "" };

        var result = await _useCase.ExecuteAsync(
            new()
            {
                Data = newNotificationDto,
                Executor = new() { Id = 1, Role = UserType.ADMIN },
            }
        );
        Assert.True(result.IsOk);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
