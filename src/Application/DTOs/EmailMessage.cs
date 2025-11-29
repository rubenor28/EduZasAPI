/// <summary>
/// Representa un mensaje de correo electrónico que se va a enviar.
/// </summary>
/// <remarks>
/// Este DTO se utiliza para transportar los datos de un correo electrónico a través de las capas de la aplicación,
/// desde los casos de uso hasta el servicio de envío de correo en la infraestructura.
/// </remarks>
namespace Application.DTOs;

public class EmailMessage
{
    /// <summary>
    /// Obtiene la lista de destinatarios principales del correo.
    /// </summary>
    /// <value>
    /// Una lista de solo lectura de direcciones de correo electrónico. No puede ser nula.
    /// </value>
    public required IReadOnlyList<string> To { get; init; }

    /// <summary>
    /// Obtiene la lista de destinatarios en copia de carbón (CC).
    /// </summary>
    /// <value>
    //. Una lista opcional de solo lectura de direcciones de correo electrónico. Puede ser nula o vacía.
    /// </value>
    public IReadOnlyList<string>? Cc { get; init; }

    /// <summary>
    /// Obtiene la lista de destinatarios en copia de carbón oculta (BCC).
    /// </summary>
    /// <remarks>
    /// Ideal para envíos masivos donde los destinatarios no deben ver las direcciones de los demás.
    /// </remarks>
    /// <value>
    /// Una lista opcional de solo lectura de direcciones de correo electrónico. Puede ser nula o vacía.
    /// </value>
    public IReadOnlyList<string>? Bcc { get; init; }

    /// <summary>
    /// Obtiene el asunto (título) del correo electrónico.
    /// </summary>
    /// <value>
    /// El texto del asunto. No puede ser nulo.
    /// </value>
    public required string Subject { get; init; }

    /// <summary>
    /// Obtiene el cuerpo del mensaje del correo electrónico.
    /// </summary>
    /// <value>
    /// El contenido del correo, que puede ser texto plano o HTML. No puede ser nulo.
    /// </value>
    public required string Body { get; init; }

    /// <summary>
    /// Obtiene un valor que indica si el cuerpo del correo está en formato HTML.
    /// </summary>
    /// <value>
    /// <c>true</c> si el cuerpo es HTML; de lo contrario, <c>false</c> para texto plano.
    /// El valor predeterminado es <c>true</c>.
    /// </value>
    public bool IsBodyHtml { get; init; } = true;
}
