using EduZasAPI.Infraestructure.EntityFramework.Application.Classes;
using EduZasAPI.Infraestructure.EntityFramework.Application.Tests;

namespace EduZasAPI.Infraestructure.EntityFramework.Application.TestsPerClass;

public partial class TestPerClass
{
    public ulong TestId { get; set; }

    public string ClassId { get; set; } = null!;

    public bool Visible { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
