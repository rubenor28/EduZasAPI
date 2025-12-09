namespace Domain.Enums;

/// <summary>
/// Define los modos de comparación para consultas que involucran cadenas de texto.
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
