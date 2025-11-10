using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassStudents;

/// <summary>
/// Implementa el caso de uso para añadir un usuario a una clase.
/// Utiliza el modelo de programación asincrónica (TAP) para la validación de dependencias.
/// </summary>
public class UnenrollClassUseCase(
    IDeleterAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> deleter,
    IReaderAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> reader,
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> classReader,
    IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> professorRelationReader
)
    : DeleteUseCase<ClassUserRelationIdDTO, UnenrollClassDTO, StudentClassRelationDTO>(
        deleter,
        reader
    )
{
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
        UnenrollClassDTO value
    )
    {
        var errors = new List<FieldErrorDTO>();

        var classSearch = await classReader.GetAsync(value.Id.ClassId);
        if (classSearch.IsNone)
            errors.Add(new() { Field = "classId", Message = "Clase no encontrada" });

        var usrSearch = await userReader.GetAsync(value.Id.UserId);
        usrSearch.IfNone(() =>
            errors.Add(new() { Field = "userId", Message = "Usuario no encontrado" })
        );

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var studentRelationSearch = await _reader.GetAsync(value.Id);

        if (studentRelationSearch.IsNone)
            return UseCaseErrors.NotFound();

        var studentRelation = studentRelationSearch.Unwrap();

        var authorized = value.Executor.Role switch
        {
            // Admin puede eliminar de una clase a cualquiera
            UserType.ADMIN => true,
            // El alumno solo puede eliminarse a sí mismo
            UserType.STUDENT => studentRelation.Id.UserId == value.Executor.Id,
            // El profesor solo puede eliminar si tiene los permisos adecuados
            UserType.PROFESSOR => await IsAuthorizedProfessor(value.Executor.Id, value.Id.ClassId),
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
        var professorRelationSearch = await professorRelationReader.GetAsync(
            new() { UserId = professorId, ClassId = classId }
        );

        // El usuario es profesor, pero no de esta clase
        if (professorRelationSearch.IsNone)
            return false;

        return professorRelationSearch.Unwrap().IsOwner;
    }
}
