using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Domain.ValueObjects;

namespace Application.UseCases.ClassStudents;

/// <summary>
/// Caso de uso para añadir un estudiante a una clase.
/// </summary>
public class AddClassStudentUseCase(
    ICreatorAsync<ClassStudentDomain, UserClassRelationId> creator,
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> classReader,
    IReaderAsync<UserClassRelationId, ClassStudentDomain> studentReader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> professorReader
) : AddUseCase<UserClassRelationId, ClassStudentDomain>(creator)
{
    private readonly IReaderAsync<ulong, UserDomain> _userReader = userReader;
    private readonly IReaderAsync<string, ClassDomain> _classReader = classReader;
    private readonly IReaderAsync<UserClassRelationId, ClassStudentDomain> _studentReader =
        studentReader;
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _professorReader =
        professorReader;

    /// <inheritdoc/>
    protected async override Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<UserClassRelationId> value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(
                value.Executor.Id,
                value.Data.ClassId
            ),
            UserType.STUDENT => value.Data.UserId == value.Executor.Id,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var errors = new List<FieldErrorDTO>();

        (await _classReader.GetAsync(value.Data.ClassId)).IfNull(() =>
            errors.Add(new() { Field = "classId", Message = "Clase no encontrada" })
        );

        (await _userReader.GetAsync(value.Data.UserId)).IfNull(() =>
            errors.Add(new() { Field = "userId", Message = "Usuario no encontrado" })
        );

        var professorSearch = await _professorReader.GetAsync(value.Data);
        professorSearch.IfSome(_ =>
            errors.Add(
                new() { Field = "userId", Message = "El usuario ya es profesor de la clase" }
            )
        );

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var relationSearch = await _studentReader.GetAsync(value.Data);

        if (relationSearch is not null)
            return UseCaseErrors.Conflict("El recurso ya existe");

        return Unit.Value;
    }

    private async Task<bool> IsProfessorAuthorized(ulong professorId, string classId)
    {
        var professor = await _professorReader.GetAsync(
            new() { ClassId = classId, UserId = professorId }
        );

        return professor is not null && professor.IsOwner;
    }
}
