using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;

namespace Application.Services.Validators;

public interface IOpenQuestionAnswerValidator
    : IBusinessValidationService<(OpenQuestionAnswer, OpenQuestion)>;
