using Application.DTOs.ClassStudents;
using Application.DTOs.Common;
using Domain.Entities;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.ClassStudents;

namespace MinimalAPI.Presentation.Mappers;

public sealed class NewClassStudentMAPIMapper
    : IMapper<EnrollClassMAPI, Executor, NewClassStudentDTO>
{
    public NewClassStudentDTO Map(EnrollClassMAPI input, Executor ex) =>
        new()
        {
            ClassId = input.ClassId,
            UserId = input.UserId,
            Executor = ex,
        };
}

public sealed class DeleteClassStudentMAPIMapper
    : IMapper<string, ulong, Executor, DeleteClassStudentDTO>
{
    public DeleteClassStudentDTO Map(string classId, ulong studentId, Executor ex) =>
        new()
        {
            Id = new UserClassRelationId { ClassId = classId, UserId = studentId },
            Executor = ex,
        };
}

public sealed class UpdateClassStudentMAPIMapper
    : IMapper<ClassStudentUpdateMAPI, Executor, ClassStudentUpdateDTO>
{
    public ClassStudentUpdateDTO Map(ClassStudentUpdateMAPI request, Executor ex) =>
        new()
        {
            ClassId = request.ClassId,
            UserId = request.UserId,
            Hidden = request.Hidden,
            Executor = ex,
        };
}
