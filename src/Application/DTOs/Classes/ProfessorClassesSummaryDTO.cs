namespace Application.DTOs.Classes;

public record ProfessorClassesSummaryDTO(
    string ClassId,
    bool Active,
    string ClassName,
    string? Subject,
    string? Section,
    string Color,
    bool Owner
);
