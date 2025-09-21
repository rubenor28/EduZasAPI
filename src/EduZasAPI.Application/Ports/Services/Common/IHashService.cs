namespace EduZasAPI.Application.Ports.Services.Common;

/// <summary>
/// Define un contrato para servicios de hashing de cadenas de texto.
/// </summary>
/// <remarks>
/// Esta interfaz se utiliza para implementaciones que proporcionan funcionalidades
/// de hashing para contraseñas u otros datos sensibles, incluyendo la generación
/// de hashes y la verificación de coincidencias.
/// </remarks>
public interface IHashService
{
    /// <summary>
    /// Genera un hash a partir de una cadena de texto de entrada.
    /// </summary>
    /// <param name="input">Cadena de texto original a hashear.</param>
    /// <returns>El hash generado a partir de la cadena de entrada.</returns>
    string Hash(string input);

    /// <summary>
    /// Verifica si una cadena de texto coincide con un hash dado.
    /// </summary>
    /// <param name="input">Cadena de texto original a verificar.</param>
    /// <param name="hash">Hash contra el cual se compara la cadena de texto.</param>
    /// <returns>
    /// true si la cadena de texto coincide con el hash proporcionado; false en caso contrario.
    /// </returns>
    bool Matches(string input, string hash);
}
