using Application.DTOs.Common;
using MinimalAPI.Application.DTOs.Common;

namespace MinimalAPI.Application.DTOs.Tags;

public sealed record TagCriteriaMAPI : CriteriaDTO
{
    public StringQueryMAPI? Text { get; set; }
    public ulong? ContactId { get; set; }
    public ulong? AgendaOwnerId { get; set; }
}
