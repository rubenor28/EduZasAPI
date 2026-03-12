using Application.DAOs;
using Application.DTOs.ClassStudents;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassStudents;

/// <summary>
/// Caso de uso para actualizar la relación de un estudiante con una clase.
/// </summary>
public sealed class UpdateClassStudentUseCase(
    IUpdaterAsync<ClassStudentDomain, ClassStudentUpdateDTO> updater,
    IReaderAsync<UserClassRelationId, ClassStudentDomain> reader,
    IBusinessValidationService<ClassStudentUpdateDTO>? validator = null
)
    : UpdateUseCase<UserClassRelationId, ClassStudentUpdateDTO, ClassStudentDomain>(
        updater,
        reader,
        validator
    )
{
    /// <summary>
    /// Valida que el ejecutor sea un administrador o el propio estudiante antes de actualizar la relación.
    /// </summary>
    /// <param name="value">Datos de la actualización.</param>
    /// <param name="record">Entidad de relación estudiante-clase original.</param>
    /// <returns>Resultado exitoso o error de autorización.</returns>
    protected override Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<ClassStudentUpdateDTO> value,
        ClassStudentDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => value.Executor.Id == value.Data.UserId,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <summary>
    /// Obtiene el identificador compuesto de la relación estudiante-clase desde el DTO.
    /// </summary>
    /// <param name="dto">DTO de actualización.</param>
    /// <returns>Identificador compuesto de la relación.</returns>
    protected override UserClassRelationId GetId(ClassStudentUpdateDTO dto) =>
        new() { UserId = dto.UserId, ClassId = dto.ClassId };
}
