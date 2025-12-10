namespace Application.DTOs.Classes;

public record StudentClassesSummaryDTO(
    string ClassId,
    bool Active,
    string ClassName,
    string? Subject,
    string? Section,
    string Color,
    bool Hidden
);
