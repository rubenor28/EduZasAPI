using System.Text;

namespace EduZasAPI.Application.Common;

/// <summary>
/// Proporciona un servicio para generar cadenas aleatorias.
/// </summary>
public class RandomStringGeneratorService : IRandomStringGeneratorService
{
    /// <summary>
    /// Conjunto de caracteres autorizados para la generación de la cadena.
    /// </summary>
    private readonly char[] _chars;
    
    /// <summary>
    /// Longitud de la cadena aleatoria a generar.
    /// </summary>
    private readonly uint _tokenLength;
    
    /// <summary>
    /// Instancia de la clase Random utilizada para la generación de números aleatorios.
    /// </summary>
    private readonly Random _rdm;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de generación de cadenas aleatorias.
    /// </summary>
    /// <param name="authorizedChars">Una cadena que contiene todos los caracteres permitidos para la generación.</param>
    /// <param name="tokenLength">La longitud deseada para la cadena generada.</param>
    public RandomStringGeneratorService(string authorizedChars, uint tokenLength)
    {
        this._chars = authorizedChars.ToCharArray();
        this._tokenLength = tokenLength;
        this._rdm = new Random();
    }

    /// <summary>
    /// Genera una cadena aleatoria de la longitud especificada.
    /// </summary>
    /// <returns>Una cadena de texto aleatoria.</returns>
    public string Generate()
    {
        var builder = new StringBuilder();
        for (uint i = 0; i < _tokenLength; i++)
        {
            var rdmIdx = _rdm.Next(_chars.Length);
            builder.Append(_chars[rdmIdx]);
        }

        return builder.ToString();
    }
}
