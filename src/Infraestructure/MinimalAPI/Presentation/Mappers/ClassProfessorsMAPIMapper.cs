using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.ClassProfessors;

namespace MinimalAPI.Presentation.Mappers;

public sealed record ClassProfessorsMAPIMapper
    : IMapper<ClassProfessorMAPI, Executor, NewClassProfessorDTO>,
        IMapper<ClassProfessorMAPI, Executor, ClassProfessorUpdateDTO>
{
    NewClassProfessorDTO IMapper<ClassProfessorMAPI, Executor, NewClassProfessorDTO>.Map(
        ClassProfessorMAPI input,
        Executor ex
    ) =>
        new()
        {
            ClassId = input.ClassId,
            UserId = input.UserId,
            IsOwner = input.IsOwner,
            Executor = ex,
        };

    ClassProfessorUpdateDTO IMapper<ClassProfessorMAPI, Executor, ClassProfessorUpdateDTO>.Map(
        ClassProfessorMAPI value,
        Executor ex
    ) =>
        new()
        {
            Id = new() { ClassId = value.ClassId, UserId = value.UserId },
            IsOwner = value.IsOwner,
            Executor = ex,
        };
}
