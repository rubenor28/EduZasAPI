namespace MinimalAPI.Application.DTOs.Common;

/// <summary>
/// Representa una consulta de búsqueda basada en texto,
/// incluyendo el valor a buscar y el tipo de coincidencia a aplicar.
/// </summary>
public sealed record StringQueryMAPI
{
    /// <summary>
    /// Texto a utilizar en la búsqueda.
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Tipo de búsqueda a aplicar (por ejemplo, "equals" o "like").
    /// </summary>
    public required string SearchType { get; set; }
}
