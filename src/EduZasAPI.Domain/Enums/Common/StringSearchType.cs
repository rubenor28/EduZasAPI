namespace EduZasAPI.Domain.Common;

/// <summary>
/// Enumera los tipos de búsqueda disponibles para cadenas de texto.
/// </summary>
public enum StringSearchType
{
    /// <summary>
    /// Búsqueda por igualdad exacta (equals).
    /// </summary>
    EQ,

    /// <summary>
    /// Búsqueda por coincidencia parcial (like).
    /// </summary>
    LIKE,
}
