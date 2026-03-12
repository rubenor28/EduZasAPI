using Application.DAOs;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Domain.ValueObjects;

namespace Application.UseCases.ClassStudents;

/// <summary>
/// Caso de uso para eliminar un estudiante de una clase.
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
    /// Valida asíncronamente que la clase y el usuario existan, y que el ejecutor tenga permisos para eliminar al estudiante de la clase.
    /// </summary>
    /// <param name="value">Datos de la acción de eliminación.</param>
    /// <param name="record">Entidad de relación estudiante-clase original.</param>
    /// <returns>Resultado exitoso o error de caso de uso.</returns>
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
            UserType.STUDENT => student.UserId == value.Executor.Id,
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
    /// Verifica si un profesor tiene permisos para eliminar estudiantes.
    /// </summary>
    private async Task<bool> IsAuthorizedProfessor(ulong professorId, string classId)
    {
        var professor = await _professorReader.GetAsync(
            new() { UserId = professorId, ClassId = classId }
        );

        return professor is not null && professor.IsOwner;
    }
}
