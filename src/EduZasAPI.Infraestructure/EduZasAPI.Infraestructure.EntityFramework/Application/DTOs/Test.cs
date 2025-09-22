using System;
using System.Collections.Generic;

namespace EduZasAPI.Infraestructure.Application.DTOs;

public partial class Test
{
    public ulong TestId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public uint? TimeLimitMinutes { get; set; }

    public ulong ProfessorId { get; set; }

    public virtual User Professor { get; set; } = null!;

    public virtual ICollection<TestsPerClass> TestsPerClasses { get; set; } = new List<TestsPerClass>();
}
