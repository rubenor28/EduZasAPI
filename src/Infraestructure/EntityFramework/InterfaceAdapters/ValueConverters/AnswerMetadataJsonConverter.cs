using System.Text.Json;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EntityFramework.InterfaceAdapters.ValueConverters;

public class AnswerMetadataJsonConverter : ValueConverter<AnswerMetadata, string>
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public AnswerMetadataJsonConverter()
        : base(
            v => JsonSerializer.Serialize(v, _jsonOptions),
            v => JsonSerializer.Deserialize<AnswerMetadata>(v, _jsonOptions) ?? new AnswerMetadata()
        ) { }
}
