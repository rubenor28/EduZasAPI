using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.InterfaceAdapters.ValueConverters;

/// <summary>
/// Convertidor de valor para JsonElement a string.
/// </summary>
public class JsonElementToStringConverter : ValueConverter<JsonElement, string>
{
    /// <summary>
    /// Inicializa el convertidor.
    /// </summary>
    public JsonElementToStringConverter() : base(
        v => v.ToString() ?? "{}",
        v => JsonDocument.Parse(v).RootElement)
    {
    }
}
