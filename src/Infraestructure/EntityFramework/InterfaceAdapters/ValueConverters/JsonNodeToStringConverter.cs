using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.InterfaceAdapters.ValueConverters;

public class JsonNodeToStringConverter : ValueConverter<JsonNode, string>
{
    public JsonNodeToStringConverter()
        : base(v => v.ToJsonString(), v => JsonNode.Parse(v)!) { }
}
