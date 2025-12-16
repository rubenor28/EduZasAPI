using System.Text.Json;
using Domain.Entities.Questions;
using InterfaceAdapters.Mappers.Tests; // Donde vive tu IQuestionJsonConverter
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.InterfaceAdapters.ValueConverters;

public class QuestionsDictionaryValueConverter : ValueConverter<IDictionary<Guid, IQuestion>, string>
{
    public QuestionsDictionaryValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            v => JsonSerializer.Serialize(v, GetSerializerOptions()),
            v => JsonSerializer.Deserialize<IDictionary<Guid, IQuestion>>(v, GetSerializerOptions())!,
            mappingHints)
    {
    }

    private static JsonSerializerOptions GetSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new IQuestionJsonConverter() }
        };
    }
}
