using Application.DAOs;
using Application.DTOs;
using Application.DTOs.ClassResources;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.Services;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Domain.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.ClassResource;

/// <summary>
/// Caso de uso para asociar un recurso a una clase.
/// </summary>
public sealed class AddClassResourceUseCase(
    ICreatorAsync<ClassResourceDomain, ClassResourceDTO> creator,
    IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> reader,
    IReaderAsync<string, ClassDomain> classReader,
    IReaderAsync<Guid, ResourceDomain> resourceReader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> professorReader,
    IReaderAsync<ulong, UserDomain> userReader,
    IQuerierAsync<UserDomain, UserCriteriaDTO> userQuierier,
    ITaskScheduler scheduler,
    IConfiguration configuration,
    IBusinessValidationService<ClassResourceDTO>? validator = null
) : AddUseCase<ClassResourceDTO, ClassResourceDomain>(creator, validator)
{
    private readonly IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> _reader = reader;
    private readonly IReaderAsync<string, ClassDomain> _classReader = classReader;
    private readonly IReaderAsync<Guid, ResourceDomain> _resourceReader = resourceReader;
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _professorReader =
        professorReader;

    private readonly IQuerierAsync<UserDomain, UserCriteriaDTO> _userQuierier = userQuierier;
    private readonly IReaderAsync<ulong, UserDomain> _userReader = userReader;

    private readonly ITaskScheduler _scheduler = scheduler;
    private readonly IConfiguration _configuration = configuration;

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ClassResourceDTO> value
    )
    {
        var classResSearch = await _reader.GetAsync(
            new() { ClassId = value.Data.ClassId, ResourceId = value.Data.ResourceId }
        );

        if (classResSearch is not null)
            return UseCaseErrors.Conflict("El recurso ya existe");

        var errors = new List<FieldErrorDTO>();
        (await _classReader.GetAsync(value.Data.ClassId)).IfNull(() =>
            errors.Add(new() { Field = "classId", Message = "No se encontró la clase" })
        );

        var resourceSearch = await _resourceReader.GetAsync(value.Data.ResourceId);
        resourceSearch.IfNull(() =>
            errors.Add(new() { Field = "resourceId", Message = "No se encontró el recurso" })
        );

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var resource = resourceSearch!;
        var professor = await _professorReader.GetAsync(
            new() { UserId = resource.ProfessorId, ClassId = value.Data.ClassId }
        );

        if (professor is null)
            return UseCaseErrors.Unauthorized();

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => resource.ProfessorId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    protected override async Task ExtraTaskAsync(
        UserActionDTO<ClassResourceDTO> newEntity,
        ClassResourceDomain createdEntity
    )
    {
        if (newEntity.Data.Hidden)
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
