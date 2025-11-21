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
    IBusinessValidationService<ClassStudentUpdateDTO>? validator = null
)
    : UpdateUseCase<UserClassRelationId, ClassStudentUpdateDTO, ClassStudentDomain>(
        updater,
        reader,
        validator
    )
{
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        ClassStudentUpdateDTO value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => value.Executor.Id == value.UserId,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var student = await _reader.GetAsync(GetId(value));
        if (student.IsNone)
            return UseCaseErrors.NotFound();

        return Unit.Value;
    }

    protected override UserClassRelationId GetId(ClassStudentUpdateDTO dto) =>
        new() { UserId = dto.UserId, ClassId = dto.ClassId };
}
