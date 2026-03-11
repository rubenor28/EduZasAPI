namespace Domain.ValueObjects.Reports;

/// <summary>
/// Respuesta del reporte de uso de un recurso en una clase.
/// </summary>
/// <param name="ResourceId">ID del recurso.</param>
/// <param name="ResourceTitle">Título del recurso.</param>
/// <param name="ClassId">ID de la clase.</param>
/// <param name="Summary">Métricas resumidas del uso del recurso.</param>
/// <param name="Students">Detalle de la actividad de cada estudiante.</param>
public record ResourceClassReportResponse(
    Guid ResourceId,
    string ResourceTitle,
    string ClassId,
    ResourceMetrics Summary,
    List<StudentActivityDetail> Students
);

/// <summary>
/// Métricas resumidas del uso de un recurso.
/// </summary>
/// <param name="TotalViews">Número total de visualizaciones.</param>
/// <param name="UniqueStudentsCount">Número de estudiantes únicos que vieron el recurso.</param>
/// <param name="AverageDurationMinutes">Duración promedio de visualización en minutos.</param>
/// <param name="TotalTimeSpentMinutes">Tiempo total de visualización en minutos.</param>
public record ResourceMetrics(
    int TotalViews,
    int UniqueStudentsCount,
    double AverageDurationMinutes,
    double TotalTimeSpentMinutes
);

/// <summary>
/// Detalle de la actividad de un estudiante en un recurso.
/// </summary>
/// <param name="UserId">ID del usuario.</param>
/// <param name="FullName">Nombre completo del estudiante.</param>
/// <param name="ViewCount">Número de veces que el estudiante vio el recurso.</param>
/// <param name="TotalMinutesSpent">Minutos totales que el estudiante pasó en el recurso.</param>
/// <param name="LastViewed">Fecha de la última visualización.</param>
public record StudentActivityDetail(
    ulong UserId,
    string FullName,
    int ViewCount,
    double TotalMinutesSpent,
    DateTimeOffset LastViewed
);
