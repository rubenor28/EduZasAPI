using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;

namespace Application.Services.Validators;

public interface IMultipleSelectionQuestionAnswerValidator
    : IBusinessValidationService<(MultipleSelectionQuestionAnswer, MultipleSelectionQuestion)>;
