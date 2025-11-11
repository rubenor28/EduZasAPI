using Application.DTOs.Common;
using Domain.Enums;
using Domain.ValueObjects;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Presentation.Mappers;

public class StringQueryMAPIMapper(
    IMapper<string, Result<StringSearchType, Unit>> searchTypeToDomainMapper,
    IMapper<StringSearchType, string> searchTypeFromDomainMapper
)
    : IMapper<StringQueryMAPI, Result<StringQueryDTO, Unit>>,
        IMapper<StringQueryDTO, StringQueryMAPI>,
        IMapper<Optional<StringQueryDTO>, StringQueryMAPI?>,
        IMapper<StringQueryMAPI?, Result<Optional<StringQueryDTO>, Unit>>
{
    private readonly IMapper<string, Result<StringSearchType, Unit>> _searchTypeToDomainMapper =
        searchTypeToDomainMapper;

    private readonly IMapper<StringSearchType, string> _searchTypeFromDomainMapper =
        searchTypeFromDomainMapper;

    public Result<StringQueryDTO, Unit> Map(StringQueryMAPI source)
    {
        ushort errors = 0;

        var searchType = _searchTypeToDomainMapper.Map(source.SearchType);
        searchType.IfErr((_) => errors++);

        if (errors > 0)
            return Unit.Value;

        return new StringQueryDTO { SearchType = searchType.Unwrap(), Text = source.Text };
    }

    public StringQueryMAPI Map(StringQueryDTO source) =>
        new()
        {
            SearchType = _searchTypeFromDomainMapper.Map(source.SearchType),
            Text = source.Text,
        };

    public StringQueryMAPI? Map(Optional<StringQueryDTO> input) =>
        input.Match<StringQueryMAPI?>((strQuery) => Map(strQuery), () => null);

    Result<Optional<StringQueryDTO>, Unit> IMapper<
        StringQueryMAPI?,
        Result<Optional<StringQueryDTO>, Unit>
    >.Map(StringQueryMAPI? input)
    {
        if (input is null)
            return Optional<StringQueryDTO>.None();

        var parse = Map(input);
        if (parse.IsErr)
            return Unit.Value;

        return Optional<StringQueryDTO>.Some(parse.Unwrap());
    }
}
