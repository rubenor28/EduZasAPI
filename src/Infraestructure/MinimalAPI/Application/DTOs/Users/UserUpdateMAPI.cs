using Application.DTOs.Common;

namespace MinimalAPI.Application.DTOs.Users;

/// <summary>
/// Representa los datos para actualizar la información de un usuario existente en el sistema.
/// </summary>
/// <remarks>
/// Esta clase encapsula la información que puede ser modificada de un usuario,
/// incluyendo datos personales, credenciales y estado. Todos los campos principales
/// son obligatorios para garantizar la integridad de la actualización.
/// </remarks>
public sealed record UserUpdateMAPI
{
    public required ulong Id { get; set; }
    public required bool Active { get; set; }
    public required uint Role { get; set; }
    public required string FirstName { get; set; }
    public required string FatherLastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string? MidName { get; set; }
    public required string? MotherLastname { get; set; }
}
