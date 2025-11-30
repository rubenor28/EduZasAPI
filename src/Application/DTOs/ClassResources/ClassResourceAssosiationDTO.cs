namespace Application.DTOs.ClassResources;

public sealed record ClassResourceAssosiationDTO
{
    public required Guid ResourceId { get; set; }
    public required string ClassId { get; set; }
    public required string ClassName { get; set; }
    public bool IsAssosiated { get; set; }
}
