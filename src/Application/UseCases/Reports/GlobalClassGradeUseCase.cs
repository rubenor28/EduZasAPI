using Application.DAOs;
using Application.DTOs.Answers;
using Application.DTOs.Tests;
using Application.DTOs.Users;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Domain.ValueObjects.Grades;

namespace Application.UseCases.Reports;

using IAnswerQuerier = IQuerierAsync<AnswerDomain, AnswerCriteriaDTO>;
using IClassReader = IReaderAsync<string, ClassDomain>;
using IProfessorReader = IReaderAsync<UserClassRelationId, ClassProfessorDomain>;
using ITestQuerier = IQuerierAsync<TestDomain, TestCriteriaDTO>;
using IUserQuerier = IQuerierAsync<UserDomain, UserCriteriaDTO>;

/// <summary>
/// Representa las puntuaciones de un estudiante en múltiples evaluaciones.
/// </summary>
public record StudentTestScores
{
    /// <summary>
    /// ID del estudiante.
    /// </summary>
    public required ulong StudentId { get; init; }
    /// <summary>
    /// Correo electrónico del estudiante.
    /// </summary>
    public required string Email { get; init; }
    /// <summary>
    /// Nombre completo del estudiante.
    /// </summary>
    public required string StudentName { get; init; }
    /// <summary>
    /// Diccionario donde la clave es el ID de la evaluación y el valor es la puntuación obtenida.
    /// </summary>
    public required IDictionary<Guid, double?> Scores { get; init; }
}

/// <summary>
/// Representa un reporte de calificaciones tabular para una clase, mostrando las puntuaciones de los estudiantes en diferentes evaluaciones.
/// </summary>
public record TabularClassGradeReport
{
    /// <summary>
    /// ID de la clase.
    /// </summary>
    public required string ClassId { get; init; }
    /// <summary>
    /// Nombre de la clase.
    /// </summary>
    public required string ClassName { get; init; }
    /// <summary>
    /// Diccionario donde la clave es el ID de la evaluación y el valor es el título de la evaluación.
    /// </summary>
    public required IDictionary<Guid, string> TestTitles { get; init; }
    /// <summary>
    /// Colección de puntuaciones de los estudiantes en las evaluaciones.
    /// </summary>
    public required IEnumerable<StudentTestScores> StudentScores { get; init; }
    /// <summary>
    /// Colección de errores de calificación individuales, agrupados por ID de evaluación.
    /// </summary>
    public required ILookup<Guid, IndividualGradeError> Errors { get; init; }
}

/// <summary>
/// Caso de uso para generar un reporte de calificaciones global para una clase, incluyendo puntuaciones de todos los estudiantes en todas las evaluaciones.
/// </summary>
public class GlobalClassGradeUseCase(
    IProfessorReader professorReader,
    IAnswerQuerier answerQuerier,
    AnswerGrader answerGrader,
    IClassReader classReader,
    ITestQuerier testQuerier,
    IUserQuerier userQuerier
) : IUseCaseAsync<string, TabularClassGradeReport>
{
    private readonly IClassReader _classReader = classReader;
    private readonly ITestQuerier _testQuerier = testQuerier;
    private readonly IUserQuerier _userQuerier = userQuerier;
    private readonly AnswerGrader _answerGrader = answerGrader;
    private readonly IAnswerQuerier _answerQuerier = answerQuerier;
    private readonly IProfessorReader _professorReader = professorReader;

    private record GradeInfo(ulong StudentId, Guid TestId, double Score);
    private record ErrorInfo(Guid TestId, IndividualGradeError Error);

    public async Task<Result<TabularClassGradeReport, UseCaseError>> ExecuteAsync(
        UserActionDTO<string> req
    )
    {
        var cls = await _classReader.GetAsync(req.Data);

        if (cls is null)
            return UseCaseErrors.NotFound();

        var authorized = req.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(req),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var allTests = await GetAllPaginatedResults(
            _testQuerier,
            new TestCriteriaDTO { AssignedInClass = cls.Id }
        );

        var allGrades = new List<GradeInfo>();
                var allErrors = new List<ErrorInfo>();
        
                foreach (var test in allTests)
                {
                    var answers = await GetAllPaginatedResults(
                        _answerQuerier,
                        new AnswerCriteriaDTO { ClassId = cls.Id, TestId = test.Id }
                    );
        
                    if (!answers.Any()) continue;
        
                    var gradeResults = await _answerGrader.SimpleGradeManyAsync(answers, test);
        
                    foreach (var result in gradeResults)
                    {
                        if (result.IsOk)
                        {
                            var grade = result.Unwrap();
                            allGrades.Add(new GradeInfo(grade.StudentId, test.Id, Math.Round((double)grade.Points / grade.TotalPoints * 100, 2)));
                        }
                        else
                        {
                            allErrors.Add(new ErrorInfo(test.Id, result.UnwrapErr()));
                        }
                    }
                }
        
                var allClassStudents = await GetAllPaginatedResults(_userQuerier, new UserCriteriaDTO { EnrolledInClass = cls.Id });
        
                var gradesByStudent = allGrades.ToLookup(g => g.StudentId);
                var errorsByTest = allErrors.ToLookup(e => e.TestId, e => e.Error);
        
                var studentScores = allClassStudents.Select(student =>
                {
                    var scoresForStudent = gradesByStudent[student.Id];
                    var testScores = allTests.ToDictionary(
                        t => t.Id,
                        t => scoresForStudent.FirstOrDefault(s => s.TestId == t.Id)?.Score
                    );
        
                    return new StudentTestScores
                    {
                        StudentId = student.Id,
                        Email = student.Email,
                        StudentName = student.FullName,
                        Scores = testScores
                    };
                }).ToList();
                return new TabularClassGradeReport
        {
            ClassId = cls.Id,
            ClassName = cls.ClassName,
            TestTitles = allTests.ToDictionary(t => t.Id, t => t.Title),
            StudentScores = studentScores,
            Errors = errorsByTest
        };
    }

    private async Task<bool> IsProfessorAuthorized(UserActionDTO<string> req)
    {
        var professor = await _professorReader.GetAsync(
            new() { ClassId = req.Data, UserId = req.Executor.Id }
        );

        return professor is not null;
    }

    private static async Task<List<T>> GetAllPaginatedResults<T, TParam>(
        IQuerierAsync<T, TParam> querier,
        TParam criteria
    )
        where T : notnull
        where TParam : CriteriaDTO
    {
        var allResults = new List<T>();
        var currentCriteria = criteria with { Page = 1 };

        while (true)
        {
            var searchResult = await querier.GetByAsync(currentCriteria);
            allResults.AddRange(searchResult.Results);

            if (searchResult.Page >= searchResult.TotalPages)
                break;

            currentCriteria = currentCriteria with { Page = currentCriteria.Page + 1 };
        }

        return allResults;
    }
}
