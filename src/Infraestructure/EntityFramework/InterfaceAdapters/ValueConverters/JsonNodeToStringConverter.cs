using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.InterfaceAdapters.ValueConverters;

/// <summary>
/// Convertidor de valor para JsonNode a string.
/// </summary>
public class JsonNodeToStringConverter : ValueConverter<JsonNode, string>
{
    /// <summary>
    /// Inicializa el convertidor.
    /// </summary>
    public JsonNodeToStringConverter()
        : base(v => v.ToJsonString(), v => JsonNode.Parse(v)!) { }
}
