using Application.DAOs;
using Application.DTOs.ClassStudents;
using Application.Services;
using Application.UseCases.Common;
using Domain.Entities;

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

        if()
    }
}
