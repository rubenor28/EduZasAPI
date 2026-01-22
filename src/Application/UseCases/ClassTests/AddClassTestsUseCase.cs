using Application.DAOs;
using Application.DTOs.ClassTests;
using Application.DTOs.Users;
using Application.Services;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Domain.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace Application.UseCases.ClassTests;

/// <summary>
/// Caso de uso para asociar una evaluación a una clase.
/// </summary>
public sealed class AddClassTestUseCase(
    ICreatorAsync<ClassTestDomain, ClassTestIdDTO> creator,
    IReaderAsync<Guid, TestDomain> testReader,
    IReaderAsync<string, ClassDomain> classReader,
    IReaderAsync<ClassTestIdDTO, ClassTestDomain> classTestReader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> professorReader,
    IReaderAsync<ulong, UserDomain> userReader,
    IQuerierAsync<UserDomain, UserCriteriaDTO> userQuierier,
    ITaskScheduler scheduler,
    IConfiguration cfg,
    IBusinessValidationService<ClassTestIdDTO>? validator = null
) : AddUseCase<ClassTestIdDTO, ClassTestDomain>(creator, validator)
{
    private readonly IReaderAsync<Guid, TestDomain> _testReader = testReader;
    private readonly IReaderAsync<string, ClassDomain> _classReader = classReader;
    private readonly IReaderAsync<ClassTestIdDTO, ClassTestDomain> _classTestReader =
        classTestReader;

    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _professorReader =
        professorReader;

    private readonly IQuerierAsync<UserDomain, UserCriteriaDTO> _userQuierier = userQuierier;
    private readonly IReaderAsync<ulong, UserDomain> _userReader = userReader;
    private readonly ITaskScheduler _scheduler = scheduler;
    private readonly IConfiguration _cfg = cfg;

    private TestDomain _test = null!;

    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ClassTestIdDTO> value
    )
    {
        List<FieldErrorDTO> errors = [];

        (await _classReader.GetAsync(value.Data.ClassId)).IfNull(() =>
            errors.Add(new() { Field = "classId", Message = "No se encontró la clase" })
        );

        _test = (await _testReader.GetAsync(value.Data.TestId))!;

        _test.IfNull(() =>
            errors.Add(new() { Field = "testId", Message = "No se encontró el test" })
        );

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => await TestOwnerIsClassProfessor(
                _test!.ProfessorId,
                value.Data.ClassId
            ),
            UserType.PROFESSOR => await IsProfessorAuthorized(
                value.Executor.Id,
                value.Data.ClassId,
                _test
            ),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var classTestSearch = await _classTestReader.GetAsync(
            new() { ClassId = value.Data.ClassId, TestId = value.Data.TestId }
        );

        if (classTestSearch is not null)
            return UseCaseErrors.Conflict("El recurso ya existe");

        return Unit.Value;
    }

    private async Task<bool> TestOwnerIsClassProfessor(ulong testOwner, string classId)
    {
        var professor = await _professorReader.GetAsync(
            new() { ClassId = classId, UserId = testOwner }
        );

        return professor is not null;
    }

    private async Task<bool> IsProfessorAuthorized(
        ulong professorId,
        string classId,
        TestDomain test
    )
    {
        var professorSearch = await _professorReader.GetAsync(
            new() { ClassId = classId, UserId = professorId }
        );

        return professorSearch is not null && test.ProfessorId == professorId;
    }

    protected override async Task ExtraTaskAsync(
        UserActionDTO<ClassTestIdDTO> dto,
        ClassTestDomain created
    )
    {
        List<Task> actionsToDo = [];

        // NOTIFICACIONES EN SISTEMA
        var user = await _userReader.GetAsync(dto.Executor.Id);
        var @class = await _classReader.GetAsync(dto.Data.ClassId);

        string professorName = $"{user!.FirstName} {user.FatherLastname}";
        string subject = $"Nueva evaluación disponible en {@class!.ClassName}";
        string title = $"{professorName} ha agregado una nueva evaluación en {@class.ClassName}";

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

        actionsToDo.Add(notificationTask);

        // NOTIFICACIONES POR EMAIL
        var emailMessages = new List<EmailMessage>();

        var frontendUrl = _cfg.GetValue<string>("ServerOptions:FrontEndUrl");
        if (frontendUrl is not null)
        {
            var htmlBody = EmailTemplates.GetGenericTemplate(
                title: "Nueva Evaluación Disponible",
                mainMessage: "Se ha publicado una evaluación.",
                detailLabel: "Clase",
                detailValue: @class.ClassName,
                actionText: "Ver Evaluación",
                actionUrl: $"{frontendUrl.TrimEnd('/')}/student/classes/test/{@class.Id}/{dto.Data.TestId}"
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
            actionsToDo.Add(emailTask);
        }
        else
            Console.WriteLine(
                "[AddClassTestUseCase] No se configuró FrontEndUrl en la configuración, no se enviarán los emails"
            );

        // CIERRE DE INTENTOS SI SE TERMINA EL TIEMPO
        if (_test.TimeLimitMinutes is not null)
        {
            var startTime = created.CreatedAt;
            var timeLimit = TimeSpan.FromMinutes(_test.TimeLimitMinutes.Value);
            var deadline = startTime.Add(timeLimit);

            var markTriesAsFinishedTask = _scheduler.ScheduleMarkAnswersAsFinished(
                dto.Data.ClassId,
                dto.Data.TestId,
                deadline
            );

            actionsToDo.Add(markTriesAsFinishedTask);
        }

        // Encolar todas las tareas
        await Task.WhenAll(actionsToDo);
    }
}
