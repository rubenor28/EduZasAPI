using EduZasAPI.Infraestructure.EntityFramework.Application.Users;
using EduZasAPI.Infraestructure.EntityFramework.Application.TestsPerClass;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.Tests;

public partial class Test
{
    public ulong TestId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public uint? TimeLimitMinutes { get; set; }

    public ulong ProfessorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public virtual User Professor { get; set; } = null!;

    public virtual ICollection<TestPerClass> TestsPerClasses { get; set; } = new List<TestPerClass>();
}
