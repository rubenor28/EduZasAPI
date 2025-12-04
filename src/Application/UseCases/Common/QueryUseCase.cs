using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.Services;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

public class QueryUseCase<C, E>(
    IQuerierAsync<E, C> querier,
    IBusinessValidationService<C>? validator = null
) : IUseCaseAsync<C, PaginatedQuery<E, C>>
    where C : notnull, CriteriaDTO
    where E : notnull
{
    protected readonly IQuerierAsync<E, C> _querier = querier;
    protected readonly IBusinessValidationService<C>? _validator = validator;

    public async Task<Result<PaginatedQuery<E, C>, UseCaseError>> ExecuteAsync(
        UserActionDTO<C> request
    )
    {
        List<FieldErrorDTO> errors = [];

        if (request.Data.Page < 1)
            errors.Add(new() { Field = "page", Message = "Formato invalido" });

        if (_validator is not null)
        {
            var result = _validator.IsValid(request.Data);
            if (result.IsErr)
            {
                var errs = result.UnwrapErr();
                errors.AddRange(errs);
            }
        }

        if (errors.Count > 0)
            return UseCaseErrors.Input(errors);

        var validation = ExtraValidation(request);
        if (validation.IsErr)
            return validation.UnwrapErr();

        var validationAsync = await ExtraValidationAsync(request);
        if (validationAsync.IsErr)
            return validationAsync.UnwrapErr();

        PrevFormat(ref request);
        await PrevFormatAsync(ref request);

        return await _querier.GetByAsync(request.Data);
    }

    protected virtual Result<Unit, UseCaseError> ExtraValidation(UserActionDTO<C> criteria) =>
        Unit.Value;

    protected virtual Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<C> criteria
    ) => Task.FromResult(Result<Unit, UseCaseError>.Ok(Unit.Value));

    protected virtual void PrevFormat(ref UserActionDTO<C> criteria) { }

    protected virtual Task PrevFormatAsync(ref UserActionDTO<C> criteria) => Task.CompletedTask;
}
