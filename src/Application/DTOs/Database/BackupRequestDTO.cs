using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Database;

/// <summary>
/// DTO para solicitar la creación de un respaldo.
/// </summary>
public sealed class BackupRequestDTO
{
    /// <summary>
    /// Entidad que ejecuta la acción.
    /// </summary>
    public required Executor Executor { get; init; }
}
