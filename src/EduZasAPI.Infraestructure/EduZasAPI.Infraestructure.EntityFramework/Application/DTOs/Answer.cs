namespace EduZasAPI.Infraestructure.EntityFramework.Application.Agenda;

public partial class Answer
{
    public ulong AnswerId { get; set; }
    public string Content { get; set; } = null!;
    public ulong UserId { get; set; }
    public ulong TestClassRelation { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
