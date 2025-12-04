using Application.DTOs.UserNotifications;
using Domain.Entities;
using EntityFramework.Application.DAOs.UserNotifications;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.UserNotifications;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkTest;

public class UserNotificationEFRepositoryTest : IDisposable
{
    private readonly SqliteConnection _conn;
    private readonly EduZasDotnetContext _ctx;
    private readonly UserNotificationEFCreator _creator;
    private readonly UserNotificationEFReader _reader;
    private readonly UserNotificationEFUpdater _updater;

    public UserNotificationEFRepositoryTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var mapper = new UserNotificationMapper();

        _creator = new(_ctx, mapper, new NewUserNotificationEFMapper());
        _reader = new(_ctx, mapper);
        _updater = new(_ctx, mapper, new UpdateUserNotificationEFMapper());
    }

    private async Task SeedData()
    {
        _ctx.Users.Add(
            new User
            {
                UserId = 1,
                Email = "test@test.com",
                FirstName = "test",
                FatherLastname = "test",
                Password = "pwd",
            }
        );
        _ctx.Classes.Add(new Class { ClassId = "class1", ClassName = "Test Class" });
        await _ctx.SaveChangesAsync();
        _ctx.Notifications.Add(
            new Notification
            {
                NotificationId = 1,
                ClassId = "class1",
                Title = "Test Notification",
            }
        );
        await _ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task AddUserNotification_ReturnsRelation()
    {
        await SeedData();
        var newUserNotification = new NewUserNotificationDTO { UserId = 1, NotificationId = 1 };

        var created = await _creator.AddAsync(newUserNotification);

        Assert.NotNull(created);
        Assert.Equal(newUserNotification.UserId, created.Id.UserId);
        Assert.Equal(newUserNotification.NotificationId, created.Id.NotificationId);
        Assert.False(created.Readed);
    }

    [Fact]
    public async Task GetAsync_WhenRelationExists_ReturnsRelation()
    {
        await SeedData();
        var newUserNotification = new NewUserNotificationDTO { UserId = 1, NotificationId = 1 };
        var created = await _creator.AddAsync(newUserNotification);

        var found = await _reader.GetAsync(created.Id);

        Assert.NotNull(found);
        Assert.Equal(created.Id.UserId, found.Id.UserId);
        Assert.Equal(created.Id.NotificationId, found.Id.NotificationId);
    }

    [Fact]
    public async Task GetAsync_WhenRelationDoesNotExist_ReturnsNone()
    {
        var id = new UserNotificationIdDTO { UserId = 123, NotificationId = 456 };
        var found = await _reader.GetAsync(id);
        Assert.Null(found);
    }

    [Fact]
    public async Task UpdateUserNotification_ReturnsUpdatedRelation()
    {
        await SeedData();
        var newUserNotification = new NewUserNotificationDTO { UserId = 1, NotificationId = 1 };
        var created = await _creator.AddAsync(newUserNotification);

        var toUpdate = new UserNotificationUpdateDTO
        {
            UserId = created.Id.UserId,
            NotificationId = created.Id.NotificationId,
            Readed = true,
        };

        var updated = await _updater.UpdateAsync(toUpdate);

        Assert.NotNull(updated);
        Assert.True(updated.Readed);
    }

    [Fact]
    public async Task UpdateAsync_WhenRelationDoesNotExist_ThrowsException()
    {
        var toUpdate = new UserNotificationUpdateDTO
        {
            UserId = 123,
            NotificationId = 456,
            Readed = true,
        };
        await Assert.ThrowsAsync<ArgumentException>(() => _updater.UpdateAsync(toUpdate));
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
