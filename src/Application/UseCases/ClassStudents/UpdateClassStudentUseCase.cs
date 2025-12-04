using Application.DAOs;
using Application.DTOs;
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
    protected override Result<Unit, UseCaseError> ExtraValidation(
        UserActionDTO<ClassStudentUpdateDTO> value,
        ClassStudentDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            _ => value.Executor.Id == value.Data.UserId,
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }

    protected override UserClassRelationId GetId(ClassStudentUpdateDTO dto) =>
        new() { UserId = dto.UserId, ClassId = dto.ClassId };
}
