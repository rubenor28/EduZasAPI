using Application.DTOs.Notifications;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Notifications;

namespace MinimalAPI.Presentation.Mappers;

public static class NotificationMAPIMapper
{
    public static NotificationCriteriaMAPI FromDomain(this NotificationCriteriaDTO s) =>
        new()
        {
            Page = s.Page,
            UserId = s.UserId.ToNullable(),
            ClassId = s.ClassId.ToNullable(),
        };
}
