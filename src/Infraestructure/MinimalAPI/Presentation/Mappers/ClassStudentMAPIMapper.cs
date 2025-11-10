using Application.DTOs.Classes;
using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.ClassStudents;

namespace MinimalAPI.Presentation.Mappers;

public sealed record ClassStudentsMAPIMapper
    : IMapper<EnrollClassMAPI, Executor, EnrollClassDTO>,
        IMapper<string, ulong, Executor, UnenrollClassDTO>,
        IMapper<string, ulong, Executor, ToggleClassVisibilityDTO>
{
    EnrollClassDTO IMapper<EnrollClassMAPI, Executor, EnrollClassDTO>.Map(
        EnrollClassMAPI input,
        Executor ex
    ) =>
        new()
        {
            ClassId = input.ClassId,
            UserId = input.UserId,
            Executor = ex,
        };

    UnenrollClassDTO IMapper<string, ulong, Executor, UnenrollClassDTO>.Map(
        string classId,
        ulong studentId,
        Executor ex
    ) =>
        new()
        {
            Id = new() { ClassId = classId, UserId = studentId },
            Executor = ex,
        };

    ToggleClassVisibilityDTO IMapper<string, ulong, Executor, ToggleClassVisibilityDTO>.Map(
        string classId,
        ulong studentId,
        Executor ex
    ) =>
        new()
        {
            ClassId = classId,
            UserId = studentId,
            Executor = ex,
        };
}
