using Application.DTOs.Common;
using Domain.Enums;
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
    : IBidirectionalResultMapper<StringQueryMAPI?, Optional<StringQueryDTO>, Unit>,
        IMapper<Optional<StringQueryDTO>, StringQueryMAPI?>
{
    private readonly IBidirectionalResultMapper<StringQueryMAPI, StringQueryDTO, Unit> _strqMapper =
        strqMapper;

    public Result<Optional<StringQueryDTO>, Unit> Map(StringQueryMAPI? input) =>
        input is null
            ? Optional<StringQueryDTO>.None()
            : _strqMapper
                .Map(input)
                .Match<Result<Optional<StringQueryDTO>, Unit>>(
                    input => Optional.Some(input),
                    _ => Unit.Value
                );

    public StringQueryMAPI? Map(Optional<StringQueryDTO> input) => MapFrom(input);

    public StringQueryMAPI? MapFrom(Optional<StringQueryDTO> input) =>
        input.Match<StringQueryMAPI?>(value => _strqMapper.MapFrom(value), () => null);
}
