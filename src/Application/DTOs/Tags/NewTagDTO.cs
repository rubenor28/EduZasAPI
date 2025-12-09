namespace Application.DTOs.Tags;

/// <summary>
/// Datos para crear una nueva etiqueta.
/// </summary>
public sealed record NewTagDTO
{
    /// <summary>Texto de la etiqueta.</summary>
    public required string Text { get; init; }
}
