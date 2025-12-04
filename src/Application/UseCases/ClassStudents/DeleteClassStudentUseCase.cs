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
/// Implementa el caso de uso para añadir un usuario a una clase.
/// Utiliza el modelo de programación asincrónica (TAP) para la validación de dependencias.
/// </summary>
public class DeleteClassStudentUseCase(
    IDeleterAsync<UserClassRelationId, ClassStudentDomain> deleter,
    IReaderAsync<UserClassRelationId, ClassStudentDomain> reader,
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> classReader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> professorReader
) : DeleteUseCase<UserClassRelationId, ClassStudentDomain>(deleter, reader)
{
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _professorReader =
        professorReader;

    /// <summary>
    /// Realiza validaciones asincrónicas antes de proceder con la adición de la relación.
    /// Las validaciones incluyen la existencia de la clase, la existencia del usuario y
    /// la existencia de la relación.
    /// </summary>
    /// <param name="value">El DTO que contiene el ID de usuario y el ID de clase.</param>
    /// <returns>
    /// Un <see cref="Result{TSuccess, TFailure}"/> que indica si la validación fue exitosa
    /// (<see cref="Unit.Value"/>) o si contiene una lista de errores de campo (<see cref="FieldErrorDTO"/>).
    /// </returns>
    protected async override Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<UserClassRelationId> value,
        ClassStudentDomain record
    )
    {
        var errors = new List<FieldErrorDTO>();

        var classSearch = await classReader.GetAsync(value.Data.ClassId);
        if (classSearch is null)
            errors.Add(new() { Field = "classId", Message = "Clase no encontrada" });

        var usrSearch = await userReader.GetAsync(value.Data.UserId);
        usrSearch.IfNull(() =>
            errors.Add(new() { Field = "userId", Message = "Usuario no encontrado" })
        );

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var student = await _reader.GetAsync(value.Data);

        if (student is null)
            return UseCaseErrors.NotFound();

        var authorized = value.Executor.Role switch
        {
            // Admin puede eliminar de una clase a cualquiera
            UserType.ADMIN => true,
            // El alumno solo puede eliminarse a sí mismo
            UserType.STUDENT => student.Id.UserId == value.Executor.Id,
            // El profesor solo puede eliminar si tiene los permisos adecuados
            UserType.PROFESSOR => await IsAuthorizedProfessor(
                value.Executor.Id,
                value.Data.ClassId
            ),
            _ => throw new NotImplementedException("UseCase not prepared for this user type"),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    /// <summary>
    /// Funcion que determina si un profesor puede desincribir a un alumno
    /// </summary>
    private async Task<bool> IsAuthorizedProfessor(ulong professorId, string classId)
    {
        var professor = await _professorReader.GetAsync(
            new() { UserId = professorId, ClassId = classId }
        );

        return professor is not null && professor.IsOwner;
    }
}
