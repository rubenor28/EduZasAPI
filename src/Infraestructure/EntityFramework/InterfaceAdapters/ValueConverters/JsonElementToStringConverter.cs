using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.InterfaceAdapters.ValueConverters;

public class JsonElementToStringConverter : ValueConverter<JsonElement, string>
{
    public JsonElementToStringConverter() : base(
        // Función para convertir de JsonElement (modelo) a string (base de datos)
        v => v.ToString() ?? "{}",
        // Función para convertir de string (base de datos) a JsonElement (modelo)
        v => JsonDocument.Parse(v).RootElement)
    {
    }
}
