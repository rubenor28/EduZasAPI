using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.DTOs.Notifications;
using Application.UseCases.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Notifications;

public sealed class QueryNotificationSummaryUseCase(
    IQuerierAsync<NotificationSummaryDTO, NotificationSummaryCriteriaDTO> querier
)
    : IUseCaseAsync<
        NotificationSummaryCriteriaDTO,
        PaginatedQuery<NotificationSummaryDTO, NotificationSummaryCriteriaDTO>
    >
{
    public async Task<
        Result<PaginatedQuery<NotificationSummaryDTO, NotificationSummaryCriteriaDTO>, UseCaseError>
    > ExecuteAsync(UserActionDTO<NotificationSummaryCriteriaDTO> criteria)
    {
        var authorized = criteria.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => criteria.Executor.Id == criteria.Data.UserId,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var results = await querier.GetByAsync(criteria.Data);
        return results;
    }
}

public sealed class HasUnreadNotificationUseCase(
    IQuerierAsync<NotificationSummaryDTO, NotificationSummaryCriteriaDTO> querier
) : IUseCaseAsync<ulong, bool>
{
    public async Task<Result<bool, UseCaseError>> ExecuteAsync(UserActionDTO<ulong> request)
    {
        var authorized = request.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => request.Executor.Id == request.Data,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return await querier.AnyAsync(new() { UserId = request.Data, Readed = false });
    }
}
