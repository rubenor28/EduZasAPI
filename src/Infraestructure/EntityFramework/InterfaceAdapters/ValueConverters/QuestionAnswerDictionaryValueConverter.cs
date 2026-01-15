using System.Text.Json;
using Domain.Entities.QuestionAnswers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.InterfaceAdapters.ValueConverters;

public class QuestionAnswerDictionaryValueConverter(ConverterMappingHints? mappingHints = null)
    : ValueConverter<IDictionary<Guid, IQuestionAnswer>, string>(
        v => JsonSerializer.Serialize(v, GetSerializerOptions()),
        v =>
            JsonSerializer.Deserialize<IDictionary<Guid, IQuestionAnswer>>(
                v,
                GetSerializerOptions()
            )!,
        mappingHints
    )
{
    public static JsonSerializerOptions GetSerializerOptions() =>
        new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new IQuestionAnswerJsonConverter() },
        };
}
