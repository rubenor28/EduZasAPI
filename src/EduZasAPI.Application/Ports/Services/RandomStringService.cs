using System.Text;

namespace EduZasAPI.Application.Common;


public class RandomStringGeneratorArgs
{
    public char[]? AllowedChars { get; init; }
    public ulong? MaxStrLenght { get; init; }
}

/// <summary>
/// Proporciona un servicio para generar cadenas aleatorias.
/// </summary>
public class RandomStringGeneratorService :
  IRandomStringGeneratorService,
  IRandomStringGeneratorService<RandomStringGeneratorArgs?>
{
    /// <summary>
    /// Conjunto de caracteres autorizados para la generación de la cadena.
    /// </summary>
    private readonly char[] _chars;

    /// <summary>
    /// Longitud de la cadena aleatoria a generar.
    /// </summary>
    private readonly uint _strLength;

    /// <summary>
    /// Instancia de la clase Random utilizada para la generación de números aleatorios.
    /// </summary>
    private readonly Random _rdm;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de generación de cadenas aleatorias.
    /// </summary>
    /// <param name="authorizedChars">Una cadena que contiene todos los caracteres permitidos para la generación.</param>
    /// <param name="tokenLength">La longitud deseada para la cadena generada.</param>
    public RandomStringGeneratorService(string authorizedChars, uint strLength)
    {
        this._chars = authorizedChars.ToCharArray();
        this._strLength = strLength;
        this._rdm = new Random();
    }

    public string Generate()
    {
        var builder = new StringBuilder();

        for (uint i = 0; i < _strLength; i++)
        {
            var rdmIdx = _rdm.Next(_chars.Length);
            builder.Append(_chars[rdmIdx]);
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

        var length = opts is not null &&
                     opts.MaxStrLenght is not null ?
                     opts.MaxStrLenght :
                     _strLength;

        var allowedChars = opts is not null &&
                           opts.AllowedChars is not null ?
                           opts.AllowedChars :
                           _chars;

        for (uint i = 0; i < length; i++)
        {
            var rdmIdx = _rdm.Next(allowedChars.Length);
            builder.Append(allowedChars[rdmIdx]);
        }

        return builder.ToString();
    }
}
