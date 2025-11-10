using Application.DTOs.Common;
using Application.DTOs.Notifications;
using Domain.Entities;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Notifications;

namespace MinimalAPI.Presentation.Mappers;

public class NotificationMAPIMapper
    : IMapper<NotificationCriteriaMAPI, NotificationCriteriaDTO>,
        IMapper<NotificationCriteriaDTO, NotificationCriteriaMAPI>,
        IMapper<int, ulong, NotificationCriteriaDTO>,
        IMapper<NotificationDomain, PublicNotificationMAPI>,
        IMapper<
            PaginatedQuery<NotificationDomain, NotificationCriteriaDTO>,
            PaginatedQuery<PublicNotificationMAPI, NotificationCriteriaMAPI>
        >
{
    public NotificationCriteriaDTO Map(int page, ulong userId) =>
        new() { Page = page, UserId = userId };

    public NotificationCriteriaDTO Map(NotificationCriteriaMAPI s) =>
        new()
        {
            Page = s.Page,
            UserId = s.UserId.ToOptional(),
            ClassId = s.ClassId.ToOptional(),
        };

    public NotificationCriteriaMAPI Map(NotificationCriteriaDTO s) =>
        new()
        {
            Page = s.Page,
            UserId = s.UserId.ToNullable(),
            ClassId = s.ClassId.ToNullable(),
        };

    public PublicNotificationMAPI Map(NotificationDomain input) => new() { Title = input.Title };

    public PaginatedQuery<PublicNotificationMAPI, NotificationCriteriaMAPI> Map(
        PaginatedQuery<NotificationDomain, NotificationCriteriaDTO> input
    ) =>
        new()
        {
            Page = input.Page,
            TotalPages = input.TotalPages,
            Criteria = Map(input.Criteria),
            Results = input.Results.Select(Map),
        };
}
