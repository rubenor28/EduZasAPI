using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
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
    IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> relationReader
) : UpdateUseCase<string, ClassUpdateDTO, ClassDomain>(updater, reader, validator)
{
    /// <summary>
    /// Realiza validaciones adicionales para la actualización de la clase.
    /// </summary>
    /// <param name="value">DTO con los datos de la clase a actualizar.</param>
    /// <returns>Un resultado que indica si la validación fue exitosa o no.</returns>
    protected async override Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        ClassUpdateDTO value
    )
    {
        if (value.Executor.Role != UserType.ADMIN)
        {
            var result = await relationReader.GetAsync(
                new() { UserId = value.Executor.Id, ClassId = value.Id }
            );

            if (result.IsNone || !result.Unwrap().IsOwner)
                return Result.Err(UseCaseError.Unauthorized());
        }

        var classToUpdate = await _reader.GetAsync(value.Id);
        if (classToUpdate.IsNone)
            return UseCaseError.NotFound();

        return Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value);
    }
}
