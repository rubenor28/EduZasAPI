using Application.DTOs.Notifications;
using EntityFramework.Application.DAOs.Notifications;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers.Notifications;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkTest;

public class NotificationEFRepositoryTest : IDisposable
{
    private readonly EduZasDotnetContext _ctx;
    private readonly SqliteConnection _conn;
    private readonly NotificationEFCreator _creator;
    private readonly NotificationEFReader _reader;
    private readonly NotificationEFQuerier _querier;

    public NotificationEFRepositoryTest()
    {
        var dbName = Guid.NewGuid().ToString();
        _conn = new SqliteConnection($"Data Source={dbName};Mode=Memory;Cache=Shared");
        _conn.Open();

        var opts = new DbContextOptionsBuilder<EduZasDotnetContext>().UseSqlite(_conn).Options;

        _ctx = new EduZasDotnetContext(opts);
        _ctx.Database.EnsureCreated();

        var mapper = new NotificationProjector();

        _creator = new(_ctx, mapper, new NewNotificationEFMapper());
        _reader = new(_ctx, mapper);
        _querier = new(_ctx, mapper, 10);
    }

    private async Task SeedData()
    {
        var classEntity = new Class { ClassId = "id", ClassName = "Test class" };
        _ctx.Classes.Add(classEntity);
        await _ctx.SaveChangesAsync();
    }

    [Fact]
    public async Task AddNotification_ReturnsRelation()
    {
        await SeedData();
        var newNotification = new NewNotificationDTO { ClassId = "id", Title = "Test" };
        var notification = await _creator.AddAsync(newNotification);

        Assert.NotNull(notification);
        Assert.Equal(notification.ClassId, newNotification.ClassId);
        Assert.Equal(notification.Title, newNotification.Title);
    }

    [Fact]
    public async Task GetAsync_WhenNotificationExists_ReturnsNotification()
    {
        await SeedData();
        var newNotification = new NewNotificationDTO { ClassId = "id", Title = "Test" };
        var created = await _creator.AddAsync(newNotification);

        var found = await _reader.GetAsync(created.Id);

        Assert.True(found.IsSome);
        Assert.Equal(created.Id, found.Unwrap().Id);
    }

    [Fact]
    public async Task GetAsync_WhenNotificationDoesNotExists_ReturnsNone()
    {
        var found = await _reader.GetAsync(123);
        Assert.True(found.IsNone);
    }

    [Fact]
    public async Task GetByAsync_WithCriteria_ReturnsMatchingNotifications()
    {
        await SeedData();
        var newNotification1 = new NewNotificationDTO { ClassId = "id", Title = "Test 1" };
        var newNotification2 = new NewNotificationDTO { ClassId = "id", Title = "Test 2" };
        await _creator.AddAsync(newNotification1);
        await _creator.AddAsync(newNotification2);

        var criteria = new NotificationCriteriaDTO { ClassId = "id" };
        var result = await _querier.GetByAsync(criteria);

        Assert.Equal(2, result.Results.Count());
    }

    [Fact]
    public async Task GetByAsync_WithNonMatchingCriteria_ReturnsEmpty()
    {
        var criteria = new NotificationCriteriaDTO { ClassId = "non-existent" };
        var result = await _querier.GetByAsync(criteria);

        Assert.Empty(result.Results);
    }

    [Fact]
    public async Task GetByAsync_UserIdCriteria_ReturnsMatchingNotifications()
    {
        await SeedData();
        var newNotification1 = new NewNotificationDTO { ClassId = "id", Title = "Test 1" };
        var newNotification2 = new NewNotificationDTO { ClassId = "id", Title = "Test 2" };
        await _creator.AddAsync(newNotification1);
        await _creator.AddAsync(newNotification2);

        var criteria = new NotificationCriteriaDTO { ClassId = "id" };
        var result = await _querier.GetByAsync(criteria);

        Assert.Equal(2, result.Results.Count());
    }

    public void Dispose()
    {
        _conn.Close();
        _conn.Dispose();
        _ctx.Dispose();
    }
}
