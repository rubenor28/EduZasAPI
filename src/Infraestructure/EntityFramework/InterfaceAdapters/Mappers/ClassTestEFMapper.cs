using Application.DTOs.Classes;
using Application.DTOs.ClassTests;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.InterfaceAdapters.Mappers;

public class ClassTestEFMapper
    : IMapper<NewClassTestDTO, TestPerClass>,
        IMapper<TestPerClass, ClassTestDomain>,
        IUpdateMapper<ClassTestUpdateDTO, TestPerClass>
{
    public TestPerClass Map(NewClassTestDTO nct) =>
        new()
        {
            TestId = nct.TestId,
            ClassId = nct.ClassId,
            Visible = nct.Visible,
        };

    public ClassTestDomain Map(TestPerClass tpc) =>
        new()
        {
            Id = new() { ClassId = tpc.ClassId, TestId = tpc.TestId },
            Visible = tpc.Visible,
            CreatedAt = tpc.CreatedAt,
        };

    public void Map(ClassTestUpdateDTO source, TestPerClass destination)
    {
      destination.Visible = source.Visible;
    }
}
