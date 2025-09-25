using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;

namespace EduZasAPI.Application.Users;

/// <summary>
/// Métodos de extensión para mapear valores de <see cref="UserType"/> a representaciones
/// en <see cref="string"/> o <see cref="int"/> y viceversa.
/// </summary>
public static class UserTypeMapper
{
    /// <summary>
    /// Convierte un <see cref="UserType"/> a su representación en string.
    /// </summary>
    /// <param name="value">Valor de <see cref="UserType"/>.</param>
    /// <returns>Un <see cref="Optional{String}"/> con el valor correspondiente, o vacío si no es válido.</returns>
    public static Optional<string> ToString(this UserType value)
    {
        return value switch
        {
            UserType.STUDENT => Optional<string>.Some("student"),
            UserType.PROFESSOR => Optional<string>.Some("professor"),
            UserType.ADMIN => Optional<string>.Some("admin"),
            _ => Optional<string>.None(),
        };
    }

    /// <summary>
    /// Convierte un <see cref="UserType"/> a su representación en entero.
    /// </summary>
    /// <param name="value">Valor de <see cref="UserType"/>.</param>
    /// <returns>Un <see cref="Optional{Int32}"/> con el valor correspondiente, o vacío si no es válido.</returns>
    public static Optional<int> ToInt(this UserType value)
    {
        return value switch
        {
            UserType.STUDENT => Optional<int>.Some(0),
            UserType.PROFESSOR => Optional<int>.Some(1),
            UserType.ADMIN => Optional<int>.Some(2),
            _ => Optional<int>.None(),
        };
    }

    /// <summary>
    /// Convierte un string a su <see cref="UserType"/> correspondiente.
    /// </summary>
    /// <param name="value">Valor en string a convertir.</param>
    /// <returns>Un <see cref="Optional{UserType}"/> con el valor correspondiente, o vacío si no es válido.</returns>
    public static Optional<UserType> FromString(string value) => value switch
    {
        "student" => Optional<UserType>.Some(UserType.STUDENT),
        "professor" => Optional<UserType>.Some(UserType.PROFESSOR),
        "admin" => Optional<UserType>.Some(UserType.ADMIN),
        _ => Optional<UserType>.None(),
    };

    /// <summary>
    /// Convierte un entero a su <see cref="UserType"/> correspondiente.
    /// </summary>
    /// <param name="value">Valor entero a convertir.</param>
    /// <returns>Un <see cref="Optional{UserType}"/> con el valor correspondiente, o vacío si no es válido.</returns>
    public static Optional<UserType> FromInt(int value) => value switch
    {
        0 => Optional<UserType>.Some(UserType.STUDENT),
        1 => Optional<UserType>.Some(UserType.PROFESSOR),
        2 => Optional<UserType>.Some(UserType.ADMIN),
        _ => Optional<UserType>.None(),
    };
}
