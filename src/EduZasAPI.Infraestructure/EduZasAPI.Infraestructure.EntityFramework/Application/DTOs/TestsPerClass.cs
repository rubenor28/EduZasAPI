using System;
using System.Collections.Generic;

namespace EduZasAPI.Infraestructure.Application.DTOs;

public partial class TestsPerClass
{
    public ulong TestId { get; set; }

    public string ClassId { get; set; } = null!;

    public bool Visible { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
