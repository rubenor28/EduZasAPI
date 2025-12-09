using Domain.Enums;

namespace Application.DTOs.Users;

/// <summary>
/// Representa los datos requeridos para crear un nuevo usuario en el sistema.
/// </summary>
/// <remarks>
/// Esta clase define los campos mínimos obligatorios y opcionales necesarios para
/// registrar un usuario. Los nombres y apellidos se almacenan automáticamente
/// en mayúsculas invariables, garantizando consistencia en el formato.
/// Los campos opcionales están representados mediante <see cref="T?"/>,
/// lo que evita el uso de valores nulos y permite un control más seguro.
/// </remarks>
public sealed record NewUserDTO
{
    /// <summary>
    /// Obtiene o inicializa el primer nombre del usuario.
    /// </summary>
    /// <value>Primer nombre del usuario. Campo obligatorio. Se normaliza automáticamente a mayúsculas invariables.</value>
    public required string FirstName { get; init; }

    /// <summary>
    /// Obtiene o inicializa el apellido paterno del usuario.
    /// </summary>
    /// <value>Apellido paterno del usuario. Campo obligatorio. Se normaliza automáticamente a mayúsculas invariables.</value>
    public required string FatherLastname { get; init; }

    /// <summary>
    /// Obtiene o inicializa la dirección de correo electrónico del usuario.
    /// </summary>
    /// <value>Dirección de correo electrónico del usuario. Campo obligatorio.</value>
    public required string Email { get; init; }

    /// <summary>
    /// Obtiene o inicializa la contraseña del usuario.
    /// </summary>
    /// <value>Contraseña del usuario. Campo obligatorio.</value>
    public required string Password { get; init; }

    /// <summary>
    /// Obtiene o inicializa el tipo de usuario.
    /// </summary>
    public required UserType Role { get; init; }

    /// <summary>
    /// Obtiene o inicializa el apellido materno del usuario.
    /// </summary>
    /// <value>
    /// Apellido materno del usuario (opcional). Se normaliza a mayúsculas invariables.
    /// </value>
    public string? MotherLastname { get; init; }

    /// <summary>
    /// Obtiene o inicializa el segundo nombre del usuario.
    /// </summary>
    /// <value>
    /// Segundo nombre del usuario (opcional). Se normaliza a mayúsculas invariables.
    /// </value>
    public string? MidName { get; init; }
}
