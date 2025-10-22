namespace MinimalAPI.Application.DTOs.Classes;

public sealed record class AddProfessorToClassMAPI
{
    public required string ClassId { get; set; }
    public required ulong UserId { get; set; }
    public required bool IsOwner { get; set; }
}
