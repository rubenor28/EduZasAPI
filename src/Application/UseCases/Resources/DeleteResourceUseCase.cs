using Application.DAOs;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Resources;

/// <summary>
/// Caso de uso para eliminar un recurso.
/// </summary>
public sealed class DeleteResourceUseCase(
    IDeleterAsync<Guid, ResourceDomain> deleter,
    IReaderAsync<Guid, ResourceDomain> reader
) : DeleteUseCase<Guid, ResourceDomain>(deleter, reader, null)
{
    /// <summary>
    /// Valida asíncronamente que el recurso exista y que el ejecutor esté autorizado para eliminarlo.
    /// </summary>
    /// <param name="value">Datos de la acción de eliminación.</param>
    /// <param name="record">Entidad del recurso original.</param>
    /// <returns>Resultado exitoso o error de caso de uso.</returns>
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<Guid> value,
        ResourceDomain record
    )
    {
        var resource = await _reader.GetAsync(value.Data);
        if (resource is null)
            return UseCaseErrors.NotFound();

        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => resource.ProfessorId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }
}
