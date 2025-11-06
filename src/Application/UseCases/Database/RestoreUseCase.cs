using Application.DTOs.Common;
using Application.DTOs.Database;
using Application.Services;
using Application.UseCases.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Database;

/// <summary>
/// Caso de uso para restaurar la base de datos desde un respaldo.
/// </summary>
/// <param name="importer">Servicio para importar la base de datos.</param>
public sealed class RestoreUseCase(IDatabaseImporter importer)
    : IUseCaseAsync<RestoreRequestDTO, Unit>
{
    private readonly IDatabaseImporter _importer = importer;

    private static bool IsAuthorized(Executor executor) =>
        executor.Role switch
        {
            UserType.ADMIN => true,
            _ => false,
        };

    /// <summary>
    /// Ejecuta el caso de uso.
    /// </summary>
    /// <param name="request">La solicitud con el stream del respaldo.</param>
    /// <returns>Un resultado que indica éxito o un error.</returns>
    public async Task<Result<Unit, UseCaseErrorImpl>> ExecuteAsync(RestoreRequestDTO request)
    {
        if (!IsAuthorized(request.Executor))
            return UseCaseError.Unauthorized();

        await _importer.RestoreAsync(request.InputStream);

        return Unit.Value;
    }
}
