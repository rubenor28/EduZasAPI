namespace Application.Services;

public interface IDatabaseExporter
{
    Task ExportBackupAsync(Stream outputStream);
}
