using Application.DAOs;
using Application.DTOs;
using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassProfessors;

/// <summary>
/// Caso de uso para actualizar la relación de un profesor con una clase.
/// </summary>
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
    /// <inheritdoc/>
    protected override Result<Unit, UseCaseError> ExtraValidation(UserActionDTO<ClassProfessorUpdateDTO> value, ClassProfessorDomain record)
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.Data.UserId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <inheritdoc/>
    protected override UserClassRelationId GetId(ClassProfessorUpdateDTO dto) =>
        new() { UserId = dto.UserId, ClassId = dto.ClassId };
}
