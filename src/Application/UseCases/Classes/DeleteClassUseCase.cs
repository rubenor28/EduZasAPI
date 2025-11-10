using Application.DAOs;
using Application.DTOs.Classes;
using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.UseCases.Classes;

public class DeleteClassUseCase(
    IDeleterAsync<string, ClassDomain> deleter,
    IReaderAsync<string, ClassDomain> reader,
    IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> relationReader
) : DeleteUseCase<string, DeleteClassDTO, ClassDomain>(deleter, reader)
{
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        DeleteClassDTO value
    )
    {
        if (value.Executor.Role == UserType.STUDENT)
            return UseCaseErrors.Unauthorized();

        var classSearch = await _reader.GetAsync(value.Id);

        if (classSearch.IsNone)
            return UseCaseErrors.NotFound();

        var c = classSearch.Unwrap();

        if (value.Executor.Role != UserType.ADMIN)
        {
            var relation = await relationReader.GetAsync(
                new ClassUserRelationIdDTO { ClassId = c.Id, UserId = value.Executor.Id }
            );

            if (relation.IsNone || relation.Unwrap().IsOwner == false)
                return UseCaseErrors.Unauthorized();
        }

        return Unit.Value;
    }
}
