namespace EntityFramework.Application.DTOs;

public partial class TestPerClass
{
    public ulong TestId { get; set; }

    public string ClassId { get; set; } = null!;

    public bool Visible { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
}
