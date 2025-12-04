using Application.DAOs;
using Application.DTOs;
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
    IBusinessValidationService<UserClassRelationId>? validator = null
) : DeleteUseCase<UserClassRelationId, ClassProfessorDomain>(deleter, reader, validator)
{
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<UserClassRelationId> value,
        ClassProfessorDomain professor
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => professor is not null && professor.IsOwner,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }
}
