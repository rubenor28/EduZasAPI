using Application.DTOs.Notifications;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using EntityFramework.Extensions;
using EntityFramework.InterfaceAdapters.Mappers.Common;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Application.DAOs.Notifications;

public sealed class NotificationSummaryEFQuerier(
    EduZasDotnetContext ctx,
    IEFProjector<
        NotificationPerUser,
        NotificationSummaryDTO,
        NotificationSummaryCriteriaDTO
    > projector,
    int maxPageSize
)
    : EFQuerier<NotificationSummaryDTO, NotificationSummaryCriteriaDTO, NotificationPerUser>(
        ctx,
        projector,
        maxPageSize
    )
{
    public override IQueryable<NotificationPerUser> BuildQuery(
        NotificationSummaryCriteriaDTO criteria
    ) =>
        _dbSet
            .AsNoTracking()
            .Where(un => un.UserId == criteria.UserId)
            .WhereOptional(criteria.Readed, readed => un => un.Readed == readed)
            .OrderBy(un => un.CreatedAt);
}
