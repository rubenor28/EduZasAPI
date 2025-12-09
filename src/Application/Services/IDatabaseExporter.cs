namespace Application.Services;

/// <summary>
/// Contrato para exportar respaldos de la base de datos.
/// </summary>
public interface IDatabaseExporter
{
    /// <summary>
    /// Genera y retorna un respaldo de la base de datos.
    /// </summary>
    /// <returns>Stream con el contenido del respaldo.</returns>
    Task<Stream> ExportBackupAsync();
}
