using Application.DAOs;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.UseCases.Notifications;

using IUserNotificationReader = IReaderAsync<UserNotificationIdDTO, UserNotificationDomain>;
using IUserNotificationUpdater = IUpdaterAsync<UserNotificationDomain, UserNotificationDomain>;

public sealed class MarkNotificationAsReadUseCase(
    IUserNotificationReader reader,
    IUserNotificationUpdater updater
) : IUseCaseAsync<UserNotificationIdDTO, UserNotificationDomain>
{
    public readonly IUserNotificationReader _reader = reader;
    public readonly IUserNotificationUpdater _updater = updater;

    public async Task<Result<UserNotificationDomain, UseCaseErrorImpl>> ExecuteAsync(
        UserNotificationIdDTO id
    )
    {
        var notificationSearch = await _reader.GetAsync(id);

        if (notificationSearch.IsNone)
            return UseCaseError.NotFound();

        var updated = await _updater.UpdateAsync(new() { Id = id, Readed = true });

        return updated;
    }
}
