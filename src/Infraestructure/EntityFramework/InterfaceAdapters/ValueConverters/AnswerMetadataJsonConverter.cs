using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Domain.Entities;
using System.Text.Json.Serialization;

namespace EntityFramework.InterfaceAdapters.ValueConverters;

public class AnswerMetadataJsonConverter : ValueConverter<AnswerMetadata, string>
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
    };

    public AnswerMetadataJsonConverter()
        : base(
            v => JsonSerializer.Serialize(v, _jsonOptions),
            v => JsonSerializer.Deserialize<AnswerMetadata>(v, _jsonOptions)
                ?? new AnswerMetadata { ManualMarkAsCorrect = new HashSet<Guid>() }
        ) { }
}
