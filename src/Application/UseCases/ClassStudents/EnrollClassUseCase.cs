using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.UseCases.ClassStudents;

/// <summary>
/// Implementa el caso de uso para añadir un usuario a una clase.
/// Utiliza el modelo de programación asincrónica (TAP) para la validación de dependencias.
/// </summary>
public class EnrollClassUseCase(
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> classReader,
    IReaderAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> studentReader,
    IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> professorReader,
    ICreatorAsync<StudentClassRelationDTO, StudentClassRelationDTO> creator
) : AddUseCase<StudentClassRelationDTO, StudentClassRelationDTO>(creator)
{
    protected override StudentClassRelationDTO PostValidationFormat(
        StudentClassRelationDTO value
    ) => new() { Id = value.Id, Hidden = true };

    /// <summary>
    /// Realiza validaciones asincrónicas antes de proceder con la adición de la relación.
    /// Las validaciones incluyen la existencia de la clase y la existencia del usuario.
    /// Las búsquedas de usuario y clase se ejecutan de forma concurrente.
    /// </summary>
    /// <param name="value">El DTO que contiene el ID de usuario y el ID de clase.</param>
    /// <returns>
    /// Un <see cref="Result{TSuccess, TFailure}"/> que indica si la validación fue exitosa
    /// (<see cref="Unit.Value"/>) o si contiene una lista de errores de campo (<see cref="FieldErrorDTO"/>).
    /// </returns>
    protected async override Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        StudentClassRelationDTO value
    )
    {
        var relationSearch = await studentReader.GetAsync(value.Id);

        if (relationSearch.IsSome)
            return UseCaseError.Input(
                [
                    new()
                    {
                        Field = "userId, classId",
                        Message = "El usuario ya se encuentra inscrito a esta clase",
                    },
                ]
            );

        var errors = new List<FieldErrorDTO>();

        var classSearch = await classReader.GetAsync(value.Id.ClassId);
        if (classSearch.IsNone)
        {
            errors.Add(new() { Field = "classId", Message = "Clase no encontrada" });
        }

        var usrSearch = await userReader.GetAsync(value.Id.UserId);
        usrSearch.IfNone(() =>
            errors.Add(new() { Field = "userId", Message = "Usuario no encontrado" })
        );

        var userIsProfessorSearch = await professorReader.GetAsync(value.Id);

        userIsProfessorSearch.IfSome(_ =>
            errors.Add(
                new() { Field = "userId", Message = "El usuario ya es profesor de la clase" }
            )
        );

        if (errors.Count > 0)
            return UseCaseError.Input(errors);

        return Unit.Value;
    }
}
