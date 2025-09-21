namespace EduZasAPI.Domain.Enums.Common;

using EduZasAPI.Domain.ValueObjects.Common;

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
/// Proporciona métodos de extensión para conversión entre el enum <see cref="StringSearchType"/> y sus representaciones en cadena.
/// </summary>
/// <remarks>
/// Esta clase ofrece funcionalidades para convertir valores del enum <see cref="StringSearchType"/>
/// a sus equivalentes en cadena de texto y viceversa, utilizando el tipo <see cref="Optional{T}"/>
/// para manejar de forma segura conversiones inválidas o valores nulos.
/// </remarks>
public class StringSearchTypeExtensions
{
    /// <summary>
    /// Convierte un valor del enum <see cref="StringSearchType"/> a su representación en cadena.
    /// </summary>
    /// <param name="value">Valor del enum a convertir.</param>
    /// <returns>
    /// Un <see cref="Optional{T}"/> que contiene la cadena correspondiente al valor del enum si es válido,
    /// o <see cref="Optional{T}.None"/> si el valor no está mapeado.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Se lanza cuando el valor del enum proporcionado es nulo.
    /// </exception>
    /// <example>
    /// <code>
    /// var result = StringSearchTypeExtensions.ToString(StringSearchType.EQ);
    /// // result contiene Optional<string>.Some("equals")
    /// </code>
    /// </example>
    public static Optional<string> ToString(StringSearchType value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value switch
        {
            StringSearchType.EQ => Optional<string>.Some("equals"),
            StringSearchType.LIKE => Optional<string>.Some("like"),
            _ => Optional<string>.None(),
        };
    }

    /// <summary>
    /// Convierte una cadena de texto a su correspondiente valor del enum <see cref="StringSearchType"/>.
    /// </summary>
    /// <param name="value">Cadena de texto a convertir.</param>
    /// <returns>
    /// Un <see cref="Optional{T}"/> que contiene el valor del enum correspondiente a la cadena si es válida,
    /// o <see cref="Optional{T}.None"/> si la cadena no coincide con ningún valor del enum.
    /// </returns>
    /// <remarks>
    /// Este método es case-sensitive. Solo reconoce "equals" y "like" en minúsculas.
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = StringSearchTypeExtensions.FromString("like");
    /// // result contiene Optional<StringSearchType>.Some(StringSearchType.LIKE)
    /// </code>
    /// </example>
    public static Optional<StringSearchType> FromString(string value)
    {
        return value switch
        {
            "equals" => Optional<StringSearchType>.Some(StringSearchType.EQ),
            "like" => Optional<StringSearchType>.Some(StringSearchType.LIKE),
            _ => Optional<StringSearchType>.None(),
        };
    }
}
