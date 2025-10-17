using Application.DTOs.Classes;
using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Classes;

public class ClassEFCreator : EFCreator<ClassDomain, NewClassDTO, Class>
{
    public ClassEFCreator(
        EduZasDotnetContext ctx,
        IMapper<Class, ClassDomain> domainMapper,
        IMapper<NewClassDTO, Class> newEntityMapper
    )
        : base(ctx, domainMapper, newEntityMapper) { }
}
