using Application.DTOs.Notifications;
using Application.UseCases.Notifications;
using Domain.Entities;
using Domain.Enums;
using EntityFramework.Application.DAOs.Notifications;
using EntityFramework.Application.DAOs.UserNotifications;
using EntityFramework.Application.DAOs.Users;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Users;
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

    private readonly ClassEFMapper _classMapper = new();
    private readonly UserEFMapper _userMapper;

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

        var roleMapper = new UserTypeUintMapper();
        _userMapper = new UserEFMapper(roleMapper);
        var userQuerier = new UserEFQuerier(_ctx, _userMapper, 10);

        _addNotificationUseCase = new AddNotificationUseCase(
            notificationCreator,
            userNotificationCreator,
            userQuerier
        );

        var notificationQuerier = new NotificationEFQuerier(_ctx, notificationMapper, 10);
        _useCase = new SearchNotificationUseCase(notificationQuerier);
    }

    private async Task<(ClassDomain, UserDomain)> SeedStudent()
    {
        var id = (ulong)_rdm.NextInt64(1, 100_000);

        var user = new User
        {
            UserId = id,
            Email = $"student{id}@test.com",
            FirstName = "test",
            FatherLastname = "test",
            Password = "test",
        };

        var cls = new Class { ClassId = $"class-test-{id}", ClassName = "Test Class" };

        _ctx.Users.Add(user);

        _ctx.Classes.Add(cls);

        _ctx.ClassStudents.Add(
            new()
            {
                ClassId = $"class-test-{id}",
                StudentId = id,
                Hidden = false,
            }
        );

        await _ctx.SaveChangesAsync();
        return (_classMapper.Map(cls), _userMapper.Map(user));
    }

    [Fact]
    public async Task SearchUserNotification_ByStudent_ReturnsNotification()
    {
        var (cls, user) = await SeedStudent();

        var notification = new NewNotificationDTO { ClassId = cls.Id, Title = "Test" };

        await _addNotificationUseCase.ExecuteAsync(notification);

        var criteria = new NotificationCriteriaDTO { UserId = user.Id };

        var result = await _useCase.ExecuteAsync(criteria);

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
        var criteria = new NotificationCriteriaDTO { UserId = 1 };

        var result = await _useCase.ExecuteAsync(criteria);

        Assert.True(result.IsOk);
        var search = result.Unwrap();
        Assert.NotNull(search);
        Assert.Equal(search.Criteria, criteria);
        Assert.False(search.Results.Any());
    }

    [Fact]
    public async Task SearchUserNotification_ByClass_ReturnsNotification()
    {
        var (cls, _) = await SeedStudent();

        var notification = new NewNotificationDTO { ClassId = cls.Id, Title = "Test" };

        await _addNotificationUseCase.ExecuteAsync(notification);

        var criteria = new NotificationCriteriaDTO { ClassId = cls.Id };
        var result = await _useCase.ExecuteAsync(criteria);

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
        var criteria = new NotificationCriteriaDTO { ClassId = "Test-Class" };
        var result = await _useCase.ExecuteAsync(criteria);

        Assert.True(result.IsOk);
        var search = result.Unwrap();
        Assert.NotNull(search);
        Assert.Equal(search.Criteria, criteria);
        Assert.False(search.Results.Any());
    }
}
