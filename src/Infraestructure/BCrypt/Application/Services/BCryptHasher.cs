using Application.Services;
using BCrypt.Net;

namespace Bcrypt.Application.Services;

// Si no está dentro del namespace 
// no funciona esta cosa

/// <summary>
/// Implementación del servicio de hashing utilizando el algoritmo BCrypt.
/// </summary>
/// <remarks>
/// Esta clase proporciona funcionalidades de hashing y verificación de contraseñas
/// utilizando la biblioteca BCrypt.Net, que es una implementación del algoritmo
/// de hashing BCrypt conocido por su seguridad frente a ataques de fuerza bruta.
/// </remarks>
public class BCryptHasher : IHashService
{
    /// <summary>
    /// Genera un hash BCrypt a partir de una cadena de texto de entrada.
    /// </summary>
    /// <param name="input">Cadena de texto original a hashear.</param>
    /// <returns>El hash BCrypt generado a partir de la cadena de entrada.</returns>
    public string Hash(string input) =>
        BCrypt.Net.BCrypt.EnhancedHashPassword(input);

    /// <summary>
    /// Verifica si una cadena de texto coincide con un hash BCrypt dado.
    /// </summary>
    /// <param name="input">Cadena de texto original a verificar.</param>
    /// <param name="hash">Hash BCrypt contra el cual se compara la cadena de texto.</param>
    /// <returns>
    /// true si la cadena de texto coincide con el hash BCrypt proporcionado; false en caso contrario.
    /// </returns>
    public bool Matches(string input, string hash) =>
        BCrypt.Net.BCrypt.EnhancedVerify(input, hash);
}
