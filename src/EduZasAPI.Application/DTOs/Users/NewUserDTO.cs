using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Users;

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
public class NewUserDTO
{
    private string _firstName = string.Empty;
    private string _fatherLastName = string.Empty;
    private Optional<string> _midName = Optional<string>.None();
    private Optional<string> _motherLastName = Optional<string>.None();

    /// <summary>
    /// Obtiene o establece el primer nombre del usuario.
    /// El valor se normaliza automáticamente a mayúsculas invariables.
    /// </summary>
    /// <value>Primer nombre del usuario. Campo obligatorio.</value>
    public required string FirstName
    {
        get => _firstName;
        set => _firstName = (value ?? throw new ArgumentNullException(nameof(FirstName))).ToUpperInvariant();
    }

    /// <summary>
    /// Obtiene o establece el apellido paterno del usuario.
    /// El valor se normaliza automáticamente a mayúsculas invariables.
    /// </summary>
    /// <value>Apellido paterno del usuario. Campo obligatorio.</value>
    public required string FatherLastName
    {
        get => _fatherLastName;
        set => _fatherLastName = (value ?? throw new ArgumentNullException(nameof(FatherLastName))).ToUpperInvariant();
    }

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
    /// Obtiene o establece el apellido materno del usuario.
    /// </summary>
    /// <value>
    /// <see cref="Optional{String}"/> que contiene el apellido materno en mayúsculas
    /// si se proporciona, o <c>None</c> si no está presente.
    /// Valor por defecto: <c>None</c>.
    /// </value>
    public Optional<string> MotherLastname
    {
        get => _motherLastName;
        set => _motherLastName = value is null
          ? Optional<string>.None()
          : value.Map(s => s.ToUpperInvariant());
    }

    /// <summary>
    /// Obtiene o establece el segundo nombre del usuario.
    /// </summary>
    /// <value>
    /// <see cref="Optional{String}"/> que contiene el segundo nombre en mayúsculas
    /// si se proporciona, o <c>None</c> si no está presente.
    /// Valor por defecto: <c>None</c>.
    /// </value>
    public Optional<string> MidName
    {
        get => _midName;
        set => _midName = value is null
          ? Optional<string>.None()
          : value.Map(s => s.ToUpperInvariant());
    }
}
