namespace EduZasAPI.Infraestructure.Application.DTOs;

public partial class TestsPerClassEF
{
    public ulong TestId { get; set; }

    public string ClassId { get; set; } = null!;

    public bool Visible { get; set; }

    public virtual ClassEF Class { get; set; } = null!;

    public virtual TestEF Test { get; set; } = null!;
}
