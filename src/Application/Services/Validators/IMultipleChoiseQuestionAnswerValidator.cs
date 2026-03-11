using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;

namespace Application.Services.Validators;

/// <summary>
/// Define un servicio de validación de negocio para respuestas a preguntas de opción múltiple.
/// </summary>
public interface IMultipleChoiseQuestionAnswerValidator
    : IBusinessValidationService<(MultipleChoiseQuestionAnswer, MultipleChoiseQuestion)>;
