using Application.DAOs;
using Application.DTOs;
using Application.DTOs.Common;
using Application.Services;
using Domain.ValueObjects;

namespace Application.UseCases.Common;

public abstract class AddUseCase<NE, E>(
    ICreatorAsync<E, NE> creator,
    IBusinessValidationService<NE>? validator = null
) : IUseCaseAsync<NE, E>
    where NE : notnull
    where E : notnull
{
    /// <summary>
    /// Entidad encargada de persistir una nueva entidad
    /// </summary>
    protected readonly ICreatorAsync<E, NE> _creator = creator;

    /// <summary>
    /// Entidad encargada de validar el formato de la entidad a insertar
    /// </summary>
    protected readonly IBusinessValidationService<NE>? _validator = validator;

    public async virtual Task<Result<E, UseCaseError>> ExecuteAsync(UserActionDTO<NE> request)
    {
        var formattedRequest = PreValidationFormat(request);

        if (_validator is not null)
        {
            var validation = _validator.IsValid(formattedRequest.Data);
            if (validation.IsErr)
                return UseCaseErrors.Input(validation.UnwrapErr());
        }

        var syncCheck = ExtraValidation(formattedRequest);
        if (syncCheck.IsErr)
            return syncCheck.UnwrapErr();

        var asyncCheck = await ExtraValidationAsync(formattedRequest);
        if (asyncCheck.IsErr)
            return asyncCheck.UnwrapErr();

        var finalRequest = PostValidationFormat(formattedRequest);
        finalRequest = await PostValidationFormatAsync(finalRequest);

        PrevTask(finalRequest);
        await PrevTaskAsync(finalRequest);

        var newRecord = await _creator.AddAsync(finalRequest.Data);

        ExtraTask(finalRequest, newRecord);
        await ExtraTaskAsync(finalRequest, newRecord);

        return newRecord;
    }

    protected virtual UserActionDTO<NE> PreValidationFormat(UserActionDTO<NE> value) => value;

    protected virtual Task<UserActionDTO<NE>> PostValidationFormatAsync(UserActionDTO<NE> value) =>
        Task.FromResult(value);

    protected virtual UserActionDTO<NE> PostValidationFormat(UserActionDTO<NE> value) => value;

    protected virtual Result<Unit, UseCaseError> ExtraValidation(UserActionDTO<NE> value) =>
        Result<Unit, UseCaseError>.Ok(Unit.Value);

    protected virtual Task<Result<Unit, UseCaseError>> ExtraValidationAsync(
        UserActionDTO<NE> value
    ) => Task.FromResult(Result<Unit, UseCaseError>.Ok(Unit.Value));

    protected virtual void ExtraTask(UserActionDTO<NE> newEntity, E createdEntity) { }

    protected virtual Task ExtraTaskAsync(UserActionDTO<NE> newEntity, E createdEntity) =>
        Task.CompletedTask;

    protected virtual void PrevTask(UserActionDTO<NE> newEntity) { }

    protected virtual Task PrevTaskAsync(UserActionDTO<NE> newEntity) => Task.CompletedTask;
}
