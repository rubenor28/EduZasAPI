using Application.DTOs.Common;
using Domain.ValueObjects;

namespace InterfaceAdapters.Mappers.Common;

public interface ITryMapper<TInput, TOutput>
    where TOutput : notnull
{
    public Result<TOutput, IEnumerable<FieldErrorDTO>> Map(TInput input);
}
