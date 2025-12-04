using Application.DTOs.Common;
using Application.DTOs.UserNotifications;
using Application.UseCases.UserNotifications;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.UserNotifications;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.UserNotifications;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Notifications;

public class UpdateUserNotificationUseCaseTest : IDisposable
{
    private readonly UpdateUserNotificationUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly UserMapper _userMapper = new();
    private readonly Random _rdm = new();

    public UpdateUserNotificationUseCaseTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var userNotificationMapper = new UserNotificationMapper();

        var reader = new UserNotificationEFReader(_ctx, userNotificationMapper);
        var updater = new UserNotificationEFUpdater(
            _ctx,
            userNotificationMapper,
            new UpdateUserNotificationEFMapper()
        );

        _useCase = new UpdateUserNotificationUseCase(updater, reader);
    }

    public static Executor AsExecutor(UserDomain u) => new() { Id = u.Id, Role = u.Role };

    public async Task<UserDomain> SeedUser(UserType role = UserType.STUDENT)
    {
        ulong id = (ulong)_rdm.NextInt64();
        var user = new User
        {
            UserId = id,
            Email = $"test{id}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)role,
        };

        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();

        return _userMapper.Map(user);
    }

    private async Task<ulong> SeedNotification(ulong userId, bool isRead)
    {
        var user = await SeedUser();
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
            UserId = user.Id,
            Readed = isRead,
        };
        _ctx.NotificationPerUsers.Add(userNotification);

        await _ctx.SaveChangesAsync();
        return notification.NotificationId;
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotificationExistsAndIsNotRead_ShouldUpdateReadedToTrue()
    {
        var user = await SeedUser();
        var notificationId = await SeedNotification(user.Id, isRead: false);
        var inputDto = new UserNotificationUpdateDTO
        {
            UserId = user.Id,
            NotificationId = notificationId,
            Readed = true,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = inputDto, Executor = AsExecutor(user) }
        );

        Assert.True(result.IsOk);

        var updatedNotification = await _ctx.NotificationPerUsers.FirstAsync(n =>
            n.NotificationId == notificationId && n.UserId == user.Id
        );
        Assert.True(updatedNotification.Readed);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotificationDoesNotExist_ShouldReturnNotFoundError()
    {
        var user = await SeedUser();
        ulong nonExistentNotificationId = 100;
        var inputDto = new UserNotificationUpdateDTO
        {
            UserId = 1,
            NotificationId = nonExistentNotificationId,
            Readed = true,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = inputDto, Executor = AsExecutor(user) }
        );

        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotificationIsAlreadyRead_ShouldReturnSuccess()
    {
        var user = await SeedUser();
        var notificationId = await SeedNotification(user.Id, isRead: true);
        var inputDto = new UserNotificationUpdateDTO
        {
            UserId = user.Id,
            NotificationId = notificationId,
            Readed = true,
        };

        var result = await _useCase.ExecuteAsync(
            new() { Data = inputDto, Executor = AsExecutor(user) }
        );

        Assert.True(result.IsOk);

        var updatedNotification = await _ctx.NotificationPerUsers.FirstAsync(n =>
            n.NotificationId == notificationId && n.UserId == user.Id
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
