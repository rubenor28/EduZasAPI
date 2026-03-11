using System.Text;

namespace Application.Services;

/// <summary>
/// Define los argumentos opcionales para la generación de cadenas aleatorias.
/// </summary>
public sealed record RandomStringGeneratorArgs
{
    /// <summary>
    /// Obtiene o inicializa el conjunto de caracteres permitidos para la generación.
    /// </summary>
    public char[]? AllowedChars { get; init; }
    /// <summary>
    /// Obtiene o inicializa la longitud máxima de la cadena a generar.
    /// </summary>
    public ulong? MaxStrLenght { get; init; }
}

/// <summary>
/// Implementación de servicio para generar cadenas aleatorias.
/// </summary>
/// <param name="chars">El conjunto de caracteres por defecto a utilizar.</param>
/// <param name="strLength">La longitud por defecto de la cadena a generar.</param>
public class RandomStringGeneratorService(char[] chars, uint strLength)
    : IRandomStringGeneratorService,
        IRandomStringGeneratorService<RandomStringGeneratorArgs?>
{
    private readonly Random _rdm = new();

    /// <inheritdoc/>
    public string Generate()
    {
        var builder = new StringBuilder();

        for (uint i = 0; i < strLength; i++)
        {
            var rdmIdx = _rdm.Next(chars.Length);
            builder.Append(chars[rdmIdx]);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Genera una cadena aleatoria con opciones específicas.
    /// </summary>
    /// <param name="opts">Argumentos opcionales para personalizar la generación.</param>
    /// <returns>Cadena generada.</returns>
    public string Generate(RandomStringGeneratorArgs? opts = null)
    {
        var builder = new StringBuilder();

        var length =
            opts is not null && opts.MaxStrLenght is not null ? opts.MaxStrLenght : strLength;

        var allowedChars =
            opts is not null && opts.AllowedChars is not null ? opts.AllowedChars : chars;

        for (uint i = 0; i < length; i++)
        {
            var rdmIdx = _rdm.Next(allowedChars.Length);
            builder.Append(allowedChars[rdmIdx]);
        }

        return builder.ToString();
    }
}
