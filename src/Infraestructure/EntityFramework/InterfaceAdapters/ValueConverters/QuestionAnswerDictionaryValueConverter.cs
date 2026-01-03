using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities.QuestionAnswers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.InterfaceAdapters.ValueConverters;

public class QuestionAnswerDictionaryValueConverter
    : ValueConverter<IDictionary<Guid, IQuestionAnswer>, string>
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        Converters = { new IQuestionAnswerJsonConverter() },
    };

    public QuestionAnswerDictionaryValueConverter()
        : base(
            v => JsonSerializer.Serialize(v, _jsonOptions),
            v =>
                JsonSerializer.Deserialize<IDictionary<Guid, IQuestionAnswer>>(v, _jsonOptions)
                ?? new Dictionary<Guid, IQuestionAnswer>()
        ) { }
}
