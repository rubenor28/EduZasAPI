using Application.DAOs;
using Application.DTOs;
using Application.DTOs.ClassResources;
using Application.DTOs.Common;
using Application.DTOs.Notifications;
using Application.DTOs.UserNotifications;
using Application.DTOs.Users;
using Application.Services;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.ClassResource;

/// <summary>
/// Caso de uso para actualizar la asociación de un recurso con una clase.
/// </summary>
public sealed class UpdateClassResourceUseCase(
    IUpdaterAsync<ClassResourceDomain, ClassResourceDTO> updater,
    IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> reader,
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> classReader,
    IQuerierAsync<UserDomain, UserCriteriaDTO> userQuierier,
    ITaskScheduler scheduler,
    IConfiguration configuration,
    IBusinessValidationService<ClassResourceDTO>? validator = null
)
    : UpdateUseCase<ClassResourceIdDTO, ClassResourceDTO, ClassResourceDomain>(
        updater,
        reader,
        validator
    )
{
    private readonly IReaderAsync<string, ClassDomain> _classReader = classReader;
    private readonly IQuerierAsync<UserDomain, UserCriteriaDTO> _userQuierier = userQuierier;
    private readonly IReaderAsync<ulong, UserDomain> _userReader = userReader;

    private readonly ITaskScheduler _scheduler = scheduler;
    private readonly IConfiguration _configuration = configuration;

    /// <inheritdoc/>
    protected override ClassResourceIdDTO GetId(ClassResourceDTO dto) =>
        new() { ClassId = dto.ClassId, ResourceId = dto.ResourceId };

    protected override async Task ExtraTaskAsync(
        UserActionDTO<ClassResourceDTO> newEntity,
        ClassResourceDomain original,
        ClassResourceDomain createdEntity
    )
    {
        if (newEntity.Data.Hidden != original.Hidden && newEntity.Data.Hidden)
            return;

        var user = await _userReader.GetAsync(newEntity.Executor.Id);
        var @class = await _classReader.GetAsync(newEntity.Data.ClassId);

        string professorName = $"{user!.FirstName} {user.FatherLastname}";
        string subject = $"Nuevo recurso disponible en {@class!.ClassName}";
        string title = $"{professorName} ha agregado un nuevo recurso en {@class.ClassName}";

        var users = new List<UserDomain>();
        var page = 1;
        PaginatedQuery<UserDomain, UserCriteriaDTO> usersSearch;

        do
        {
            usersSearch = await _userQuierier.GetByAsync(
                new() { EnrolledInClass = @class.Id, Page = page }
            );
            users.AddRange(usersSearch.Results);
            page++;
        } while (usersSearch.TotalPages >= page);

        var userIds = users.Select(u => u.Id).ToList();

        var notificationTask = _scheduler.CreateNotification(
            new() { ClassId = @class.Id, Title = title },
            userIds
        );

        var emailMessages = new List<EmailMessage>();

        var frontendUrl = _configuration.GetValue<string>("ServerOptions:FrontEndUrl");

        if (string.IsNullOrEmpty(frontendUrl))
        {
            await notificationTask;

            Console.WriteLine(
                "[AddClassResourceUseCase] No se definió ServerOptions:FrontEndUrl en la configuración, no se enviarán los emails"
            );

            return;
        }

        var htmlBody = EmailTemplates.GetGenericTemplate(
            title: "Nuevo Recurso Disponible",
            mainMessage: "Se ha publicado un nuevo material académico que podría interesarte.",
            detailLabel: "Clase",
            detailValue: @class.ClassName,
            actionText: "Ver Recurso",
            actionUrl: $"{frontendUrl.TrimEnd('/')}/student/classes/resource/{@class.Id}/{createdEntity.ResourceId}"
        );

        foreach (var u in users)
        {
            emailMessages.Add(
                new EmailMessage
                {
                    To = [u.Email],
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true,
                }
            );
        }

        var emailTask = _scheduler.BulkSendEmail(emailMessages);

        await Task.WhenAll(notificationTask, emailTask);
    }
}
