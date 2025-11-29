using Application.DTOs;

namespace Application.Services;

/// <summary>
/// Define el contrato para un servicio de envío de correos electrónicos.
/// </summary>
/// <remarks>
/// Esta abstracción permite desacoplar la lógica de la aplicación de la implementación
/// concreta de envío de correos (ej. SMTP, SendGrid, etc.), facilitando las pruebas y la mantenibilidad.
/// </remarks>
public interface IEmailSender
{
    /// <summary>
    /// Envía un correo electrónico de forma asíncrona.
    /// </summary>
    /// <param name="message">El objeto <see cref="EmailMessage"/> que contiene los detalles del correo a enviar.</param>
    /// <param name="cancellationToken">Un token de cancelación para la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación de envío asíncrona.</returns>
    /// <exception cref="Exception">Se lanza si ocurre un error durante el envío del correo.</exception>
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
