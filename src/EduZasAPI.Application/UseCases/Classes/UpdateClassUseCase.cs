using EduZasAPI.Domain.Common;
using EduZasAPI.Domain.Classes;
using EduZasAPI.Application.Common;

namespace EduZasAPI.Application.Classes;

public class UpdateClassUseCase : UpdateUseCase<ClassUpdateDTO, ClassDomain>
{
    private IQuerierAsync<ProfessorClassRelationDTO, ProfessorClassRelationCriteriaDTO> _professorClassQuerier;

    public UpdateClassUseCase(
        IUpdaterAsync<ClassDomain, ClassUpdateDTO> updater,
        IBusinessValidationService<ClassUpdateDTO> validator,
        IQuerierAsync<ProfessorClassRelationDTO, ProfessorClassRelationCriteriaDTO> professorClassQuerier) :
      base(updater, validator)
    {
        _professorClassQuerier = professorClassQuerier;
    }

    protected async override Task<Result<Unit, UseCaseErrorImpl>> ExtraValidationAsync(ClassUpdateDTO value)
    {
        var result = await _professorClassQuerier.GetByAsync(new ProfessorClassRelationCriteriaDTO
        {
            Page = 1,
            UserId = Optional<ulong>.Some(value.Professor),
            ClassId = value.Id.ToOptional(),
        });

        if (result.Results.Count == 0 || !result.Results[0].IsOwner)
            return Result.Err(UseCaseError.UnauthorizedError());

        return Result<Unit, UseCaseErrorImpl>.Ok(Unit.Value);
    }
}
