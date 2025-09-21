namespace EduZasAPI.Domain.ValueObjects.Common;

/// <summary>
/// Representa un tipo que tiene un único valor, utilizado para situaciones donde se necesita
/// un tipo pero no se requiere almacenar o transportar información alguna.
/// </summary>
/// <remarks>
/// Esta estructura sigue el concepto de "tipo unit" de la programación funcional, que es
/// un tipo que tiene exactamente un valor. Es útil en escenarios genéricos donde se necesita
/// cumplir con parámetros de tipo pero no se tiene información significativa que devolver
/// o almacenar.
/// </remarks>
public readonly struct Unit : IEquatable<Unit>
{
    /// <summary>
    /// Obtiene la única instancia válida del tipo Unit.
    /// </summary>
    /// <value>Instancia única del tipo Unit.</value>
    public static readonly Unit Value = default;

    /// <summary>
    /// Determina si esta instancia de Unit es igual a otra instancia de Unit.
    /// </summary>
    /// <param name="other">Otra instancia de Unit para comparar.</param>
    /// <returns>Siempre devuelve true, ya que todas las instancias de Unit son iguales.</returns>
    public bool Equals(Unit other) => true;

    /// <summary>
    /// Determina si el objeto especificado es una instancia de Unit.
    /// </summary>
    /// <param name="obj">Objeto a comparar.</param>
    /// <returns>true si el objeto es una instancia de Unit; false en caso contrario.</returns>
    public override bool Equals(object? obj) => obj is Unit;

    /// <summary>
    /// Obtiene el código hash para esta instancia de Unit.
    /// </summary>
    /// <returns>Siempre devuelve 0, ya que todas las instancias de Unit son iguales.</returns>
    public override int GetHashCode() => 0;

    /// <summary>
    /// Devuelve una representación en cadena de esta instancia de Unit.
    /// </summary>
    /// <returns>La cadena "()", que representa el valor unitario.</returns>
    public override string ToString() => "()";
}
