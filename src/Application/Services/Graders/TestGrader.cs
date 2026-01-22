using Domain.Entities;
using Domain.Entities.QuestionAnswers;
using Domain.Entities.Questions;
using Domain.ValueObjects;
using Domain.ValueObjects.Grades;

namespace Application.Services.Graders;

public class TestGrader
{
    public Result<Score, GradeError> Grade(AnswerDomain answer, TestDomain test)
    {
        var missingManualGrades = test
            .RequiresManualGrade
            .Where(id => !answer.Metadata.ManualGrade.ContainsKey(id))
            .ToList();

        if (missingManualGrades.Any())
            return GradeErrors.MissingManualGrade(missingManualGrades);

        var grades = new List<Grade>();
        foreach (var (id, question) in test.Content)
        {
            var questionAnswer = answer.Content[id];

            answer.Metadata.ManualGrade.TryGetValue(id, out var manualGrade);

            Grade grade = (question, questionAnswer) switch
            {
                (ConceptRelationQuestion q, ConceptRelationQuestionAnswer qa) =>
                    new ConceptRelationGrade
                    {
                        Title = q.Title,
                        Pairs = [.. q.Concepts],
                        AnsweredPairs = [.. qa.AnsweredPairs],
                        ManualGrade = manualGrade
                    },
                (MultipleChoiseQuestion q, MultipleChoiseQuestionAnswer qa) =>
                    new MultipleChoiseGrade
                    {
                        Title = q.Title,
                        Options = q.Options,
                        CorrectOption = q.CorrectOption,
                        SelectedOption = qa.SelectedOption,
                        ManualGrade = manualGrade
                    },
                (MultipleSelectionQuestion q, MultipleSelectionQuestionAnswer qa) =>
                    new MultipleSelectionGrade
                    {
                        Title = q.Title,
                        Options = q.Options,
                        CorrectOptions = q.CorrectOptions,
                        AnsweredOptions = qa.SelectedOptions,
                        ManualGrade = manualGrade
                    },
                (OpenQuestion q, OpenQuestionAnswer qa) =>
                    new OpenGrade { ManualGrade = manualGrade },
                (OrderingQuestion q, OrderingQuestionAnswer qa) =>
                    new OrderingGrade
                    {
                        Title = q.Title,
                        Sequence = q.Sequence,
                        AnsweredSequence = qa.Sequence,
                        ManualGrade = manualGrade
                    },
                (_, _) => throw new InvalidOperationException($"Cannot grade question of type {question.GetType().Name} with answer of type {questionAnswer.GetType().Name}"),
            };
            grades.Add(grade);
        }

        var totalPoints = (uint)grades.Sum(g => g.TotalPoints);
        var earnedPoints = (uint)grades.Sum(g => g.Points);

        return new Score(earnedPoints, totalPoints);
    }
}
