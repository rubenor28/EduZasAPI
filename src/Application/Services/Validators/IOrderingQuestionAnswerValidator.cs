using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;

namespace Application.Services.Validators;

/// <summary>
/// Define un servicio de validación de negocio para respuestas a preguntas de ordenamiento.
/// </summary>
public interface IOrderingQuestionAnswerValidator
    : IBusinessValidationService<(OrderingQuestionAnswer, OrderingQuestion)>;
