using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Common;

public class AddUseCase<NE, E>
    where NE : notnull
    where E : notnull
{
    private readonly ICreatorAsync<E, NE> _creator;
    private readonly IBusinessValidationService<NE> _validator;

    public AddUseCase(ICreatorAsync<E, NE> creator, IBusinessValidationService<NE> validator)
    {
        _creator = creator;
        _validator = validator;
    }

    public async Task<Result<E, List<FieldErrorDTO>>> ExecuteAsync(
        NE request,
        Func<NE, NE>? formatData = null,
        Func<NE, Result<Unit, List<FieldErrorDTO>>>? extraValidation = null,
        Func<NE, Task<Result<Unit, List<FieldErrorDTO>>>>? extraValidationAsync = null)
    {
        var formatted = (formatData ?? (x => x))(request);

        var validation = _validator.IsValid(formatted);
        if (validation.IsErr)
            return Result<E, List<FieldErrorDTO>>.Err(validation.UnwrapErr());

        if (extraValidation is not null)
        {
            var syncCheck = extraValidation(formatted);
            if (syncCheck.IsErr)
                return Result<E, List<FieldErrorDTO>>.Err(syncCheck.UnwrapErr());
        }

        if (extraValidationAsync is not null)
        {
            var asyncCheck = await extraValidationAsync(formatted);
            if (asyncCheck.IsErr)
                return Result<E, List<FieldErrorDTO>>.Err(asyncCheck.UnwrapErr());
        }

        var newRecord = await _creator.AddAsync(formatted);
        return Result<E, List<FieldErrorDTO>>.Ok(newRecord);
    }
}
