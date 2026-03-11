using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;

namespace Application.Services.Validators;

/// <summary>
/// Define un servicio de validación de negocio para respuestas a preguntas de relación de conceptos.
/// </summary>
public interface IConceptRelationQuestionAnswerValidator
    : IBusinessValidationService<(ConceptRelationQuestionAnswer, ConceptRelationQuestion)>;
