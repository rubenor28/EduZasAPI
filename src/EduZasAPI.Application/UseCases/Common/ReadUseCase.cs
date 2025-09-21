namespace EduZasAPI.Application.UseCases.Common;

using EduZasAPI.Domain.ValueObjects.Common;
using EduZasAPI.Application.Ports.DAOs;
using EduZasAPI.Application.Ports.Services.Common;
using EduZasAPI.Application.DTOs.Common;

public class ReadUseCase<I, E> : IUseCaseAsync<I, Optional<E>, List<FieldErrorDTO>>
    where I : notnull
    where E : notnull, IIdentifiable<I>
{
    private readonly IReaderAsync<I, E> _reader;
    private readonly IBusinessValidationService<I> _validator;

    public ReadUseCase(IReaderAsync<I, E> reader, IBusinessValidationService<I> validator)
    {
        _reader = reader;
        _validator = validator;
    }

    public async Task<Result<Optional<E>, List<FieldErrorDTO>>> ExecuteAsync(I request)
    {
        var validation = _validator.IsValid(request);

        if (validation.IsErr)
        {
            var errors = validation.UnwrapErr();
            return Result<Optional<E>, List<FieldErrorDTO>>.Err(errors);
        }

        var record = await _reader.GetAsync(request);
        return Result<Optional<E>, List<FieldErrorDTO>>.Ok(record);
    }

}
