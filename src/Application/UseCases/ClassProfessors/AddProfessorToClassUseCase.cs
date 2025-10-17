using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassProfessors;

/// <summary>
/// Implementa el caso de uso para añadir un usuario a una clase, validando
/// que el usuario posea los permisos requeridos (Professor o Administrador).
/// Utiliza el modelo de programación asincrónica (TAP) para la validación de dependencias.
/// </summary>
public class AddProfessorToClassUseCase(
    ICreatorAsync<ProfessorClassRelationDTO, ProfessorClassRelationDTO> creator,
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> classReader,
    IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> professorRelationReader
) : AddUseCase<ProfessorClassRelationDTO, ProfessorClassRelationDTO>(creator)
{
    /// <summary>
    /// Lista de roles de usuario permitidos para ser añadidos a la clase.
    /// Solo se admiten <see cref="UserType.PROFESSOR"/> y <see cref="UserType.ADMIN"/>.
    /// </summary>
    protected readonly IEnumerable<UserType> _admitedRoles = [UserType.PROFESSOR, UserType.ADMIN];

    /// <summary>
    /// Realiza validaciones asincrónicas antes de proceder con la adición de la relación.
    /// Las validaciones incluyen la existencia de la clase, la existencia del usuario,
    /// y la verificación de que el usuario tenga un rol permitido.
    /// Las búsquedas de usuario y clase se ejecutan de forma concurrente.
    /// </summary>
    /// <param name="value">El DTO que contiene el ID de usuario y el ID de clase.</param>
    /// <returns>
    /// Un <see cref="Result{TSuccess, TFailure}"/> que indica si la validación fue exitosa
    /// (<see cref="Unit.Value"/>) o si contiene una lista de errores de campo (<see cref="FieldErrorDTO"/>).
    /// </returns>
    protected async override Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        ProfessorClassRelationDTO value
    )
    {
        var usrSearchTask = userReader.GetAsync(value.Id.UserId);
        var classSearchTask = classReader.GetAsync(value.Id.ClassId);
        var errors = new List<FieldErrorDTO>();

        if ((await classSearchTask).IsNone)
            errors.Add(new() { Field = "classId", Message = "Clase no encontrada" });

        var usrSearch = await usrSearchTask;
        usrSearch.IfNone(() =>
            errors.Add(new() { Field = "userId", Message = "Usuario no encontrado" })
        );

        if (errors.Count > 0)
            return UseCaseError.Input(errors);

        if (usrSearch.IsSome && !_admitedRoles.Contains(usrSearch.Unwrap().Role))
            return UseCaseError.UnauthorizedError();

        var relationSearch = await professorRelationReader.GetAsync(value.Id);
        if (relationSearch.IsSome)
            return UseCaseError.Input(
                [
                    new()
                    {
                        Field = "userId, classId",
                        Message = "El usuario ya es professor de esta clase",
                    },
                ]
            );

        return Unit.Value;
    }
}
