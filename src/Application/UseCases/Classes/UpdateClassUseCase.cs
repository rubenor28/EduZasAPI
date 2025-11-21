using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Classes;

/// <summary>
/// Caso de uso para actualizar una clase.
/// </summary>
public class UpdateClassUseCase(
    IUpdaterAsync<ClassDomain, ClassUpdateDTO> updater,
    IReaderAsync<string, ClassDomain> reader,
    IBusinessValidationService<ClassUpdateDTO> validator,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> professorReader
) : UpdateUseCase<string, ClassUpdateDTO, ClassDomain>(updater, reader, validator)
{
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _professorReader =
        professorReader;

    /// <summary>
    /// Realiza validaciones adicionales para la actualización de la clase.
    /// </summary>
    /// <param name="value">DTO con los datos de la clase a actualizar.</param>
    /// <returns>Un resultado que indica si la validación fue exitosa o no.</returns>
    protected async override Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        ClassUpdateDTO value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(value.Executor.Id, value.Id),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var classToUpdate = await _reader.GetAsync(value.Id);
        if (classToUpdate.IsNone)
            return UseCaseErrors.NotFound();

        return Result<Unit, UseCaseError>.Ok(Unit.Value);
    }

    private async Task<bool> IsProfessorAuthorized(ulong professorId, string classId)
    {
        var professorSearch = await _professorReader.GetAsync(
            new() { UserId = professorId, ClassId = classId }
        );

        return professorSearch.IsSome && professorSearch.Unwrap().IsOwner;
    }

    protected override string GetId(ClassUpdateDTO dto) => dto.Id;
}
