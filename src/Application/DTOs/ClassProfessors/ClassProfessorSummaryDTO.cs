namespace Application.DTOs.ClassProfessors;

public record ClassProfessorSummaryDTO(
    ulong UserId,
    string Email,
    string? Alias, // Solo habra alias si lo tenemos en contactos
    string FirstName,
    string? MidName,
    string FatherLastName,
    string? MotherLastname,
    bool Owner
);
