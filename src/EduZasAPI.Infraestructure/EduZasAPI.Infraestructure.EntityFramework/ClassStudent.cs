using System;
using System.Collections.Generic;

namespace EduZasAPI.Infraestructure.EntityFramework;

public partial class ClassStudent
{
    public string ClassId { get; set; } = null!;

    public ulong StudentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
