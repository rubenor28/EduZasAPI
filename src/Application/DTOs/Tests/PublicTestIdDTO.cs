/// <summary>
/// Representa un identificador público para una evaluación dentro de una clase.
/// </summary>
public sealed record PublicTestIdDTO
{
    /// <summary>
    /// Obtiene o establece el identificador único de la evaluación.
    /// </summary>
    public required Guid TestId { get; init; }
    /// <summary>
    /// Obtiene o establece el identificador único de la clase a la que está asignada la evaluación.
    /// </summary>
    public required string ClassId { get; init; }
}
