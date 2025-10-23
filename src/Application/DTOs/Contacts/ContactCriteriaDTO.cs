using Application.DTOs.Common;
using Domain.ValueObjects;

namespace Application.DTOs.Contacts;

public sealed record ContactCriteriaDTO : CriteriaDTO
{
    public required ulong Id { get; set; }
    public required string Alias { get; set; }
    public required Optional<string> Notes { get; set; }
}
