using Application.DTOs.Notifications;
using Domain.ValueObjects;

namespace Application.Services;

/// <summary>
/// Define un contrato para un programador de tareas en segundo plano.
/// </summary>
public interface ITaskScheduler
{
    /// <summary>
    /// Encola la creación de una notificación para un conjunto de usuarios.
    /// </summary>
    /// <param name="notification">Los datos de la notificación a crear.</param>
    /// <param name="userIds">La lista de IDs de usuarios que recibirán la notificación.</param>
    Task CreateNotification(NewNotificationDTO notification, IEnumerable<ulong> userIds);

    /// <summary>
    /// Encola el envío masivo de correos electrónicos.
    /// </summary>
    /// <param name="notification">La lista de mensajes de correo a enviar.</param>
    Task BulkSendEmail(IEnumerable<EmailMessage> notification);

    /// <summary>
    /// Programa una tarea para marcar las respuestas de una evaluación como finalizadas después de una fecha límite.
    /// </summary>
    /// <param name="classId">El ID de la clase.</param>
    /// <param name="testId">El ID de la evaluación.</param>
    /// <param name="deadline">La fecha y hora límite para finalizar los intentos.</param>
    Task ScheduleMarkAnswersAsFinished(string classId, Guid testId, DateTimeOffset deadline);
}
