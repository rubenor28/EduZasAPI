using Application.DTOs.ClassResources;
using Application.DTOs.Common;
using InterfaceAdapters.Mappers.Common;
using MinimalAPI.Application.DTOs.ClassResources;

namespace MinimalAPI.Presentation.Mappers;

public sealed class NewClassResourceMAPIMapper
    : IMapper<NewClassResourceMAPI, Executor, NewClassResourceDTO>
{
    public NewClassResourceDTO Map(NewClassResourceMAPI in1, Executor in2) =>
        new()
        {
            ClassId = in1.ClassId,
            ResourceId = in1.ResourceId,
            Hidden = in1.Hidden,
            Executor = in2,
        };
}

public sealed class DeleteClassResourceMAPIMapper
    : IMapper<ClassResourceIdDTO, Executor, DeleteClassResourceDTO>
{
    public DeleteClassResourceDTO Map(ClassResourceIdDTO in1, Executor in2) =>
        new()
        {
            ResourceId = in1.ResourceId,
            ClassId = in1.ClassId,
            Executor = in2,
        };
}
