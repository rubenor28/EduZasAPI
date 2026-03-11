using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

/// <summary>
/// Excepción que se lanza cuando un argumento es nulo de forma inesperada.
/// </summary>
public class NullException(string? message = null) : Exception(message)
{
    /// <summary>
    /// Lanza una <see cref="NullException"/> si el argumento proporcionado es nulo.
    /// </summary>
    /// <typeparam name="T">El tipo del argumento.</typeparam>
    /// <param name="arg">El argumento a verificar.</param>
    /// <param name="message">El nombre del argumento que se captura automáticamente.</param>
    /// <exception cref="NullException">Se lanza si <paramref name="arg"/> es nulo.</exception>
    public static void ThrowIfNull<T>(
        [NotNull] T? arg,
        [CallerArgumentExpression(nameof(arg))] string? message = null
    )
    {
        if (arg is null)
            throw new NullException(message);
    }
}
