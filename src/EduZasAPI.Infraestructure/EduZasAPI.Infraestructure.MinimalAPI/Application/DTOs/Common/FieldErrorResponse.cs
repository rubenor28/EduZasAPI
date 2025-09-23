using EduZasAPI.Application.Common;

namespace EduZasAPI.Infraestructure.MinimalAPI.Application.Common;

public readonly struct FieldErrorResponse
{
    public string Message { get; init; }
    public List<FieldErrorDTO> Errors { get; init; }
}
