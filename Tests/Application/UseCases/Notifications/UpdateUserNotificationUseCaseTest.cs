using Application.DAOs;
using Application.DTOs.Common;
using Application.DTOs.UserNotifications;
using Application.UseCases.UserNotifications;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Tests.Application.UseCases.Notifications;

public class UpdateUserNotificationUseCaseTest : BaseTest
{
    private readonly UpdateUserNotificationUseCase _useCase;
    private readonly ICreatorAsync<UserNotificationDomain, NewUserNotificationDTO> _userNotificationCreator;
    private readonly IReaderAsync<UserNotificationIdDTO, UserNotificationDomain> _userNotificationReader;


    public UpdateUserNotificationUseCaseTest()
    {
        _useCase = _sp.GetRequiredService<UpdateUserNotificationUseCase>();
        _userNotificationCreator = _sp.GetRequiredService<ICreatorAsync<UserNotificationDomain, NewUserNotificationDTO>>();
        _userNotificationReader = _sp.GetRequiredService<IReaderAsync<UserNotificationIdDTO, UserNotificationDomain>>();
    }

    private async Task<UserNotificationDomain> SeedUserNotification(ulong userId)
    {
        var cls = await SeedClass(userId);
        var notification = await SeedNotification(cls.Id);
        return await _userNotificationCreator.AddAsync(new() { NotificationId = notification.Id, UserId = userId });
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotificationExistsAndIsNotRead_ShouldUpdateReadedToTrue()
    {
        // Arrange
        var user = await SeedUser();
        var userNotification = await SeedUserNotification(user.Id);
        var inputDto = new UserNotificationUpdateDTO
        {
            UserId = user.Id,
            NotificationId = userNotification.Id.NotificationId,
            Readed = true,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = inputDto, Executor = AsExecutor(user) }
        );

        // Assert
        Assert.True(result.IsOk);

        var updatedNotification = await _userNotificationReader.GetAsync(userNotification.Id);
        Assert.NotNull(updatedNotification);
        Assert.True(updatedNotification.Readed);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotificationDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var user = await SeedUser();
        ulong nonExistentNotificationId = 100;
        var inputDto = new UserNotificationUpdateDTO
        {
            UserId = user.Id,
            NotificationId = nonExistentNotificationId,
            Readed = true,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = inputDto, Executor = AsExecutor(user) }
        );

        // Assert
        Assert.True(result.IsErr);
        Assert.IsType<NotFoundError>(result.UnwrapErr());
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotificationIsAlreadyRead_ShouldReturnSuccess()
    {
        // Arrange
        var user = await SeedUser();
        var userNotification = await SeedUserNotification(user.Id);
        var inputDto = new UserNotificationUpdateDTO
        {
            UserId = user.Id,
            NotificationId = userNotification.Id.NotificationId,
            Readed = true,
        };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = inputDto, Executor = AsExecutor(user) }
        );
        
        // Assert
        Assert.True(result.IsOk);

        var updatedNotification = await _userNotificationReader.GetAsync(userNotification.Id);
        Assert.NotNull(updatedNotification);
        Assert.True(updatedNotification.Readed);
    }
}

