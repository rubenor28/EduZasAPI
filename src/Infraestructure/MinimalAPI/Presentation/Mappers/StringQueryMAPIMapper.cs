using Application.DTOs.Common;
using Domain.Enums;
using Domain.Extensions;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Presentation.Mappers;

public class StringQueryMAPIMapper(
    IBidirectionalResultMapper<string, StringSearchType, Unit> stypeMapper
)
    : IBidirectionalResultMapper<StringQueryMAPI, StringQueryDTO, Unit>,
        IMapper<StringQueryDTO, StringQueryMAPI>
{
    private readonly IBidirectionalResultMapper<string, StringSearchType, Unit> _stypeMapper =
        stypeMapper;

    public Result<StringQueryDTO, Unit> Map(StringQueryMAPI input) =>
        _stypeMapper
            .Map(input.SearchType)
            .Match<Result<StringQueryDTO, Unit>>(
                stype => new StringQueryDTO { SearchType = stype, Text = input.Text },
                _ => Unit.Value
            );

    public StringQueryMAPI Map(StringQueryDTO input) => MapFrom(input);

    public StringQueryMAPI MapFrom(StringQueryDTO input) =>
        new() { Text = input.Text, SearchType = _stypeMapper.MapFrom(input.SearchType) };
}

public class OptionalStringQueryMAPIMapper(
    IBidirectionalResultMapper<StringQueryMAPI, StringQueryDTO, Unit> strqMapper
)
    : IBidirectionalResultMapper<StringQueryMAPI?, StringQueryDTO?, Unit>,
        IMapper<StringQueryDTO?, StringQueryMAPI?>
{
    private readonly IBidirectionalResultMapper<StringQueryMAPI, StringQueryDTO, Unit> _strqMapper =
        strqMapper;

    public Result<StringQueryDTO?, Unit> Map(StringQueryMAPI? input)
    {
        if (input is null)
            return Result.Ok<StringQueryDTO?>(null);

        return _strqMapper
            .Map(input)
            .Match<Result<StringQueryDTO?, Unit>>(input => input, _ => Unit.Value);
    }

    public StringQueryMAPI? Map(StringQueryDTO? input) => MapFrom(input);

    public StringQueryMAPI? MapFrom(StringQueryDTO? input) =>
        input.AndThen(value => _strqMapper.MapFrom(value));
}
