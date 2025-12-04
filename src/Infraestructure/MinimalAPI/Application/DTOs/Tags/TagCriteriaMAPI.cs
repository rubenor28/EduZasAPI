using Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Application.DTOs.Tags;

public sealed record TagCriteriaMAPI : CriteriaDTO
{
    public StringQueryMAPI? Text { get; init; }
    public ulong? ContactId { get; init; }
    public ulong? AgendaOwnerId { get; init; }
}
