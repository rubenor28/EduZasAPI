using Domain.Entities;
using EntityFramework.Application.DAOs.Common;
using EntityFramework.Application.DTOs;
using InterfaceAdapters.Mappers.Common;

namespace EntityFramework.Application.DAOs.Classes;

public class ClassEFReader : SimpleKeyEFReader<string, ClassDomain, Class>
{
    public ClassEFReader(EduZasDotnetContext ctx, IMapper<Class, ClassDomain> domainMapper)
        : base(ctx, domainMapper) { }
}
