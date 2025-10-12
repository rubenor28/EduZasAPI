namespace EduZasAPI.Application.Classes;

public record class NewStudent
{
    public required ulong UserId { get; set; }
    public required string ClassId { get; set; }
}
