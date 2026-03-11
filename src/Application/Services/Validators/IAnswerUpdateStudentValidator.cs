using Application.Services.Validators;
using Domain.Entities;

/// <summary>
/// Define un servicio de validación de negocio para la actualización de respuestas de estudiantes.
/// </summary>
public interface IAnswerUpdateStudentValidator
    : IBusinessValidationService<(AnswerUpdateStudentDTO, TestDomain)>;
