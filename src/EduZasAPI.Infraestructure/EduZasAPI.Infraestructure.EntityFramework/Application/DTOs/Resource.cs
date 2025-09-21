using System;
using System.Collections.Generic;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.DTOs;

public partial class Resource
{
    public ulong ResourceId { get; set; }

    public bool? Active { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public ulong ProfessorId { get; set; }

    public virtual User Professor { get; set; } = null!;
}
