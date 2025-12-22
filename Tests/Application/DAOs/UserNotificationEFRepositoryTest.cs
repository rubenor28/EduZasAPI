using Application.DAOs;
using Application.DTOs.UserNotifications;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.DAOs;

public class UserNotificationEFRepositoryTest : BaseTest
{
    private readonly ICreatorAsync<UserNotificationDomain, NewUserNotificationDTO> _creator;
    private readonly IReaderAsync<UserNotificationIdDTO, UserNotificationDomain> _reader;
    private readonly IUpdaterAsync<UserNotificationDomain, UserNotificationUpdateDTO> _updater;

    public UserNotificationEFRepositoryTest()
    {
        _creator = _sp.GetRequiredService<ICreatorAsync<UserNotificationDomain, NewUserNotificationDTO>>();
        _reader = _sp.GetRequiredService<IReaderAsync<UserNotificationIdDTO, UserNotificationDomain>>();
        _updater = _sp.GetRequiredService<IUpdaterAsync<UserNotificationDomain, UserNotificationUpdateDTO>>();
    }

    [Fact]
    public async Task AddUserNotification_ReturnsRelation()
    {
        // Arrange
        var user = await SeedUser();
        var cls = await SeedClass(user.Id);
        var notification = await SeedNotification(cls.Id);
        var newUserNotification = new NewUserNotificationDTO { UserId = user.Id, NotificationId = notification.Id };

        // Act
        var created = await _creator.AddAsync(newUserNotification);

        // Assert
        Assert.NotNull(created);
        Assert.Equal(newUserNotification.UserId, created.Id.UserId);
        Assert.Equal(newUserNotification.NotificationId, created.Id.NotificationId);
        Assert.False(created.Readed);
    }

    [Fact]
    public async Task GetAsync_WhenRelationExists_ReturnsRelation()
    {
        // Arrange
        var user = await SeedUser();
        var cls = await SeedClass(user.Id);
        var notification = await SeedNotification(cls.Id);
        var newUserNotification = new NewUserNotificationDTO { UserId = user.Id, NotificationId = notification.Id };
        var created = await _creator.AddAsync(newUserNotification);

        // Act
        var found = await _reader.GetAsync(created.Id);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(created.Id.UserId, found.Id.UserId);
        Assert.Equal(created.Id.NotificationId, found.Id.NotificationId);
    }

    [Fact]
    public async Task GetAsync_WhenRelationDoesNotExist_ReturnsNone()
    {
        // Arrange
        var id = new UserNotificationIdDTO { UserId = 123, NotificationId = 456 };
        
        // Act
        var found = await _reader.GetAsync(id);
        
        // Assert
        Assert.Null(found);
    }

    [Fact]
    public async Task UpdateUserNotification_ReturnsUpdatedRelation()
    {
        // Arrange
        var user = await SeedUser();
        var cls = await SeedClass(user.Id);
        var notification = await SeedNotification(cls.Id);
        var created = await _creator.AddAsync(new NewUserNotificationDTO { UserId = user.Id, NotificationId = notification.Id });

        var toUpdate = new UserNotificationUpdateDTO
        {
            UserId = created.Id.UserId,
            NotificationId = created.Id.NotificationId,
            Readed = true,
        };

        // Act
        var updated = await _updater.UpdateAsync(toUpdate);

        // Assert
        Assert.NotNull(updated);
        Assert.True(updated.Readed);
    }

    [Fact]
    public async Task UpdateAsync_WhenRelationDoesNotExist_ThrowsException()
    {
        // Arrange
        var toUpdate = new UserNotificationUpdateDTO
        {
            UserId = 123,
            NotificationId = 456,
            Readed = true,
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<System.ArgumentException>(() => _updater.UpdateAsync(toUpdate));
    }
}

