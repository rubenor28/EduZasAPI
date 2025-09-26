using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Métodos de extensión para mapear valores de <see cref="StringSearchType"/> a representaciones
/// en <see cref="string"/> o <see cref="int"/> y viceversa.
/// </summary>
public static class StringSearchMapper
{
    /// <summary>
    /// Convierte un <see cref="StringSearchType"/> a su representación en entero.
    /// </summary>
    /// <param name="value">Valor de <see cref="StringSearchType"/>.</param>
    /// <returns>Un <see cref="Optional{Int32}"/> con el valor correspondiente, o vacío si no es válido.</returns>
    public static Optional<int> ToInt(this StringSearchType value)
    {
        return value switch
        {
            StringSearchType.EQ => Optional<int>.Some(0),
            StringSearchType.LIKE => Optional<int>.Some(1),
            _ => Optional<int>.None(),
        };
    }

    /// <summary>
    /// Convierte un string a su <see cref="StringSearchType"/> correspondiente.
    /// </summary>
    /// <param name="value">Valor en string a convertir.</param>
    /// <returns>Un <see cref="Optional{StringSearchType}"/> con el valor correspondiente, o vacío si no es válido.</returns>
    public static Optional<StringSearchType> FromString(string value) => value switch
    {
        "EQUALS" => Optional<StringSearchType>.Some(StringSearchType.EQ),
        "LIKE" => Optional<StringSearchType>.Some(StringSearchType.LIKE),
        _ => Optional<StringSearchType>.None(),
    };

    /// <summary>
    /// Convierte un entero a su <see cref="StringSearchType"/> correspondiente.
    /// </summary>
    /// <param name="value">Valor entero a convertir.</param>
    /// <returns>Un <see cref="Optional{StringSearchType}"/> con el valor correspondiente, o vacío si no es válido.</returns>
    public static Optional<StringSearchType> FromInt(int value) => value switch
    {
        0 => Optional<StringSearchType>.Some(StringSearchType.EQ),
        1 => Optional<StringSearchType>.Some(StringSearchType.LIKE),
        _ => Optional<StringSearchType>.None(),
    };
}
