using EduZasAPI.Domain.Common;

namespace EduZasAPI.Application.Classes;

public class NewClassDTO
{
    public required string ClassName { get; set; }
    public required Optional<string> Subject { get; set; }
    public required Optional<string> Section { get; set; }
    public required ulong OwnerId { get; set; }
}
