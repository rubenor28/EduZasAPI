using Application.DTOs.Common;
using Application.DTOs.UserNotifications;
using Application.UseCases.UserNotifications;
using EntityFramework.Application.DAOs.UserNotifications;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.UserNotifications;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Notifications;

public class UpdateUserNotificationUseCaseTest : IDisposable
{
    private readonly UpdateUserNotificationUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    public UpdateUserNotificationUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var userNotificationMapper = new UserNotificationProjector();

        var reader = new UserNotificationEFReader(_ctx, userNotificationMapper);
        var updater = new UserNotificationEFUpdater(
            _ctx,
            userNotificationMapper,
            new UpdateUserNotificationEFMapper()
        );

        _useCase = new UpdateUserNotificationUseCase(updater, reader);
    }

    private async Task<(ulong userId, ulong notificationId)> SeedNotification(bool isRead)
    {
        var user = new User
        {
            UserId = 1,
            Email = "test@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
        };
        _ctx.Users.Add(user);

        var newClass = new Class { ClassId = "TEST-CLASS", ClassName = "Test Class" };
        _ctx.Classes.Add(newClass);

        var notification = new Notification
        {
            NotificationId = 1,
            ClassId = "TEST-CLASS",
            Title = "Test Notification",
        };

        _ctx.Notifications.Add(notification);

        var userNotification = new NotificationPerUser
        {
            NotificationId = notification.NotificationId,
            UserId = user.UserId,
            Readed = isRead,
        };
        _ctx.NotificationPerUsers.Add(userNotification);

        await _ctx.SaveChangesAsync();
        return (user.UserId, notification.NotificationId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotificationExistsAndIsNotRead_ShouldUpdateReadedToTrue()
    {
        var (userId, notificationId) = await SeedNotification(isRead: false);
        var inputDto = new UserNotificationUpdateDTO
        {
            UserId = userId,
            NotificationId = notificationId,
            Readed = true,
        };

        var result = await _useCase.ExecuteAsync(inputDto);

        Assert.True(result.IsOk);

        var updatedNotification = await _ctx.NotificationPerUsers.FirstAsync(n =>
            n.NotificationId == notificationId && n.UserId == userId
        );
        Assert.True(updatedNotification.Readed);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotificationDoesNotExist_ShouldReturnNotFoundError()
    {
        ulong nonExistentNotificationId = 100;
        var inputDto = new UserNotificationUpdateDTO
        {
            UserId = 1,
            NotificationId = nonExistentNotificationId,
            Readed = true,
        };

        var result = await _useCase.ExecuteAsync(inputDto);

        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotificationIsAlreadyRead_ShouldReturnSuccess()
    {
        var (userId, notificationId) = await SeedNotification(isRead: true);
        var inputDto = new UserNotificationUpdateDTO
        {
            UserId = userId,
            NotificationId = notificationId,
            Readed = true,
        };

        var result = await _useCase.ExecuteAsync(inputDto);

        Assert.True(result.IsOk);

        var updatedNotification = await _ctx.NotificationPerUsers.FirstAsync(n =>
            n.NotificationId == notificationId && n.UserId == userId
        );
        Assert.True(updatedNotification.Readed);
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
