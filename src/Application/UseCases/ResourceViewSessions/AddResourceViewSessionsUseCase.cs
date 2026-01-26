using Application.DAOs;
using Application.DTOs.ResourceViewSessions;
using Application.Services.Validators;
using Application.UseCases.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.UseCases.ResourceViewSessions;

using IBusinessValidator = IBusinessValidationService<NewResourceViewSession>;
using IResourceViewCreator = ICreatorAsync<ResourceViewSessionDomain, NewResourceViewSession>;

public class AddResourceViewSessionsUseCase(
    IResourceViewCreator creator,
    IBusinessValidator? validator = null
) : AddUseCase<NewResourceViewSession, ResourceViewSessionDomain>(creator, validator)
{
    protected override async Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<NewResourceViewSession> value
    )
    {
        if (value.Data.UserId != value.Executor.Id)
            return UseCaseErrors.Unauthorized();

        return Unit.Value;
    }
}
