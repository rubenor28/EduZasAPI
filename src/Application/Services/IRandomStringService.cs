namespace Application.Services;

/// <summary>
/// Define un contrato para servicios que generan cadenas de texto aleatorias.
/// </summary>
/// <remarks>
/// Esta interfaz se utiliza para implementaciones que crean identificadores, códigos
/// o cualquier otra cadena aleatoria con una configuración predefinida (longitud, caracteres permitidos, etc.).
/// </remarks>
public interface IRandomStringGeneratorService
{
    /// <summary>
    /// Genera una nueva cadena de texto aleatoria.
    /// </summary>
    /// <returns>Una cadena de texto aleatoria basada en la configuración del servicio.</returns>
    public string Generate(); // Sugerencia: Renombrar a Generate() o GenerateCode()
}

/// <summary>
/// Define un contrato para servicios que generan cadenas de texto aleatorias utilizando un argumento.
/// </summary>
/// <typeparam name="T">Tipo del argumento que puede influir en la generación de la cadena.</typeparam>
/// <remarks>
/// Esta interfaz es útil cuando la generación de la cadena aleatoria puede ser
/// influenciada por datos externos, como usar un objeto de configuración, una
/// semilla específica o cualquier otro parámetro.
/// </remarks>
public interface IRandomStringGeneratorService<T>
{
    /// <summary>
    /// Genera una nueva cadena de texto aleatoria utilizando el argumento proporcionado.
    /// </summary>
    /// <param name="arg">Argumento de tipo T que influye en la generación de la cadena.</param>
    /// <returns>Una cadena de texto aleatoria.</returns>
    /// <remarks>
    /// La lógica de cómo el argumento 'arg' afecta el resultado final depende de la implementación concreta.
    /// </remarks>
    public string Generate(T arg); // Sugerencia: Renombrar a Generate(T arg) o GenerateCode(T arg)
}
