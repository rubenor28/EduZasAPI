using Application.Configuration;
using Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace MailKitProj;

/// <summary>
/// Implementación del servicio de envío de correos utilizando MailKit.
/// </summary>
/// <param name="smtpSettings">Configuración SMTP.</param>
public class SmtpEmailSender(IOptions<SmtpSettings> smtpSettings) : IEmailSender
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    /// <summary>
    /// Envía un correo electrónico de forma asíncrona.
    /// </summary>
    /// <param name="message">Mensaje a enviar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Tarea asíncrona.</returns>
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        using var client = new SmtpClient();

        await ConnectAndAuthenticateAsync(client, cancellationToken);

        var mimeMessage = CreateMimeMessage(message);
        await client.SendAsync(mimeMessage, cancellationToken);

        await client.DisconnectAsync(true, cancellationToken);
    }

    public async Task SendBulkAsync(
        IEnumerable<EmailMessage> messages,
        CancellationToken cancellationToken = default
    )
    {
        using var client = new SmtpClient();

        try
        {
            await ConnectAndAuthenticateAsync(client, cancellationToken);

            foreach (var message in messages)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    var mimeMessage = CreateMimeMessage(message);
                    await client.SendAsync(mimeMessage, cancellationToken);
                }
                catch
                {
                    Console.WriteLine(
                        $"[IEmailSender] Error al enviar correo indivual a: {message.To} "
                    );
                }
            }
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Centraliza la conexión y autenticación para evitar duplicidad.
    /// </summary>
    private async Task ConnectAndAuthenticateAsync(
        ISmtpClient client,
        CancellationToken cancellationToken
    )
    {
        await client.ConnectAsync(
            _smtpSettings.Server,
            _smtpSettings.Port,
            SecureSocketOptions.StartTls,
            cancellationToken
        );

        await client.AuthenticateAsync(
            _smtpSettings.Username,
            _smtpSettings.Password,
            cancellationToken
        );
    }

    /// <summary>
    /// Mapea nuestro DTO EmailMessage al MimeMessage de MailKit.
    /// </summary>
    private MimeMessage CreateMimeMessage(EmailMessage message)
    {
        var mimeMessage = new MimeMessage();

        mimeMessage.From.Add(
            new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail)
        );

        mimeMessage.To.AddRange(
            message.To.Select(email => new MailboxAddress(string.Empty, email))
        );

        if (message.Cc?.Any() == true)
        {
            mimeMessage.Cc.AddRange(
                message.Cc.Select(email => new MailboxAddress(string.Empty, email))
            );
        }

        if (message.Bcc?.Any() == true)
        {
            mimeMessage.Bcc.AddRange(
                message.Bcc.Select(email => new MailboxAddress(string.Empty, email))
            );
        }

        mimeMessage.Subject = message.Subject;
        mimeMessage.Body = new TextPart(message.IsBodyHtml ? TextFormat.Html : TextFormat.Plain)
        {
            Text = message.Body,
        };

        return mimeMessage;
    }
}
