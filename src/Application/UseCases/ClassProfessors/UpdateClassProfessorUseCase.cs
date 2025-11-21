using Application.DAOs;
using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassProfessors;

public sealed class UpdateClassProfessorUseCase(
    IUpdaterAsync<ClassProfessorDomain, ClassProfessorUpdateDTO> updater,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> reader,
    IBusinessValidationService<ClassProfessorUpdateDTO>? validator = null
)
    : UpdateUseCase<UserClassRelationId, ClassProfessorUpdateDTO, ClassProfessorDomain>(
        updater,
        reader,
        validator
    )
{
    protected override Result<Unit, UseCaseError> ExtraValidation(ClassProfessorUpdateDTO value)
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.UserId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    protected override UserClassRelationId GetId(ClassProfessorUpdateDTO dto) =>
        new() { UserId = dto.UserId, ClassId = dto.ClassId };
}
