using Application.DTOs.Common;
using Application.DTOs.Notifications;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Notifications;

namespace MinimalAPI.Presentation.Mappers;

public sealed class NotificationCriteriaMAPIMapper
    : IBidirectionalMapper<NotificationCriteriaMAPI, NotificationCriteriaDTO>,
        IMapper<NotificationCriteriaDTO, NotificationCriteriaMAPI>
{
    public NotificationCriteriaMAPI Map(NotificationCriteriaDTO input) => MapFrom(input);

    public NotificationCriteriaDTO Map(NotificationCriteriaMAPI input) =>
        new()
        {
            Page = input.Page,
            UserId = input.UserId.ToOptional(),
            ClassId = input.ClassId.ToOptional(),
        };

    public NotificationCriteriaMAPI MapFrom(NotificationCriteriaDTO input) =>
        new()
        {
            Page = input.Page,
            UserId = input.UserId.ToNullable(),
            ClassId = input.ClassId.ToNullable(),
        };
}

public sealed class UserNotificationCriteriaMAPIMapper
    : IMapper<int, ulong, NotificationCriteriaDTO>
{
    public NotificationCriteriaDTO Map(int page, ulong userId) =>
        new() { Page = page, UserId = userId };
}

public sealed class PublicNotificationMAPIMapper : IMapper<NotificationDomain, PublicNotificationMAPI>
{
    public PublicNotificationMAPI Map(NotificationDomain input) => new() { Title = input.Title };
}

public sealed class NotificationSearchMAPIMapper(
    IMapper<NotificationDomain, PublicNotificationMAPI> mapper,
    IMapper<NotificationCriteriaDTO, NotificationCriteriaMAPI> cMapper
)
    : IMapper<
        PaginatedQuery<NotificationDomain, NotificationCriteriaDTO>,
        PaginatedQuery<PublicNotificationMAPI, NotificationCriteriaMAPI>
    >
{
    public PaginatedQuery<PublicNotificationMAPI, NotificationCriteriaMAPI> Map(
        PaginatedQuery<NotificationDomain, NotificationCriteriaDTO> input
    ) =>
        new()
        {
            Page = input.Page,
            TotalPages = input.TotalPages,
            Criteria = cMapper.Map(input.Criteria),
            Results = input.Results.Select(mapper.Map),
        };
}
