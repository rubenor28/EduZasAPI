using Application.DAOs;
using Application.DTOs.Classes;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Classes;

/// <summary>
/// Caso de uso para actualizar datos de una clase.
/// </summary>
public class UpdateClassUseCase(
    IUpdaterAsync<ClassDomain, ClassUpdateDTO> updater,
    IReaderAsync<string, ClassDomain> reader,
    IBusinessValidationService<ClassUpdateDTO> validator,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> professorReader
) : UpdateUseCase<string, ClassUpdateDTO, ClassDomain>(updater, reader, validator)
{
    private readonly IReaderAsync<UserClassRelationId, ClassProfessorDomain> _professorReader =
        professorReader;

    /// <inheritdoc/>
    protected async override Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<ClassUpdateDTO> value,
        ClassDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(value.Executor.Id, value.Data.Id),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        return Result<Unit, UseCaseError>.Ok(Unit.Value);
    }

    private async Task<bool> IsProfessorAuthorized(ulong professorId, string classId)
    {
        var professorSearch = await _professorReader.GetAsync(
            new() { UserId = professorId, ClassId = classId }
        );

        return professorSearch is not null && professorSearch.IsOwner;
    }

    /// <inheritdoc/>
    protected override string GetId(ClassUpdateDTO dto) => dto.Id;
}
