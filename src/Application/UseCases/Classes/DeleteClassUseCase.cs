using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Classes;

public class DeleteClassUseCase(
    IDeleterAsync<string, ClassDomain> deleter,
    IReaderAsync<string, ClassDomain> reader,
    IReaderAsync<UserClassRelationId, ClassProfessorDomain> relationReader
) : DeleteUseCase<string, ClassDomain>(deleter, reader)
{
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<string> value,
        ClassDomain record
    )
    {
        var authorized = value.Executor.Role switch
        {
            UserType.ADMIN => true,
            UserType.PROFESSOR => await IsProfessorAuthorized(value.Executor.Id, value.Data),
            UserType.STUDENT => false,
            _ => throw new NotImplementedException(),
        };

        if (!authorized)
            return UseCaseErrors.Unauthorized();

        var classSearch = await _reader.GetAsync(value.Data);

        if (classSearch is null)
            return UseCaseErrors.NotFound();

        return Unit.Value;
    }

    private async Task<bool> IsProfessorAuthorized(ulong professorId, string classId)
    {
        var professorSearch = await relationReader.GetAsync(
            new() { UserId = professorId, ClassId = classId }
        );

        return professorSearch is not null && professorSearch.IsOwner;
    }
}
