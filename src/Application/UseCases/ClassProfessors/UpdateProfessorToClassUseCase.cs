using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Application.Services;
using Application.UseCases.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.ClassProfessors;

public sealed class UpdateProfessorToClassUseCase(
    IUpdaterAsync<ProfessorClassRelationDTO, ProfessorClassRelationUpdateDTO> updater,
    IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> reader,
    IBusinessValidationService<ProfessorClassRelationUpdateDTO>? validator = null
)
    : UpdateUseCase<
        ClassUserRelationIdDTO,
        ProfessorClassRelationUpdateDTO,
        ProfessorClassRelationDTO
    >(updater, reader, validator)
{
    protected override Result<Unit, UseCaseError> ExtraValidation(
        ProfessorClassRelationUpdateDTO value
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
