using Application.DAOs;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Classes;

/// <summary>
/// Caso de uso para eliminar clases.
/// </summary>
/// <remarks>
/// Requiere permisos de Admin o ser el Profesor propietario.
/// </remarks>

public class DeleteClassUseCase(
    IDeleterAsync<string, ClassDomain> deleter,
    IReaderAsync<string, ClassDomain> reader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> relationReader
) : DeleteUseCase<string, ClassDomain>(deleter, reader)
{
    /// <summary>
    /// Valida asíncronamente la autorización del ejecutor y la existencia de la clase antes de eliminarla.
    /// </summary>
    /// <param name="value">Datos de la acción de eliminación.</param>
    /// <param name="record">Entidad de la clase original.</param>
    /// <returns>Resultado exitoso o error de caso de uso.</returns>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<string> value,
        ClassDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(value.Executor.Id, value.Data),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var classSearch = await _reader.GetAsync(value.Data);

        if (classSearch is null)
            return UseCaseErrors.NotFound();

        return Unit.Value;
    }

    /// <summary>Determina si un usuario con rol profesor puede realizar la eliminación</summary>
    /// <param name="professorId">ID del profesor que ejecuta la acción</param>
    /// <param name="classId">ID de la clase</param>
    private async Task<bool> IsProfessorAuthorized(ulong professorId, string classId)
    {
        var professorSearch = await relationReader.GetAsync(
            new() { UserId = professorId, ClassId = classId }
        );

        return professorSearch is not null && professorSearch.IsOwner;
    }
}
