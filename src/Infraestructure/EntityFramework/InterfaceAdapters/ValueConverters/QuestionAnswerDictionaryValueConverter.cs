using System.Text.Json;
using Domain.Entities.QuestionAnswers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.InterfaceAdapters.ValueConverters;

public class QuestionAnswerDictionaryValueConverter(ConverterMappingHints? mappingHints = null)
    : ValueConverter<IDictionary<Guid, IQuestionAnswer>, string>(
        v => JsonSerializer.Serialize(v, _jsonOptions),
        v =>
            JsonSerializer.Deserialize<IDictionary<Guid, IQuestionAnswer>>(v, _jsonOptions)
            ?? new Dictionary<Guid, IQuestionAnswer>(),
        mappingHints
    )
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new IQuestionAnswerJsonConverter() },
    };
}
