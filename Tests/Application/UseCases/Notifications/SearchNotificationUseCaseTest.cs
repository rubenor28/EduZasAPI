using Domain.ValueObjects;
using Application.DTOs.Notifications;
using Application.UseCases.Notifications;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Application.UseCases.Notifications;

public class SearchNotificationUseCaseTest : BaseTest
{
    private readonly AddNotificationUseCase _addNotificationUseCase;
    private readonly SearchNotificationUseCase _useCase;

    public SearchNotificationUseCaseTest()
    {
        _addNotificationUseCase = _sp.GetRequiredService<AddNotificationUseCase>();
        _useCase = _sp.GetRequiredService<SearchNotificationUseCase>();
    }

    private async Task<(Domain.Entities.ClassDomain, Domain.Entities.UserDomain)> SeedStudentAndClass()
    {
        var student = await SeedUser();
        var cls = await SeedClass(student.Id);
        await SeedClassStudent(cls.Id, student.Id);
        return (cls, student);
    }

    [Fact]
    public async Task SearchUserNotification_ByStudent_ReturnsNotification()
    {
        // Arrange
        var admin = await SeedUser(UserType.ADMIN);
        var (cls, user) = await SeedStudentAndClass();

        var notification = new NewNotificationDTO { ClassId = cls.Id, Title = "Test" };

        await _addNotificationUseCase.ExecuteAsync(
            new() { Data = notification, Executor = AsExecutor(admin) }
        );

        var criteria = new NotificationCriteriaDTO { PageSize = 10, UserId = user.Id };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        // Assert
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
        // Arrange
        var admin = await SeedUser(UserType.ADMIN);
        var criteria = new NotificationCriteriaDTO { PageSize = 10, UserId = 1 };

        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );
        
        // Assert
        Assert.True(result.IsOk);
        var search = result.Unwrap();
        Assert.NotNull(search);
        Assert.Equal(search.Criteria, criteria);
        Assert.False(search.Results.Any());
    }

    [Fact]
    public async Task SearchUserNotification_ByClass_ReturnsNotification()
    {
        // Arrange
        var admin = await SeedUser(UserType.ADMIN);
        var (cls, _) = await SeedStudentAndClass();

        var notification = new NewNotificationDTO { ClassId = cls.Id, Title = "Test" };

        await _addNotificationUseCase.ExecuteAsync(
            new() { Data = notification, Executor = AsExecutor(admin) }
        );

        var criteria = new NotificationCriteriaDTO { PageSize = 10, ClassId = cls.Id };
        
        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        // Assert
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
        // Arrange
        var admin = await SeedUser(UserType.ADMIN);
        var criteria = new NotificationCriteriaDTO { PageSize = 10, ClassId = "Test-Class" };
        
        // Act
        var result = await _useCase.ExecuteAsync(
            new() { Data = criteria, Executor = AsExecutor(admin) }
        );

        // Assert
        Assert.True(result.IsOk);
        var search = result.Unwrap();
        Assert.NotNull(search);
        Assert.Equal(search.Criteria, criteria);
        Assert.False(search.Results.Any());
    }
}
