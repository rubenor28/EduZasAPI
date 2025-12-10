namespace Application.DTOs.ClassContent;

public enum ContentType
{
    TEST,
    RESOURCE,
}

public sealed class ClassContentDTO
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required ContentType Type { get; set; }
    public required DateTime PublishDate { get; set; }
}
