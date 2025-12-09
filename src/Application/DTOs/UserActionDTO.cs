using Application.DTOs.Common;

namespace Application.DTOs;

/// <summary>
/// Envuelve una acción realizada por un usuario.
/// </summary>
/// <typeparam name="T">Tipo de datos de la acción.</typeparam>
public sealed record UserActionDTO<T>
{
    /// <summary>Datos de la acción.</summary>
    public required T Data { get; init; }

    /// <summary>Información del ejecutor.</summary>
    public required Executor Executor { get; init; }
}
