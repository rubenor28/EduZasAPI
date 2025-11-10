using System.Text.RegularExpressions;
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
    : IMapper<StringQueryMAPI, Result<StringQueryDTO, IEnumerable<FieldErrorDTO>>>,
        IMapper<StringQueryDTO, StringQueryMAPI>,
        IMapper<Optional<StringQueryDTO>, StringQueryMAPI?>,
        IMapper<StringQueryMAPI?, Result<Optional<StringQueryDTO>, IEnumerable<FieldErrorDTO>>>
{
    private readonly IMapper<string, Result<StringSearchType, Unit>> _searchTypeToDomainMapper =
        searchTypeToDomainMapper;

    private readonly IMapper<StringSearchType, string> _searchTypeFromDomainMapper =
        searchTypeFromDomainMapper;

    public Result<StringQueryDTO, IEnumerable<FieldErrorDTO>> Map(StringQueryMAPI source)
    {
        List<FieldErrorDTO> errors = [];

        var searchType = _searchTypeToDomainMapper.Map(source.SearchType);

        if (searchType.IsErr)
            errors.Add(new() { Field = "searchType", Message = "Formato invalido" });

        if (errors.Count > 0)
            return errors;

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

    Result<Optional<StringQueryDTO>, IEnumerable<FieldErrorDTO>> IMapper<
        StringQueryMAPI?,
        Result<Optional<StringQueryDTO>, IEnumerable<FieldErrorDTO>>
    >.Map(StringQueryMAPI? input)
    {
        if (input is null)
            return Optional<StringQueryDTO>.None();

        var parse = Map(input);

        if (parse.IsErr)
            return Result<Optional<StringQueryDTO>, IEnumerable<FieldErrorDTO>>.Err(
                parse.UnwrapErr()
            );

        return Optional<StringQueryDTO>.Some(parse.Unwrap());
    }
}
