using Application.DTOs.Common;
using Application.DTOs.Notifications;
using Application.UseCases.Notifications;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Notifications;
using EntityFramework.Application.DAOs.UserNotifications;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Classes;
using EntityFramework.InterfaceAdapters.Mappers.Notifications;
using EntityFramework.InterfaceAdapters.Mappers.UserNotifications;
using EntityFramework.InterfaceAdapters.Mappers.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTest.Notifications;

public class SearchNotificationUseCaseTest
{
    private readonly AddNotificationUseCase _addNotificationUseCase;
    private readonly SearchNotificationUseCase _useCase;
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;

    private readonly Random _rdm = new();

    private readonly ClassMapper _classMapper = new();
    private readonly UserMapper _userMapper = new();

    public SearchNotificationUseCaseTest()
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

        var userProjector = new UserProjector();
        var userQuerier = new UserEFQuerier(_ctx, userProjector, 10);

        _addNotificationUseCase = new AddNotificationUseCase(
            notificationCreator,
            userNotificationCreator,
            userQuerier
        );

        var notificationProjector = new NotificationProjector();
        var notificationQuerier = new NotificationEFQuerier(_ctx, notificationProjector, 10);
        _useCase = new SearchNotificationUseCase(notificationQuerier);
    }

    private async Task<UserDomain> SeedUser(UserType role = UserType.STUDENT)
    {
        var id = (ulong)_rdm.NextInt64(1, 100_000);

        var user = new User
        {
            UserId = id,
            Email = $"student{id}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
            Role = (uint)role,
        };

        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        return _userMapper.Map(user);
    }

    private async Task<(ClassDomain, UserDomain)> SeedStudent()
    {
        var student = await SeedUser();

        var cls = new Class { ClassId = $"class-test-{student.Id}", ClassName = "Test Class" };

        _ctx.Classes.Add(cls);
        _ctx.ClassStudents.Add(
            new()
            {
                ClassId = $"class-test-{student.Id}",
                StudentId = student.Id,
                Hidden = false,
            }
        );

        await _ctx.SaveChangesAsync();
        return (_classMapper.Map(cls), student);
    }

    private static Executor AsExecutor(UserDomain u) => new() { Id = u.Id, Role = u.Role };

    [Fact]
    public async Task SearchUserNotification_ByStudent_ReturnsNotification()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var (cls, user) = await SeedStudent();

        var notification = new NewNotificationDTO { ClassId = cls.Id, Title = "Test" };

        await _addNotificationUseCase.ExecuteAsync(
            new() { Data = notification, Executor = AsExecutor(admin) }
        );

        var criteria = new NotificationCriteriaDTO { PageSize = 10, UserId = user.Id };

        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        var search = result.Unwrap();

        Assert.NotNull(search);
        Assert.Equal(search.Criteria, criteria);
        Assert.True(search.Results.Any());

        var seekedNotification = search.Results.First();
        Assert.NotNull(seekedNotification);
        Assert.Equal(notification.Title, seekedNotification.Title);
        Assert.Equal(notification.ClassId, seekedNotification.ClassId);
    }

    [Fact]
    public async Task SearchUserNotification_ByStudent_ReturnsEmtpy()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var criteria = new NotificationCriteriaDTO { PageSize = 10, UserId = 1 };

        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        var search = result.Unwrap();
        Assert.NotNull(search);
        Assert.Equal(search.Criteria, criteria);
        Assert.False(search.Results.Any());
    }

    [Fact]
    public async Task SearchUserNotification_ByClass_ReturnsNotification()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var (cls, _) = await SeedStudent();

        var notification = new NewNotificationDTO { ClassId = cls.Id, Title = "Test" };

        await _addNotificationUseCase.ExecuteAsync(
            new() { Data = notification, Executor = AsExecutor(admin) }
        );

        var criteria = new NotificationCriteriaDTO { PageSize = 10, ClassId = cls.Id };
        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        var search = result.Unwrap();
        Assert.NotNull(search);
        Assert.Equal(search.Criteria, criteria);
        Assert.True(search.Results.Any());

        var seekedNotification = search.Results.First();
        Assert.NotNull(seekedNotification);
        Assert.Equal(notification.Title, seekedNotification.Title);
        Assert.Equal(notification.ClassId, seekedNotification.ClassId);
    }

    [Fact]
    public async Task SearchUserNotification_ByClass_ReturnsEmtpy()
    {
        var admin = await SeedUser(UserType.ADMIN);
        var criteria = new NotificationCriteriaDTO { PageSize = 10, ClassId = "Test-Class" };
        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        Assert.True(result.IsOk);
        var search = result.Unwrap();
        Assert.NotNull(search);
        Assert.Equal(search.Criteria, criteria);
        Assert.False(search.Results.Any());
    }
}
