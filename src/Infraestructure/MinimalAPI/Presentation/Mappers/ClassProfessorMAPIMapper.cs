using Application.DTOs.ClassProfessors;
using Application.DTOs.Common;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.ClassProfessors;

namespace MinimalAPI.Presentation.Mappers;

public sealed record ClassProfessorMAPIMapper
    : IMapper<AddProfessorToClassMAPI, Executor, AddProfessorToClassDTO>
{
    public AddProfessorToClassDTO Map(AddProfessorToClassMAPI input, Executor ex) =>
        new()
        {
            ClassId = input.ClassId,
            UserId = input.UserId,
            IsOwner = input.IsOwner,
            Executor = ex,
        };
}
