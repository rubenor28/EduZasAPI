namespace Application.Services;

/// <summary>
/// Define un contrato para exportar bases de datos.
/// </summary>
public interface IDatabaseExporter
{
    /// <summary>
    /// Exporta una copia de seguridad de la base de datos y la devuelve como un stream.
    /// </summary>
    /// <returns>Una tarea que representa la operación de exportación asíncrona, conteniendo el stream del respaldo.</returns>
    Task<Stream> ExportBackupAsync();
}
