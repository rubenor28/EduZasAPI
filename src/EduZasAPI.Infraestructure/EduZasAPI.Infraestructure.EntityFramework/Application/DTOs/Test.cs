namespace EduZasAPI.Infraestructure.Application.DTOs;

public partial class TestEF
{
    public ulong TestId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public uint? TimeLimitMinutes { get; set; }

    public ulong ProfessorId { get; set; }

    public virtual UserEF Professor { get; set; } = null!;

    public virtual ICollection<TestsPerClassEF> TestsPerClasses { get; set; } = new List<TestsPerClassEF>();
}
