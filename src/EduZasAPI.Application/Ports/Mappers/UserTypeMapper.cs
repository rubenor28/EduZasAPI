namespace EduZasAPI.Application.Ports.Mappers;

using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Domain.Enums.Users;

/// <summary>
/// Proporciona métodos de extensión para mapear entre <see cref="UserType"/> y valores numéricos.
/// </summary>
public static class UserTypeMapper
{
    /// <summary>
    /// Convierte un valor de <see cref="UserType"/> a su representación numérica sin signo.
    /// </summary>
    /// <param name="value">Valor del enumerado <see cref="UserType"/>.</param>
    /// <returns>Valor numérico equivalente.</returns>
    public static uint ToUInt(this UserType value) => (uint)value;

    /// <summary>
    /// Convierte un valor numérico en una instancia de <see cref="UserType"/> si es válido.
    /// </summary>
    /// <param name="value">Valor numérico a convertir.</param>
    /// <returns>
    /// Un <see cref="Optional{UserType}"/> con el valor del enumerado si es válido,
    /// o vacío si no corresponde a ningún valor definido en <see cref="UserType"/>.
    /// </returns>
    public static Optional<UserType> FromUInt(uint value)
    {
        return Enum.IsDefined(typeof(UserType), (int)value)
            ? Optional<UserType>.Some((UserType)value)
            : Optional<UserType>.None();
    }
}
