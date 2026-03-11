namespace Domain.ValueObjects;

/// <summary>
/// Representa una puntuación, indicando los puntos obtenidos frente a los puntos totales.
/// </summary>
/// <param name="Points">Puntos obtenidos.</param>
/// <param name="Total">Puntos totales.</param>
public record Score(uint Points, uint Total);
