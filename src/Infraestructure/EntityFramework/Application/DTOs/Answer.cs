namespace EntityFramework.Application.DTOs;

public partial class Answer
{
    public ulong AnswerId { get; set; }
    public string Content { get; set; } = null!;
    public ulong UserId { get; set; }
    public ulong TestId { get; set; }
    public string ClassId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual TestPerClass TestPerClass { get; set; } = null!;
}
