namespace EduZasAPI.Domain.Enums.Common;

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

/// <summary>
/// Proporciona métodos de extensión para el enum <see cref="StringSearchType"/>.
/// </summary>
public class StringSearchTypeExtensions
{
    /// <summary>
    /// Convierte un valor del enum <see cref="StringSearchType"/> a su representación en cadena.
    /// </summary>
    /// <param name="value">Valor del enum a convertir.</param>
    /// <returns>
    /// Cadena que representa el tipo de búsqueda:
    /// <list type="bullet">
    /// <item><description>"equals" para <see cref="StringSearchType.EQ"/></description></item>
    /// <item><description>"like" para <see cref="StringSearchType.LIKE"/></description></item>
    /// </list>
    /// </returns>
    /// <exception cref="InvalidCastException">
    /// Se lanza cuando se proporciona un valor no definido en el enum <see cref="StringSearchType"/>.
    /// </exception>
    public static string ToString(StringSearchType value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value switch
        {
            StringSearchType.EQ => "equals",
            StringSearchType.LIKE => "like",
            _ => throw new InvalidCastException()
        };
    }

    /// <summary>
    /// Convierte una cadena de texto en su correspondiente valor del enum <see cref="StringSearchType"/>.
    /// </summary>
    /// <param name="value">Cadena de texto a convertir. Los valores válidos son: "equals" o "like".</param>
    /// <returns>
    /// El valor del enum <see cref="StringSearchType"/> correspondiente a la cadena proporcionada:
    /// <list type="bullet">
    /// <item><description><see cref="StringSearchType.EQ"/> para la cadena "equals"</description></item>
    /// <item><description><see cref="StringSearchType.LIKE"/> para la cadena "like"</description></item>
    /// </list>
    /// </returns>
    /// <exception cref="InvalidCastException">
    /// Se lanza cuando la cadena proporcionada no coincide con ninguno de los valores esperados ("equals" o "like").
    /// </exception>
    public static StringSearchType FromString(string value)
    {
        return value switch
        {
            "equals" => StringSearchType.EQ,
            "like" => StringSearchType.LIKE,
            _ => throw new InvalidCastException()
        };
    }
}
