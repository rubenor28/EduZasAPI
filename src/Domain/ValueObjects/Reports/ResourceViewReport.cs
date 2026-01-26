namespace Domain.ValueObjects.Reports;

public record ResourceClassReportResponse(
    Guid ResourceId,
    string ResourceTitle,
    string ClassId,
    ResourceMetrics Summary,
    List<StudentActivityDetail> Students
);

public record ResourceMetrics(
    int TotalViews,
    int UniqueStudentsCount,
    double AverageDurationMinutes,
    double TotalTimeSpentMinutes
);

public record StudentActivityDetail(
    ulong UserId,
    string FullName,
    int ViewCount,
    double TotalMinutesSpent,
    DateTimeOffset LastViewed
);
