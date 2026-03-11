namespace Application.Configuration;

/// <summary>
/// Contiene la configuración relacionada con las calificaciones y los umbrales de aprobación.
/// </summary>
public record GradeSettings
{
    /// <summary>
    /// Obtiene el umbral de aprobación como un valor porcentual entre 0 y 1.
    /// </summary>
    /// <value>
    /// Un valor de tipo <see cref="double"/> que representa la calificación mínima para aprobar, expresada como una fracción (ej. 0.7 para 70%).
    /// </value>
    public double PassThresholdPercentage { get; init; }

    /// <summary>
    /// Obtiene el umbral de aprobación como un valor porcentual entre 0 y 100.
    /// </summary>
    /// <value>
    /// Un valor de tipo <see cref="double"/> que representa la calificación mínima para aprobar, expresada como un porcentaje (ej. 70 para 70%).
    /// </value>
    public double PassThreshold => PassThresholdPercentage * 100;
}
