namespace Application.Services;

/// <summary>
/// Contrato para generadores de cadenas aleatorias.
/// </summary>
public interface IRandomStringGeneratorService
{
    /// <summary>
    /// Genera una cadena aleatoria.
    /// </summary>
    /// <returns>Cadena generada.</returns>
    public string Generate(); // Sugerencia: Renombrar a Generate() o GenerateCode()
}

/// <summary>
/// Contrato para generadores de cadenas aleatorias con argumentos.
/// </summary>
/// <typeparam name="T">Tipo de argumento de configuración.</typeparam>
public interface IRandomStringGeneratorService<T>
{
    /// <summary>
    /// Genera una cadena aleatoria usando un argumento.
    /// </summary>
    /// <param name="arg">Argumento de configuración.</param>
    /// <returns>Cadena generada.</returns>
    public string Generate(T arg); // Sugerencia: Renombrar a Generate(T arg) o GenerateCode(T arg)
}
