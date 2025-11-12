using Application.DAOs;
using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassStudents;

public sealed class UpdateClassStudentUseCase(
    IUpdaterAsync<ClassStudentDomain, ClassStudentUpdateDTO> updater,
    IReaderAsync<UserClassRelationId, ClassStudentDomain> reader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> professorReader,
    IBusinessValidationService<ClassStudentUpdateDTO>? validator = null
)
    : UpdateUseCase<UserClassRelationId, ClassStudentUpdateDTO, ClassStudentDomain>(
        updater,
        reader,
        validator
    )
{
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _professorReader =
        professorReader;

    private async Task<bool> IsProfessorAuthorized(ulong professorId, string classId)
    {
        var professor = await _professorReader.GetAsync(
            new() { UserId = professorId, ClassId = classId }
        );

        return professor.IsSome;
    }

    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        ClassStudentUpdateDTO value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(value.Executor.Id, value.Id.ClassId),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var student = await _reader.GetAsync(value.Id);
        if (student.IsNone)
            return UseCaseErrors.NotFound();

        return Unit.Value;
    }
}
