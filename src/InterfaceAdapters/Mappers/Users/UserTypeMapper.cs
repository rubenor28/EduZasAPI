using Domain.Enums;
using Domain.ValueObjects;

namespace InterfaceAdapters.Mappers.Users;

/// <summary>
/// Métodos de extensión para mapear valores de <see cref="UserType"/> a representaciones
/// en <see cref="string"/> o <see cref="int"/> y viceversa.
/// </summary>
public static class UserTypeMapper
{
    /// <summary>
    /// Convierte un <see cref="UserType"/> a su representación en entero.
    /// </summary>
    /// <param name="value">Valor de <see cref="UserType"/>.</param>
    /// <returns>Un <see cref="Optional{Int32}"/> con el valor correspondiente, o vacío si no es válido.</returns>
    public static Optional<int> ToInt(this UserType value)
    {
        return value switch
        {
            UserType.STUDENT => 0,
            UserType.PROFESSOR => 1,
            UserType.ADMIN => 2,
            _ => Optional<int>.None(),
        };
    }

    /// <summary>
    /// Convierte un string a su <see cref="UserType"/> correspondiente.
    /// </summary>
    /// <param name="value">Valor en string a convertir.</param>
    /// <returns>Un <see cref="Optional{UserType}"/> con el valor correspondiente, o vacío si no es válido.</returns>
    public static Optional<UserType> FromString(string value) =>
        value switch
        {
            "STUDENT" => UserType.STUDENT,
            "PROFESSOR" => UserType.PROFESSOR,
            "ADMIN" => UserType.ADMIN,
            _ => Optional<UserType>.None(),
        };

    /// <summary>
    /// Convierte un entero a su <see cref="UserType"/> correspondiente.
    /// </summary>
    /// <param name="value">Valor entero a convertir.</param>
    /// <returns>Un <see cref="Optional{UserType}"/> con el valor correspondiente, o vacío si no es válido.</returns>
    public static Optional<UserType> FromInt(int value) =>
        value switch
        {
            0 => UserType.STUDENT,
            1 => UserType.PROFESSOR,
            2 => UserType.ADMIN,
            _ => Optional<UserType>.None(),
        };
}
