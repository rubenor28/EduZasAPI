using Application.DAOs;
using Application.DTOs.ClassProfessors;
using Application.Services.Validators;
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
    IQuerierAsync<ClassProfessorDomain, ClassProfessorCriteriaDTO> querier,
    IBusinessValidationService<ClassProfessorUpdateDTO>? validator = null
)
    : UpdateUseCase<UserClassRelationId, ClassProfessorUpdateDTO, ClassProfessorDomain>(
        updater,
        reader,
        validator
    )
{
    private readonly IQuerierAsync<ClassProfessorDomain, ClassProfessorCriteriaDTO> _querier =
        querier;

    /// <summary>
    /// Valida asíncronamente la autorización y asegura que no se deje la clase sin al menos un propietario.
    /// </summary>
    /// <param name="current">Datos de la actualización.</param>
    /// <param name="prev">Entidad de relación profesor-clase original.</param>
    /// <returns>Resultado exitoso o error de caso de uso.</returns>
    protected async override Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ClassProfessorUpdateDTO> current,
        ClassProfessorDomain prev
    )
    {
        var authorized = current.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(
                current.Executor.Id,
                current.Data.ClassId
            ),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        if (prev.IsOwner && !current.Data.IsOwner)
        {
            var c = new ClassProfessorCriteriaDTO { IsOwner = true, ClassId = current.Data.ClassId };
            var ownersCount = await _querier.CountAsync(c);
            if (ownersCount <= 1)
                return UseCaseErrors.Conflict("Debe haber al menos un dueño de la clase");
        }

        return Unit.Value;
    }

    /// <summary>
    /// Obtiene el identificador compuesto de la relación profesor-clase desde el DTO.
    /// </summary>
    /// <param name="dto">DTO de actualización.</param>
    /// <returns>Identificador compuesto de la relación.</returns>
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
