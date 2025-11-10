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
public class EnrollClassUseCase(
    ICreatorAsync<StudentClassRelationDTO, EnrollClassDTO> creator,
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> classReader,
    IReaderAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> studentReader,
    IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> professorReader
) : AddUseCase<EnrollClassDTO, StudentClassRelationDTO>(creator)
{
    private readonly IReaderAsync<ulong, UserDomain> _userReader = userReader;
    private readonly IReaderAsync<string, ClassDomain> _classReader = classReader;
    private readonly IReaderAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> _studentReader =
        studentReader;
    private readonly IReaderAsync<
        ClassUserRelationIdDTO,
        ProfessorClassRelationDTO
    > _professorReader = professorReader;

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
    protected async override Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        EnrollClassDTO value
    )
    {
        var errors = new List<FieldErrorDTO>();

        (await _classReader.GetAsync(value.ClassId)).IfNone(() =>
            errors.Add(new() { Field = "classId", Message = "Clase no encontrada" })
        );

        (await _userReader.GetAsync(value.UserId)).IfNone(() =>
            errors.Add(new() { Field = "userId", Message = "Usuario no encontrado" })
        );

        var professorId = new ClassUserRelationIdDTO
        {
            UserId = value.UserId,
            ClassId = value.ClassId,
        };

        var professorSearch = await _professorReader.GetAsync(professorId);
        professorSearch.IfSome(_ =>
            errors.Add(
                new() { Field = "userId", Message = "El usuario ya es profesor de la clase" }
            )
        );

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var profRel = professorSearch.Unwrap();

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => profRel.Id.UserId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var relationSearch = await _studentReader.GetAsync(
            new() { ClassId = value.ClassId, UserId = value.UserId }
        );

        if (relationSearch.IsSome)
            return UseCaseErrors.AlreadyExists();

        return Unit.Value;
    }
}
