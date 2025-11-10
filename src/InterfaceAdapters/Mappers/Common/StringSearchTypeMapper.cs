using Domain.Enums;
using Domain.ValueObjects;

namespace InterfaceAdapters.Mappers.Common;

public class StringSearchMapper
    : IMapper<StringSearchType, string>,
        IMapper<string, Result<StringSearchType, Unit>>
{
    public Result<StringSearchType, Unit> Map(string value) =>
        value switch
        {
            "EQUALS" => StringSearchType.EQ,
            "LIKE" => StringSearchType.LIKE,
            _ => Unit.Value,
        };

    public string Map(StringSearchType value) =>
        value switch
        {
            StringSearchType.EQ => "EQUALS",
            StringSearchType.LIKE => "LIKE",
            _ => throw new NotImplementedException(),
        };
}
