namespace Application.Services;

/// <summary>
/// Contrato para servicios de hashing de datos sensibles (ej. contraseñas).
/// </summary>
public interface IHashService
{
    /// <summary>
    /// Genera un hash a partir de una cadena de texto.
    /// </summary>
    /// <param name="input">Texto a hashear.</param>
    /// <returns>Hash generado.</returns>
    string Hash(string input);

    /// <summary>
    /// Verifica si un texto coincide con un hash dado.
    /// </summary>
    /// <param name="input">Texto a verificar.</param>
    /// <param name="hash">Hash de comparación.</param>
    /// <returns>True si coinciden, False en caso contrario.</returns>
    bool Matches(string input, string hash);
}
