namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Users;

/// <summary>
/// Representa los datos requeridos para crear un nuevo usuario en el sistema.
/// </summary>
/// <remarks>
/// Esta clase encapsula la información mínima y obligatoria necesaria para
/// registrar un nuevo usuario, junto con campos opcionales adicionales.
/// Utiliza campos requeridos (required) para garantizar que la información
/// esencial esté siempre presente durante la creación del usuario.
/// </remarks>
public class NewUserMAPI
{
    /// <summary>
    /// Obtiene o establece el primer nombre del usuario.
    /// </summary>
    /// <value>Primer nombre del usuario. Campo obligatorio.</value>
    public required string FirstName { get; set; }

    /// <summary>
    /// Obtiene o establece el apellido paterno del usuario.
    /// </summary>
    /// <value>Apellido paterno del usuario. Campo obligatorio.</value>
    public required string FatherLastName { get; set; }

    /// <summary>
    /// Obtiene o establece la dirección de correo electrónico del usuario.
    /// </summary>
    /// <value>Email del usuario. Campo obligatorio.</value>
    public required string Email { get; set; }

    /// <summary>
    /// Obtiene o establece la contraseña del usuario.
    /// </summary>
    /// <value>Contraseña del usuario. Campo obligatorio.</value>
    public required string Password { get; set; }

    /// <summary>
    /// Obtiene o establece el apellido materno del usuario (opcional).
    /// </summary>
    /// <value>
    /// Optional que contiene el apellido materno si está presente,
    /// o null si no se proporciona. Valor por defecto: null.
    /// </value>
    public string? MotherLastname { get; set; } = null;

    /// <summary>
    /// Obtiene o establece el segundo nombre del usuario (opcional).
    /// </summary>
    /// <value>
    /// Optional que contiene el segundo nombre si está presente,
    /// o null si no se proporciona. Valor por defecto: null.
    /// </value>
    public string? MidName { get; set; } = null;
}
