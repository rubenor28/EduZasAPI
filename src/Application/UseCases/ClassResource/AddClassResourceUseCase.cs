using Application.DAOs;
using Application.DTOs;
using Application.DTOs.ClassResources;
using Application.DTOs.Common;
using Application.DTOs.Users;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Domain.ValueObjects;

namespace Application.UseCases.ClassResource;

public sealed class AddClassResourceUseCase(
    ICreatorAsync<ClassResourceDomain, ClassResourceDTO> creator,
    IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> reader,
    IReaderAsync<string, ClassDomain> classReader,
    IReaderAsync<Guid, ResourceDomain> resourceReader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> professorReader,
    IEmailSender emailSender,
    IReaderAsync<ulong, UserDomain> userReader,
    IQuerierAsync<UserDomain, UserCriteriaDTO> userQuierier,
    IBusinessValidationService<ClassResourceDTO>? validator = null
) : AddUseCase<ClassResourceDTO, ClassResourceDomain>(creator, validator)
{
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> _reader = reader;
    private readonly IReaderAsync<string, ClassDomain> _classReader = classReader;
    private readonly IReaderAsync<Guid, ResourceDomain> _resourceReader = resourceReader;
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _professorReader =
        professorReader;
    private readonly IQuerierAsync<UserDomain, UserCriteriaDTO> _userQuierier = userQuierier;
    private readonly IReaderAsync<ulong, UserDomain> _userReader = userReader;

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

    /*
     protected override async Task ExtraTaskAsync(
         UserActionDTO<ClassResourceDTO> newEntity,
         ClassResourceDomain createdEntity
     )
     {
        var user = await _userReader.GetAsync(newEntity.Executor.Id) ?? throw new ArgumentException("No se pudo encontrar el usuario");
         var search = await _userQuierier.GetByAsync(new() { EnrolledInClass = newEntity.Data.ClassId });
         var email = new EmailMessage
         {
             Subject = $"Nuevo recurso compartido por {user.FatherLastname} {user.FirstName}",
             To = [.. search.Results.Select(s => s.Email)],
             Body =
                 $"<h1>EduZas</h1><h2>{user.FatherLastname} {user.FirstName} ha publicado un nuevo recurso</h2><a href=\"\">Abrir recurso</a>",
         };
     }
     */
}
