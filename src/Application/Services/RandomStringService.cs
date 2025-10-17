using System.Text;

namespace Application.Services;

public sealed record RandomStringGeneratorArgs
{
    public char[]? AllowedChars { get; init; }
    public ulong? MaxStrLenght { get; init; }
}

/// <summary>
/// Proporciona un servicio para generar cadenas aleatorias.
/// </summary>
public class RandomStringGeneratorService(char[] chars, uint strLength)
    : IRandomStringGeneratorService,
        IRandomStringGeneratorService<RandomStringGeneratorArgs?>
{
    /// <summary>
    /// Instancia de la clase Random utilizada para la generación de números aleatorios.
    /// </summary>
    private readonly Random _rdm = new();

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
    /// Genera una cadena aleatoria de la longitud especificada.
    /// </summary>
    /// <returns>Una cadena de texto aleatoria.</returns>
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
