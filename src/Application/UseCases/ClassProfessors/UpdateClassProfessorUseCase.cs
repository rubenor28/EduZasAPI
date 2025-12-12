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
    protected async override Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ClassProfessorUpdateDTO> value,
        ClassProfessorDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(
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

    /// <inheritdoc/>
    protected override UserClassRelationId GetId(ClassProfessorUpdateDTO dto) =>
        new() { UserId = dto.UserId, ClassId = dto.ClassId };

    ///<summary>
    /// Valida que un profesor que intenta modificar los profesores en una clase
    /// pertenezca a la clase y sea dueño
    ///</summary>
    private async Task<bool> IsProfessorAuthorized(ulong professorId, string classId)
    {
        var classProfessor = await _reader.GetAsync(
            new() { UserId = professorId, ClassId = classId }
        );

        return classProfessor is not null && classProfessor.IsOwner;
    }
}
