using Application.DAOs;
using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassProfessors;

public sealed class DeleteClassProfessorUseCase(
    IDeleterAsync<UserClassRelationId, ClassProfessorDomain> deleter,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> reader,
    IBusinessValidationService<DeleteClassProfessorDTO>? validator = null
)
    : DeleteUseCase<UserClassRelationId, DeleteClassProfessorDTO, ClassProfessorDomain>(
        deleter,
        reader,
        validator
    )
{
    private async Task<bool> IsProfessorAuthorized(ulong professorId, string classId)
    {
        var professor = await _reader.GetAsync(new() { UserId = professorId, ClassId = classId });
        return professor.IsSome && professor.Unwrap().IsOwner;
    }

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        DeleteClassProfessorDTO value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(value.Executor.Id, value.Id.ClassId),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }
}
