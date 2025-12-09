namespace Domain.ValueObjects;

/// <summary>
/// Representa un tipo con un único valor, análogo a `void` en un contexto funcional.
/// </summary>
/// <remarks>
/// Es útil en genéricos o en `Result<T, E>` para indicar la ausencia de un valor significativo.
/// </remarks>
public readonly struct Unit : IEquatable<Unit>
{
    /// <summary>
    /// Obtiene la única instancia del tipo `Unit`.
    /// </summary>
    public static readonly Unit Value = default;

    /// <summary>
    /// Compara esta instancia con otra. Siempre devuelve `true`.
    /// </summary>
    public bool Equals(Unit other) => true;

    /// <summary>
    /// Determina si el objeto especificado es una instancia de `Unit`.
    /// </summary>
    public override bool Equals(object? obj) => obj is Unit;

    /// <summary>
    /// Devuelve el código hash para esta instancia. Siempre es `0`.
    /// </summary>
    public override int GetHashCode() => 0;

    /// <summary>
    /// Devuelve la representación en cadena de `Unit`, que es "()".
    /// </summary>
    public override string ToString() => "()";

    /// <summary>
    /// Compara dos instancias de `Unit` por igualdad.
    /// </summary>
    public static bool operator ==(Unit left, Unit right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Compara dos instancias de `Unit` por desigualdad.
    /// </summary>
    public static bool operator !=(Unit left, Unit right)
    {
        return !(left == right);
    }
}
