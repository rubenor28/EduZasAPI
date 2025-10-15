using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Domain.Users;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Classes;

/// <summary>
/// Represents the use case for toggling the visibility of a class for a student.
/// </summary>
public class ToggleClassVisibilityUseCase : IUseCaseAsync<ToggleClassVisibilityDTO, Result<Unit, UseCaseErrorImpl>>
{
    protected readonly IReaderAsync<string, ClassDomain> _classReader;
    protected readonly IReaderAsync<ulong, UserDomain> _userReader;
    protected readonly IUpdaterAsync<StudentClassRelationDTO, StudentClassRelationDTO> _updater;
    protected readonly IReaderAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> _relationReader;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToggleClassVisibilityUseCase"/> class.
    /// </summary>
    /// <param name="classReader">The reader for retrieving class domain objects.</param>
    /// <param name="updater">The updater for student-class relations.</param>
    /// <param name="relationReader">The reader for retrieving student-class relations.</param>
    public ToggleClassVisibilityUseCase(
        IReaderAsync<string, ClassDomain> classReader,
        IReaderAsync<ulong, UserDomain> userReader,
        IUpdaterAsync<StudentClassRelationDTO, StudentClassRelationDTO> updater,
        IReaderAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> relationReader)
    {
        _updater = updater;
        _relationReader = relationReader;
        _classReader = classReader;
        _userReader = userReader;
    }

    /// <summary>
    /// Executes the use case to toggle the hidden state of a class for a user.
    /// </summary>
    /// <param name="value">The DTO containing the class ID and executor information.</param>
    /// <returns>A result indicating success or a use case error.</returns>
    public async Task<Result<Unit, UseCaseErrorImpl>> ExecuteAsync(ToggleClassVisibilityDTO value)
    {
        var errors = new List<FieldErrorDTO>();

        var classSearch = await _classReader.GetAsync(value.ClassId);
        if (classSearch.IsNone)
            errors.Add(new FieldErrorDTO { Field = "classId", Message = "No se encontró la clase" });

        var userSearch = await _userReader.GetAsync(value.Executor.Id);
        if (userSearch.IsNone)
            errors.Add(new FieldErrorDTO { Field = "userId", Message = "No se encontró el usuario" });

        if (errors.Any())
            return Result.Err(UseCaseError.Input(errors));

        var relationSearch = await _relationReader.GetAsync(new ClassUserRelationIdDTO
        {
            ClassId = value.ClassId,
            UserId = value.Executor.Id
        });

        if (relationSearch.IsNone)
            return Result.Err(UseCaseError.NotFound());

        var relation = relationSearch.Unwrap();
        relation.Hidden = !relation.Hidden;

        await _updater.UpdateAsync(relation);
        return Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value);
    }
}
