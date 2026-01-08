using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;

namespace Application.Services.Validators;

public interface IMultipleChoiseQuestionAnswerValidator
    : IBusinessValidationService<(MultipleChoiseQuestionAnswer, MultipleChoiseQuestion)>;
