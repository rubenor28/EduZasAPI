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
    /// <returns>Un <see cref="Result{Int32, Unit}"/> con el valor correspondiente, o error si no es válido.</returns>
    public static Result<int, Unit> ToInt(this StringSearchType value)
    {
        return value switch
        {
            StringSearchType.EQ => Result.Ok(0),
            StringSearchType.LIKE => Result.Ok(1),
            _ => Result<int, Unit>.Err(Unit.Value),
        };
    }

    /// <summary>
    /// Convierte un string a su <see cref="StringSearchType"/> correspondiente.
    /// </summary>
    /// <param name="value">Valor en string a convertir.</param>
    /// <returns>Un <see cref="Optional{StringSearchType}"/> con el valor correspondiente, o vacío si no es válido.</returns>
    public static Result<StringSearchType, Unit> FromString(string value) => value switch
    {
        "EQUALS" => Result.Ok(StringSearchType.EQ),
        "LIKE" => Result.Ok(StringSearchType.LIKE),
        _ => Result<StringSearchType, Unit>.Err(Unit.Value),
    };

    /// <summary>
    /// Convierte un entero a su <see cref="StringSearchType"/> correspondiente.
    /// </summary>
    /// <param name="value">Valor entero a convertir.</param>
    /// <returns>Un <see cref="Optional{StringSearchType}"/> con el valor correspondiente, o vacío si no es válido.</returns>
    public static Result<StringSearchType, Unit> FromInt(int value) => value switch
    {
        0 => Result.Ok(StringSearchType.EQ),
        1 => Result.Ok(StringSearchType.LIKE),
        _ => Result<StringSearchType, Unit>.Err(Unit.Value),
    };
}
