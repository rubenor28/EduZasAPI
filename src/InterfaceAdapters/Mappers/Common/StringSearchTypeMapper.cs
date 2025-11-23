using Application.DTOs.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace InterfaceAdapters.Mappers.Common;

public class StringSearchMapper
    : IBidirectionalResultMapper<string, StringSearchType, Unit>,
        IMapper<StringSearchType, string>
{
    public string MapFrom(StringSearchType input) =>
        input switch
        {
            StringSearchType.EQ => "EQUALS",
            StringSearchType.LIKE => "LIKE",
            _ => throw new NotImplementedException(),
        };

    public Result<StringSearchType, Unit> Map(string input) =>
        input switch
        {
            "EQUALS" => StringSearchType.EQ,
            "LIKE" => StringSearchType.LIKE,
            _ => Unit.Value,
        };

    public string Map(StringSearchType input) => MapFrom(input);
}
