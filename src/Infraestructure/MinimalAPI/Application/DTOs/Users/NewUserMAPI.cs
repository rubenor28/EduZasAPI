namespace MinimalAPI.Application.DTOs.Users;

public sealed record NewUserMAPI
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
    public required uint Role { get; init; }

    /// <summary>
    /// Obtiene o inicializa el apellido materno del usuario.
    /// </summary>
    /// <value>
    /// Un objeto de tipo <see cref="string?"/> que contiene el apellido materno
    /// si se proporciona, o <c>None</c> si no está presente. Se normaliza a mayúsculas invariables.
    /// </value>
    public string? MotherLastname { get; init; }

    /// <summary>
    /// Obtiene o inicializa el segundo nombre del usuario.
    /// </summary>
    /// <value>
    /// Un objeto de tipo <see cref="string?"/> que contiene el segundo nombre
    /// si se proporciona, o <c>None</c> si no está presente. Se normaliza a mayúsculas invariables.
    /// </value>
    public string? MidName { get; init; }
}
