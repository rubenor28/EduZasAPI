namespace Application.Services;

/// <summary>
/// Contrato para importar respaldos de base de datos.
/// </summary>
public interface IDatabaseImporter
{
    /// <summary>
    /// Restaura la base de datos desde un stream.
    /// </summary>
    Task RestoreAsync(Stream input);
}
