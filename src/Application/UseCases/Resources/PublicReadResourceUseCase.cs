using Application.DAOs;
using Application.DTOs;
using Application.DTOs.ClassResources;
using Application.DTOs.Common;
using Application.DTOs.Resources;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Resources;

/// <summary>
/// Caso de uso para leer un recurso por su ID.
/// </summary>
public sealed class PublicReadResourceUseCase(
    IReaderAsync<ReadResourceDTO, ResourceDomain> reader,
    IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> classResourceReader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> professorReader,
    IReaderAsync<UserClassRelationId, ClassStudentDomain> studentReader,
    IBusinessValidationService<ReadResourceDTO>? validator = null
) : ReadUseCase<ReadResourceDTO, ResourceDomain>(reader, validator)
{
    private readonly IReaderAsync<ClassResourceIdDTO, ClassResourceDomain> _classResourceReader =
        classResourceReader;
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _professorReader =
        professorReader;
    private readonly IReaderAsync<UserClassRelationId, ClassStudentDomain> _studentReader =
        studentReader;

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ReadResourceDTO> value
    )
    {
        var resource = await _classResourceReader.GetAsync(
            new() { ClassId = value.Data.ClassId, ResourceId = value.Data.ResourceId }
        );

        if (resource is null)
            return UseCaseErrors.NotFound();

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(
                new() { ClassId = value.Data.ClassId, UserId = value.Executor.Id }
            ),
            UserType.STUDENT => !resource.Hidden
                && await IsStudentAuthorized(
                    new() { ClassId = value.Data.ClassId, UserId = value.Executor.Id }
                ),
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    private async Task<bool> IsProfessorAuthorized(UserClassRelationId executorClassRelation)
    {
        var professor = await _professorReader.GetAsync(executorClassRelation);
        if (professor is not null)
            return true;

        var student = await _studentReader.GetAsync(executorClassRelation);
        if (student is not null)
            return true;

        return false;
    }

    private async Task<bool> IsStudentAuthorized(UserClassRelationId executorClassRelation)
    {
        var student = await _studentReader.GetAsync(executorClassRelation);
        return student is not null;
    }
}
