namespace Application.DTOs.ClassProfessors;

/// <summary>
/// DTO que representa un resumen de la información de un profesor en una clase.
/// </summary>
/// <param name="UserId">ID del usuario (profesor).</param>
/// <param name="Email">Dirección de correo electrónico del profesor.</param>
/// <param name="Alias">Alias o apodo del profesor en el contexto de la agenda (opcional). Solo habra alias si lo tenemos en contactos</param>
/// <param name="FirstName">Primer nombre del profesor.</param>
/// <param name="MidName">Segundo nombre del profesor (opcional).</param>
/// <param name="FatherLastName">Apellido paterno del profesor.</param>
/// <param name="MotherLastname">Apellido materno del profesor (opcional).</param>
/// <param name="Owner">Indica si el profesor es propietario de la clase.</param>
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
