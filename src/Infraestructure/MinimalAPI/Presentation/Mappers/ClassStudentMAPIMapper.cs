using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.ClassStudents;

namespace MinimalAPI.Presentation.Mappers;

public sealed record ClassStudentsMAPIMapper
    : IMapper<EnrollClassMAPI, Executor, NewClassStudentDTO>,
        IMapper<string, ulong, Executor, DeleteClassStudentDTO>,
        IMapper<ClassStudentUpdateMAPI, Executor, ClassStudentUpdateDTO>
{
    NewClassStudentDTO IMapper<EnrollClassMAPI, Executor, NewClassStudentDTO>.Map(
        EnrollClassMAPI input,
        Executor ex
    ) =>
        new()
        {
            ClassId = input.ClassId,
            UserId = input.UserId,
            Executor = ex,
        };

    DeleteClassStudentDTO IMapper<string, ulong, Executor, DeleteClassStudentDTO>.Map(
        string classId,
        ulong studentId,
        Executor ex
    ) =>
        new()
        {
            Id = new() { ClassId = classId, UserId = studentId },
            Executor = ex,
        };

    ClassStudentUpdateDTO IMapper<ClassStudentUpdateMAPI, Executor, ClassStudentUpdateDTO>.Map(
        ClassStudentUpdateMAPI request,
        Executor ex
    ) =>
        new()
        {
            Id = new() { ClassId = request.ClassId, UserId = request.UserId },
            Hidden = request.Hidden,
            Executor = ex,
        };
}
