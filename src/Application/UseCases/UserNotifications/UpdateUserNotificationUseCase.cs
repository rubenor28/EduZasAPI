using Application.DAOs;
using Application.DTOs.UserNotifications;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

namespace Application.UseCases.UserNotifications;

public sealed class UpdateUserNotificationUseCase(
    IUpdaterAsync<UserNotificationDomain, UserNotificationUpdateDTO> updater,
    IReaderAsync<UserNotificationIdDTO, UserNotificationDomain> reader,
    IBusinessValidationService<UserNotificationUpdateDTO>? validator = null
)
    : UpdateUseCase<UserNotificationIdDTO, UserNotificationUpdateDTO, UserNotificationDomain>(
        updater,
        reader,
        validator
    );
