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
    IReaderAsync<ulong, UserDomain> userReader,
    IReaderAsync<string, ClassDomain> classReader,
    IDeleterAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> deleter,
    IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> professorRelationReader,
    IReaderAsync<ClassUserRelationIdDTO, StudentClassRelationDTO> studentRelationReader
) : DeleteUseCase<ClassUserRelationIdDTO, UnenrollClassDTO, StudentClassRelationDTO>(deleter)
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
    protected async override Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
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

        var studentRelationSearch = await studentRelationReader.GetAsync(value.Id);

        switch (studentRelationSearch)
        {
            // Si no se encuentra la relacion, se marca como error de la entrada
            case { IsNone: true }:
            {
                return UseCaseError.NotFound();
            }

            /**
             * Validar que el ejecutor sea, o administrador, o profesor dueño de la clase.
             * Se hace uso del early return. Un break indica un caso valido, y en caso de
             * return envia un error o una excepcion según corresponda
             */
            case { IsSome: true }:
            {
                var relation = studentRelationSearch.Unwrap();

                if (relation.Id.UserId == value.Executor.Id)
                    break;

                if (value.Executor.Role == UserType.ADMIN)
                    break;

                if (value.Executor.Role == UserType.STUDENT)
                    return UseCaseError.UnauthorizedError();

                var professorRelationSearch = await professorRelationReader.GetAsync(
                    new() { UserId = value.Executor.Id, ClassId = value.Id.ClassId }
                );

                // Caso el argumento diga que el ejecutor es usuario pero los registros
                // digan lo contrario
                if (professorRelationSearch.IsNone)
                    throw new InvalidOperationException(
                        "El executor dice ser profesor pero no lo es"
                    );

                var professorRelation = professorRelationSearch.Unwrap();

                if (professorRelation.IsOwner)
                    break;

                return UseCaseError.UnauthorizedError();
            }
        }

        if (errors.Count > 0)
            return UseCaseError.Input(errors);

        return Unit.Value;
    }
}
