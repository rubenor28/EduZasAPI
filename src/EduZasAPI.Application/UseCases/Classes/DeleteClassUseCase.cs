using EduZasAPI.Domain.Users;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Common;
using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Classes;

public class DeleteClassUseCase : DeleteUseCase<string, DeleteClassDTO, ClassDomain>
{
    private IReaderAsync<string, ClassDomain> _reader;
    private IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> _relationReader;

    public DeleteClassUseCase(
        IDeleterAsync<string, ClassDomain> deleter,
        IReaderAsync<string, ClassDomain> reader,
        IReaderAsync<ClassUserRelationIdDTO, ProfessorClassRelationDTO> classRelationReader) : base(deleter)
    {
        _reader = reader;
        _relationReader = classRelationReader;
    }

    protected async override Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(DeleteClassDTO value)
    {
        if (value.Executor.Role == UserType.STUDENT)
            return Result.Err(UseCaseError.UnauthorizedError());

        var classSearch = await _reader.GetAsync(value.Id);

        if (classSearch.IsNone)
            return Result.Err(UseCaseError.NotFound());

        var c = classSearch.Unwrap();

        if (value.Executor.Role != UserType.ADMIN)
        {
            var relation = await _relationReader.GetAsync(new ClassUserRelationIdDTO
            {
                ClassId = c.Id,
                UserId = value.Executor.Id

            });

            if (relation.IsNone && relation.Unwrap().IsOwner == false)
                return Result.Err(UseCaseError.UnauthorizedError());
        }

        return Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value);
    }
}
