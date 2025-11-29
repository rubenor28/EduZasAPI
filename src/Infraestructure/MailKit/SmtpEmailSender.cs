using Application.Configuration;
using Application.DTOs;
using Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace MailKitProj;

/// <summary>
/// Implementación del servicio de envío de correos utilizando MailKit y un servidor SMTP.
/// </summary>
/// <remarks>
/// Inicializa una new instancia de la clase <see cref="SmtpEmailSender"/>.
/// </remarks>
/// <param name="smtpSettings">La configuración SMTP inyectada a través del patrón de opciones.</param>
public class SmtpEmailSender(IOptions<SmtpSettings> smtpSettings) : IEmailSender
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    /// <summary>
    /// Envía un correo electrónico de forma asíncrona utilizando la configuración SMTP proporcionada.
    /// </summary>
    /// <param name="message">El mensaje de correo electrónico a enviar.</param>
    /// <param name="cancellationToken">El token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación de envío asíncrona.</returns>
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(
            new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail)
        );

        mimeMessage.To.AddRange(
            message.To.Select(email => new MailboxAddress(string.Empty, email))
        );

        if (message.Cc?.Any() == true)
            mimeMessage.Cc.AddRange(
                message.Cc.Select(email => new MailboxAddress(string.Empty, email))
            );

        if (message.Bcc?.Any() == true)
            mimeMessage.Bcc.AddRange(
                message.Bcc.Select(email => new MailboxAddress(string.Empty, email))
            );

        mimeMessage.Subject = message.Subject;
        mimeMessage.Body = new TextPart(message.IsBodyHtml ? TextFormat.Html : TextFormat.Plain)
        {
            Text = message.Body,
        };

        using var client = new SmtpClient();

        // Conectar al servidor SMTP. Usar STARTTLS que es el estándar de seguridad moderno.
        await client.ConnectAsync(
            _smtpSettings.Server,
            _smtpSettings.Port,
            SecureSocketOptions.StartTls,
            cancellationToken
        );

        // Autenticarse con las credenciales proporcionadas.
        await client.AuthenticateAsync(
            _smtpSettings.Username,
            _smtpSettings.Password,
            cancellationToken
        );

        // Enviar el mensaje.
        await client.SendAsync(mimeMessage, cancellationToken);

        // Desconectarse del servidor.
        await client.DisconnectAsync(true, cancellationToken);
    }
}

