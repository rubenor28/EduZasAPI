using Application.DAOs;
using Application.DTOs.Notifications;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.Notifications;

public sealed class AddNotificationUseCase(ICreatorAsync<NewNotificationDTO, NotificationDomain> creator) : AddUseCase<NewNotificationDTO, NotificationDomain>(creator) { }
