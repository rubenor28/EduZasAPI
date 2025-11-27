using Application.DAOs;
using Application.DTOs.ClassResources;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassResource;

public sealed class AddClassResourceUseCase(
    ICreatorAsync<ClassResourceDomain, NewClassResourceDTO> creator,
    IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> reader,
    IReaderAsync<string, ClassDomain> classReader,
    IReaderAsync<Guid, ResourceDomain> resourceReader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> professorReader,
    IBusinessValidationService<NewClassResourceDTO>? validator = null
) : AddUseCase<NewClassResourceDTO, ClassResourceDomain>(creator, validator)
{
    private readonly IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> _reader = reader;
    private readonly IReaderAsync<string, ClassDomain> _classReader = classReader;
    private readonly IReaderAsync<Guid, ResourceDomain> _resourceReader = resourceReader;
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _professorReader =
        professorReader;

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        NewClassResourceDTO value
    )
    {
        var classResSearch = await _reader.GetAsync(
            new() { ClassId = value.ClassId, ResourceId = value.ResourceId }
        );

        if (classResSearch.IsSome)
            return UseCaseErrors.AlreadyExists();

        var errors = new List<FieldErrorDTO>();
        (await _classReader.GetAsync(value.ClassId)).IfNone(() =>
            errors.Add(new() { Field = "classId", Message = "No se encontró la clase" })
        );

        var resourceSearch = await _resourceReader.GetAsync(value.ResourceId);
        resourceSearch.IfNone(() =>
            errors.Add(new() { Field = "resourceId", Message = "No se encontró el recurso" })
        );

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var resource = resourceSearch.Unwrap();
        var professor = await _professorReader.GetAsync(
            new() { UserId = resource.ProfessorId, ClassId = value.ClassId }
        );

        if (professor.IsNone)
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
}
