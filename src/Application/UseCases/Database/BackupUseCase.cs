using Application.Services;
using Application.UseCases.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Database;

/// <summary>
/// Caso de uso para exportar una copia de seguridad de la base de datos.
/// </summary>
public sealed class BackupUseCase(IDatabaseExporter exporter) : IUseCaseAsync<Unit, Stream>
{
    private readonly IDatabaseExporter _exporter = exporter;

    private static bool IsAuthorized(Executor executor) =>
        executor.Role switch
        {
            UserType.ADMIN => true,
            _ => false,
        };

    /// <summary>
    /// Ejecuta la exportación de la base de datos si el usuario tiene permisos de administrador.
    /// </summary>
    /// <param name="request">Solicitud de ejecución (sin datos adicionales).</param>
    /// <returns>Stream con el contenido del backup o error de autorización.</returns>
    public async Task<Result<Stream, UseCaseError>> ExecuteAsync(UserActionDTO<Unit> request)
    {
        if (!IsAuthorized(request.Executor))
            return UseCaseErrors.Unauthorized();

        var stream = await _exporter.ExportBackupAsync();

        return stream;
    }
}
