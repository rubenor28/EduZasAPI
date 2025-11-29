using Application.DTOs.ClassResources;
using Application.DTOs.Common;
using InterfaceAdapters.Mappers.Common;

namespace MinimalAPI.Presentation.Mappers;

public sealed class NewClassResourceMAPIMapper
    : IMapper<ClassResourceIdDTO, Executor, NewClassResourceDTO>
{
    public NewClassResourceDTO Map(ClassResourceIdDTO in1, Executor in2) =>
        new()
        {
            ClassId = in1.ClassId,
            ResourceId = in1.ResourceId,
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
