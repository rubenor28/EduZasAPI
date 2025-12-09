using Application.DAOs;
using Application.DTOs.Notifications;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Notifications;

/// <summary>
/// Caso de uso para buscar notificaciones.
/// </summary>
public class SearchNotificationUseCase(
    IQuerierAsync<NotificationDomain, NotificationCriteriaDTO> querier
) : QueryUseCase<NotificationCriteriaDTO, NotificationDomain>(querier);
