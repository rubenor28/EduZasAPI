using System;
using System.Collections.Generic;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.DTOs;

public partial class ClassProfessor
{
    public string ClassId { get; set; } = null!;

    public ulong ProfessorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User Professor { get; set; } = null!;
}
