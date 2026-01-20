namespace Application.Services;

/// <summary>
/// Contrato para el servicio de envío de correos electrónicos.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Envía un correo electrónico de forma asíncrona.
    /// </summary>
    /// <param name="message">Detalles del correo a enviar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envía varios correo electrónico de forma asíncrona.
    /// </summary>
    /// <param name="messages">Detalles de los correos a enviar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task SendBulkAsync(
        IEnumerable<EmailMessage> messages,
        CancellationToken cancellationToken = default
    );
}
