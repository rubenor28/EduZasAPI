using Application.DTOs;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Database;

public sealed class BackupUseCase(IDatabaseExporter exporter) : IUseCaseAsync<Unit, Stream>
{
    private readonly IDatabaseExporter _exporter = exporter;

    private static bool IsAuthorized(Executor executor) =>
        executor.Role switch
        {
            UserType.ADMIN => true,
            _ => false,
        };

    public async Task<Result<Stream, UseCaseError>> ExecuteAsync(UserActionDTO<Unit> request)
    {
        if (!IsAuthorized(request.Executor))
            return UseCaseErrors.Unauthorized();

        var stream = await _exporter.ExportBackupAsync();

        return stream;
    }
}
