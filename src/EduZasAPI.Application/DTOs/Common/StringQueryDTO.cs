using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Representa una consulta de texto con un tipo específico de búsqueda a aplicar.
/// </summary>
/// <remarks>
/// Esta estructura encapsula una cadena de texto junto con el tipo de búsqueda
/// que debe ser aplicado sobre ella. Es útil para realizar búsquedas flexibles
/// donde el mismo texto puede requerir diferentes tratamientos (búsqueda exacta,
/// búsqueda parcial, etc.).
/// </remarks>
public struct StringQueryDTO
{
    /// <summary>
    /// Obtiene o establece el texto de la consulta a buscar.
    /// </summary>
    /// <value>Cadena de texto a consultar. Campo obligatorio.</value>
    public required string Text { get; set; }

    /// <summary>
    /// Obtiene o establece el tipo de búsqueda a aplicar sobre el texto.
    /// </summary>
    /// <value>
    /// Tipo de búsqueda definido por <see cref="StringSearchType"/>. Campo obligatorio.
    /// </value>
    public required StringSearchType SearchType { get; set; }
}
