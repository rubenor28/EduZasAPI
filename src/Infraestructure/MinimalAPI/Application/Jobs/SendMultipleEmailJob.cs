using System.Text.Json;
using Application.Services;
using Quartz;

namespace MinimalAPI.Application.Jobs;

[DisallowConcurrentExecution] // Evita saturar el servidor SMTP con múltiples conexiones simultáneas
public class SendMultipleEmailJob(
    IEmailSender emailSender, 
    ILogger<SendMultipleEmailJob> logger
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;
        var emailsJson = dataMap.GetString("EmailsJson");

        if (string.IsNullOrEmpty(emailsJson))
        {
            logger.LogWarning("El Job de emails se ejecutó sin datos en 'EmailsJson'.");
            return;
        }

        try
        {
            // Deserializamos a una lista para aprovechar el envío masivo
            var messages = JsonSerializer.Deserialize<List<EmailMessage>>(emailsJson);

            if (messages == null || messages.Count == 0) return;

            logger.LogInformation("Iniciando envío de {Count} correos.", messages.Count);
            
            await emailSender.SendBulkAsync(messages, context.CancellationToken);
            
            logger.LogInformation("Envío de correos finalizado exitosamente.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error crítico en SendEmailJob.");
            // Si el error es temporal (ej. servidor SMTP caído), Quartz reintentará
            throw new JobExecutionException(ex) { RefireImmediately = true };
        }
    }
}
