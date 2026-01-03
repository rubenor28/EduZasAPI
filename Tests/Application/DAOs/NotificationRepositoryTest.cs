using Application.DAOs;
using Application.DTOs.Notifications;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.DAOs;

public class NotificationRepositoryTest : BaseTest
{
    private readonly ICreatorAsync<NotificationDomain, NewNotificationDTO> _creator;
    private readonly IReaderAsync<ulong, NotificationDomain> _reader;
    private readonly IQuerierAsync<NotificationDomain, NotificationCriteriaDTO> _querier;

    public NotificationRepositoryTest()
    {
        _creator = _sp.GetRequiredService<ICreatorAsync<NotificationDomain, NewNotificationDTO>>();
        _reader = _sp.GetRequiredService<IReaderAsync<ulong, NotificationDomain>>();
        _querier = _sp.GetRequiredService<IQuerierAsync<NotificationDomain, NotificationCriteriaDTO>>();
    }
    
    [Fact]
    public async Task AddNotification_ReturnsRelation()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(owner.Id, "test-class");
        var newNotification = new NewNotificationDTO { ClassId = cls.Id, Title = "Test" };
        
        // Act
        var notification = await _creator.AddAsync(newNotification);

        // Assert
        Assert.NotNull(notification);
        Assert.Equal(notification.ClassId, newNotification.ClassId);
        Assert.Equal(notification.Title, newNotification.Title);
    }

    [Fact]
    public async Task GetAsync_WhenNotificationExists_ReturnsNotification()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(owner.Id, "test-class");
        var newNotification = new NewNotificationDTO { ClassId = cls.Id, Title = "Test" };
        var created = await _creator.AddAsync(newNotification);

        // Act
        var found = await _reader.GetAsync(created.Id);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(created.Id, found.Id);
    }

    [Fact]
    public async Task GetAsync_WhenNotificationDoesNotExists_ReturnsNone()
    {
        // Act
        var found = await _reader.GetAsync(123);

        // Assert
        Assert.Null(found);
    }

    [Fact]
    public async Task GetByAsync_WithCriteria_ReturnsMatchingNotifications()
    {
        // Arrange
        var owner = await SeedUser(UserType.PROFESSOR);
        var cls = await SeedClass(owner.Id, "test-class");
        var newNotification1 = new NewNotificationDTO { ClassId = cls.Id, Title = "Test 1" };
        var newNotification2 = new NewNotificationDTO { ClassId = cls.Id, Title = "Test 2" };
        await _creator.AddAsync(newNotification1);
        await _creator.AddAsync(newNotification2);

        var criteria = new NotificationCriteriaDTO { ClassId = cls.Id };
        
        // Act
        var result = await _querier.GetByAsync(criteria);

        // Assert
        Assert.Equal(2, result.Results.Count());
    }

    [Fact]
    public async Task GetByAsync_WithNonMatchingCriteria_ReturnsEmpty()
    {
        // Arrange
        var criteria = new NotificationCriteriaDTO { ClassId = "non-existent" };
        
        // Act
        var result = await _querier.GetByAsync(criteria);

        // Assert
        Assert.Empty(result.Results);
    }
}

