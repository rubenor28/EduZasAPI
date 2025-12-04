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
    public required ulong Id { get; init; }
    public required bool Active { get; init; }
    public required uint Role { get; init; }
    public required string FirstName { get; init; }
    public required string FatherLastname { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string? MidName { get; init; }
    public required string? MotherLastname { get; init; }
}
