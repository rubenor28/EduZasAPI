namespace EduZasAPI.Application.Ports.Mappers;

using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Domain.Enums.Common;

/// <summary>
/// Métodos de extensión para mapear entre <see cref="StringSearchType"/>,
/// cadenas de texto y valores numéricos.
/// </summary>
public static class StringSearchTypeExtensions
{
    /// <summary>
    /// Convierte un valor de <see cref="StringSearchType"/> en su representación textual.
    /// </summary>
    /// <param name="value">Valor del enumerado a convertir.</param>
    /// <returns>
    /// Una cadena equivalente si existe mapeo, o vacío si no hay una representación definida.
    /// </returns>
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
    /// Convierte una cadena en su valor correspondiente de <see cref="StringSearchType"/>.
    /// </summary>
    /// <param name="value">Cadena a convertir.</param>
    /// <returns>
    /// Un <see cref="Optional{StringSearchType}"/> con el valor del enumerado
    /// si la cadena es válida, o vacío en caso contrario.
    /// </returns>
    public static Optional<StringSearchType> FromString(string value)
    {
        return value switch
        {
            "equals" => Optional<StringSearchType>.Some(StringSearchType.EQ),
            "like" => Optional<StringSearchType>.Some(StringSearchType.LIKE),
            _ => Optional<StringSearchType>.None(),
        };
    }

    /// <summary>
    /// Convierte un valor de <see cref="StringSearchType"/> a su representación numérica sin signo.
    /// </summary>
    /// <param name="value">Valor del enumerado.</param>
    /// <returns>Valor numérico equivalente.</returns>
    public static uint ToUInt(this StringSearchType value) => (uint)value;
}
