using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassProfessors;

public sealed class UpdateClassProfessorUseCase(
    IUpdaterAsync<ProfessorClassRelationDTO, ClassProfessorUpdateDTO> updater,
    IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> reader,
    IBusinessValidationService<ClassProfessorUpdateDTO>? validator = null
)
    : UpdateUseCase<
        ClassUserRelationIdDTO,
        ClassProfessorUpdateDTO,
        ProfessorClassRelationDTO
    >(updater, reader, validator)
{
    protected override Result<Unit, UseCaseError> ExtraValidation(
        ClassProfessorUpdateDTO value
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => value.Id.UserId == value.Executor.Id,
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }
}
