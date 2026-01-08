using Application.Services.Validators;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using FluentValidation;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Answers.QuestionAnswers;

public class OpenQuestionAnswerFluentValidator
    : FluentValidator<(OpenQuestionAnswer, OpenQuestion)>,
        IOpenQuestionAnswerValidator
{
    public OpenQuestionAnswerFluentValidator()
    {
        RuleFor(answer => answer.Item1.Text).NotNull().NotEmpty().WithMessage("Campo requerido");
    }
}
