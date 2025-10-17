using Application.DTOs.Classes;
using Domain.Entities;
using EntityFramework.Application.DTOs;
using EntityFramework.InterfaceAdapters.Mappers;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Classes;

public class ClassEFUpdater : SimpleKeyEFUpdater<string, ClassDomain, ClassUpdateDTO, Class>
{
    public ClassEFUpdater(
        EduZasDotnetContext ctx,
        IMapper<Class, ClassDomain> domainMapper,
        IUpdateMapper<ClassUpdateDTO, Class> updateMapper
    )
        : base(ctx, domainMapper, updateMapper) { }
}
