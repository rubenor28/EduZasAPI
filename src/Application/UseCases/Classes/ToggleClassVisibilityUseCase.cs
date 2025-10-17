using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.UseCases.Classes;

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
    public async Task<Result<Unit, UseCaseErrorImpl>> ExecuteAsync(ToggleClassVisibilityDTO value)
    {
        var errors = new List<FieldErrorDTO>();

        var classSearch = await classReader.GetAsync(value.ClassId);
        if (classSearch.IsNone)
            errors.Add(new() { Field = "classId", Message = "No se encontró la clase" });

        var userSearch = await userReader.GetAsync(value.Executor.Id);
        if (userSearch.IsNone)
            errors.Add(new() { Field = "userId", Message = "No se encontró el usuario" });

        if (errors.Count > 0)
            return UseCaseError.Input(errors);

        var relationSearch = await relationReader.GetAsync(
            new() { ClassId = value.ClassId, UserId = value.Executor.Id }
        );

        if (relationSearch.IsNone)
            return UseCaseError.NotFound();

        var relation = relationSearch.Unwrap();
        relation.Hidden = !relation.Hidden;

        await updater.UpdateAsync(relation);
        return Unit.Value;
    }
}
