using Application.DAOs;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassProfessors;

/// <summary>
/// Caso de uso para eliminar un profesor de una clase.
/// </summary>
public sealed class DeleteClassProfessorUseCase(
    IDeleterAsync<UserClassRelationId, ClassProfessorDomain> deleter,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> reader,
    IBusinessValidationService<UserClassRelationId>? validator = null
) : DeleteUseCase<UserClassRelationId, ClassProfessorDomain>(deleter, reader, validator)
{
    /// <inheritdoc/>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<UserClassRelationId> value,
        ClassProfessorDomain professor
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(
                value.Data.UserId,
                value.Executor.Id,
                value.Data.ClassId
            ),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    private async Task<bool> IsProfessorAuthorized(
        ulong professorId,
        ulong executorId,
        string classId
    )
    {
        var professor = await _reader.GetAsync(new() { ClassId = classId, UserId = executorId });

        return professor is not null && (professor.IsOwner || professorId == executorId);
    }
}
