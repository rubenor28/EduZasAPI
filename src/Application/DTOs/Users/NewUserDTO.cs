using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Users;

/// <summary>
/// Representa los datos requeridos para crear un nuevo usuario en el sistema.
/// </summary>
/// <remarks>
/// Esta clase define los campos mínimos obligatorios y opcionales necesarios para
/// registrar un usuario. Los nombres y apellidos se almacenan automáticamente
/// en mayúsculas invariables, garantizando consistencia en el formato.
/// Los campos opcionales están representados mediante <see cref="Optional{T}"/>,
/// lo que evita el uso de valores nulos y permite un control más seguro.
/// </remarks>
public sealed record NewUserDTO
{
    /// <summary>
    /// Obtiene o establece el primer nombre del usuario.
    /// </summary>
    /// <value>Primer nombre del usuario. Campo obligatorio. Se normaliza automáticamente a mayúsculas invariables.</value>
    public required string FirstName { get; set; }

    /// <summary>
    /// Obtiene o establece el apellido paterno del usuario.
    /// </summary>
    /// <value>Apellido paterno del usuario. Campo obligatorio. Se normaliza automáticamente a mayúsculas invariables.</value>
    public required string FatherLastName { get; set; }

    /// <summary>
    /// Obtiene o establece la dirección de correo electrónico del usuario.
    /// </summary>
    /// <value>Dirección de correo electrónico del usuario. Campo obligatorio.</value>
    public required string Email { get; set; }

    /// <summary>
    /// Obtiene o establece la contraseña del usuario.
    /// </summary>
    /// <value>Contraseña del usuario. Campo obligatorio.</value>
    public required string Password { get; set; }

    /// <summary>
    /// Obtiene o establece el apellido materno del usuario.
    /// </summary>
    /// <value>
    /// Un objeto de tipo <see cref="Optional{T}"/> que contiene el apellido materno
    /// si se proporciona, o <c>None</c> si no está presente. Se normaliza a mayúsculas invariables.
    /// </value>
    public Optional<string> MotherLastname { get; set; } = Optional<string>.None();

    /// <summary>
    /// Obtiene o establece el segundo nombre del usuario.
    /// </summary>
    /// <value>
    /// Un objeto de tipo <see cref="Optional{T}"/> que contiene el segundo nombre
    /// si se proporciona, o <c>None</c> si no está presente. Se normaliza a mayúsculas invariables.
    /// </value>
    public Optional<string> MidName { get; set; } = Optional<string>.None();
}
