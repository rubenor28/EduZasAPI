using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassStudents;

/// <summary>
/// Represents the use case for toggling the visibility of a class for a student.
/// </summary>
public class ToggleClassVisibilityUseCase(
    IUpdaterAsync<StudentClassRelationDTO, StudentClassRelationDTO> updater,
    IReaderAsync<string, ClassDomain> classReader,
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> relationReader
) : IUseCaseAsync<ToggleClassVisibilityDTO, Unit>
{
    /// <summary>
    /// Executes the use case to toggle the hidden state of a class for a user.
    /// </summary>
    /// <param name="value">The DTO containing the class ID and executor information.</param>
    /// <returns>A result indicating success or a use case error.</returns>
    public async Task<Result<Unit, UseCaseError>> ExecuteAsync(ToggleClassVisibilityDTO value)
    {

        var errors = new List<FieldErrorDTO>();

        (await classReader.GetAsync(value.ClassId)).IfNone(() =>
            errors.Add(new() { Field = "classId", Message = "No se encontró la clase" })
        );

        (await userReader.GetAsync(value.UserId)).IfNone(() =>
            errors.Add(new() { Field = "userId", Message = "No se encontró el usuario" })
        );

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var authorized = IsAuthorized(value);
        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var relationSearch = await relationReader.GetAsync(
            new() { ClassId = value.ClassId, UserId = value.UserId }
        );

        if (relationSearch.IsNone)
            return UseCaseErrors.NotFound();

        var relation = relationSearch.Unwrap();
        relation.Hidden = !relation.Hidden;

        await updater.UpdateAsync(relation);
        return Unit.Value;
    }

    public static bool IsAuthorized(ToggleClassVisibilityDTO value) =>
        value.Executor.Role switch
        {
            UserType.ADMIN => true,
            // Solo si es el propio estudiante de la relacion podra ocultar una clase
            _ => value.UserId == value.Executor.Id,
        };
}
