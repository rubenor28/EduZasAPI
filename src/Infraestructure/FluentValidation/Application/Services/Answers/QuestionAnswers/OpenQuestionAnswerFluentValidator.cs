using Application.Services.Validators;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using FluentValidation;
using FluentValidation.Results;
using FluentValidationProj.Application.Services.Common;

namespace FluentValidationProj.Application.Services.Answers.QuestionAnswers;

public class OpenQuestionAnswerFluentValidator
    : FluentValidator<(OpenQuestionAnswer, OpenQuestion)>,
        IOpenQuestionAnswerValidator
{
    public OpenQuestionAnswerFluentValidator()
    {
        RuleFor(answer => answer.Item1.Text)
            .Custom(
                (txt, ctx) =>
                {
                    if (txt is null || txt != string.Empty)
                        return;

                        Console.WriteLine($"[OpenQuestionAnswerFluentValidator] Valor pregunta abierta {txt}");
                    var field = "Text";
                    var error = "Campo requerido";
                    ctx.AddFailure(new ValidationFailure(field, error));
                }
            );
    }
}
