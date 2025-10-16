using EduZasAPI.Application.Common;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Users;

namespace EduZasAPI.Application.Classes;

public class DeleteClassUseCase(
    IDeleterAsync<string, ClassDomain> deleter,
    IReaderAsync<string, ClassDomain> reader,
    IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> relationReader
) : DeleteUseCase<string, DeleteClassDTO, ClassDomain>(deleter)
{
    protected override async Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(
        DeleteClassDTO value
    )
    {
        if (value.Executor.Role == UserType.STUDENT)
            return Result.Err(UseCaseError.UnauthorizedError());

        var classSearch = await reader.GetAsync(value.Id);

        if (classSearch.IsNone)
            return Result.Err(UseCaseError.NotFound());

        var c = classSearch.Unwrap();

        if (value.Executor.Role != UserType.ADMIN)
        {
            var relation = await relationReader.GetAsync(
                new ClassUserRelationIdDTO { ClassId = c.Id, UserId = value.Executor.Id }
            );

            if (relation.IsNone || relation.Unwrap().IsOwner == false)
                return Result.Err(UseCaseError.UnauthorizedError());
        }

        return Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value);
    }
}
