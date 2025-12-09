using Application.Services;

namespace Bcrypt.Application.Services;



/// <summary>
/// Implementación del servicio de hashing utilizando BCrypt.
/// </summary>
public class BCryptHasher : IHashService
{
    /// <summary>
    /// Genera un hash BCrypt.
    /// </summary>
    /// <param name="input">Texto a hashear.</param>
    /// <returns>Hash generado.</returns>
    public string Hash(string input) =>
        BCrypt.Net.BCrypt.HashPassword(input);

    /// <summary>
    /// Verifica si el texto coincide con el hash.
    /// </summary>
    /// <param name="input">Texto a verificar.</param>
    /// <param name="hash">Hash contra el cual comparar.</param>
    /// <returns>True si coinciden, false en caso contrario.</returns>
    public bool Matches(string input, string hash) =>
        BCrypt.Net.BCrypt.Verify(input, hash);
}
